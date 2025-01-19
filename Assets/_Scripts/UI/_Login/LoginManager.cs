using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

using BackEnd;
using static BackEnd.SendQueue;
using LitJson;

public class LoginManager : MonoBehaviour
{
    [Header("Popups")]
    public List<GameObject> popups = new List<GameObject>();

    //UI objects
    [SerializeField] GameObject coverPanel;
    [SerializeField] GameObject updatePopup;
    [SerializeField] GameObject privacyUI;

    [Header("Login")]
    [SerializeField] GameObject loginUI;
    [SerializeField] Button guestLoginButton;
    [SerializeField] Button googleLoginButton;
    [SerializeField] TMP_Text loginErrorText;

    [Header("NickName")]
    [SerializeField] GameObject nickNameUI;
    [SerializeField] TMP_Text nicknameText;

    [SerializeField] ErrorPopupScript errorPopup;

    private string currentVersion = "";
    
    //update에서 비동기 처리하기 위한 플래그 값
    BackendReturnObject bro = new BackendReturnObject();

    public void TEST_ID_DELETE()
    {
        Debug.Log("아이디 삭제되었음");
        Backend.BMember.DeleteGuestInfo();
    }

    // ====================================================================================================
    #region 초기화
    private void Start()
    {
        Debug.Log("#### 초기화 시작 ####");
        //UI 초기화
        InitCommonUI();

    }

    void InitCommonUI()
    {
        Debug.Log("#### UI 초기화 시작 ####");
        try
        {
            popups.Add(coverPanel);
            popups.Add(updatePopup);
            popups.Add(privacyUI);
            popups.Add(loginUI);
            popups.Add(nickNameUI);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        Debug.Log("#### UI 초기화 완료 ####");
    }

    //시작하려면 터치하세요에서 부른다.
    public void InitMainBackendProcess()
    {

#if UNITY_ANDROID
        // ----- GPGS -----
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()
            .RequestIdToken()
            .Build();

        //커스텀된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        //GPGS 시작.
        PlayGamesPlatform.Activate();
#endif
        currentVersion = Application.version;
        // ----- 뒤끝 -----

        ShowUI(coverPanel); //프로세싱 팝업 켜기

        var bro = Backend.Initialize(true);
        if (bro.IsSuccess())
        {
            if (bro.IsSuccess() == false)
            {
                Debug.Log("초기화 실패 - " + bro);
                return;
            }
            ShowUI(coverPanel);

            // 구글 해시키 획득 
            if (!string.IsNullOrEmpty(Backend.Utils.GetGoogleHash()))
                Debug.Log(Backend.Utils.GetGoogleHash());

            // 서버시간 획득
            Debug.Log(Backend.Utils.GetServerTime());
            // Application 버전 확인
            CheckApplicationVersion();
        }
        else
        {
            Debug.Log("초기화 실패 - " + bro);
        }

    }

    #endregion

    // ====================================================================================================
    #region 로그인 / 로그아웃
    private void CheckApplicationVersion()
    {
        Debug.Log("#### 버전 체크 시작 ####");

        Backend.Utils.GetLatestVersion(versionBRO =>
        {
            if (versionBRO.IsSuccess())
            {
                string latest = versionBRO.GetReturnValuetoJSON()["version"].ToString();
                Debug.Log("version info - current: " + currentVersion + " latest: " + latest);
                if (currentVersion != latest)
                {
                    int type = (int)versionBRO.GetReturnValuetoJSON()["type"];
                    // type = 1 : 선택, type = 2 : 강제
                    bool isForced = type == 1;
                    DispatcherAction(() => ShowUpdateUI(isForced));
                }
                else
                {
                    // 뒷끝 토큰 로그인 시도
                    LoginWithTheBackendToken();
                }
            }
            else
            {
                // 뒷끝 토큰 로그인 시도
                LoginWithTheBackendToken();
            }
        });
    }
    public void LoginWithTheBackendToken()
    {
        Debug.Log("#### 뒤끝 로그인 시작 ####");

        Enqueue(Backend.BMember.LoginWithTheBackendToken, loginBro =>
        {
            if (loginBro.IsSuccess())
            {
                SuccessLogin();
            }
            else
            {
                // 뒤끝 토큰 로그인 실패
                // 개인정보 취급 UI 열기
                DispatcherAction(ShowPrivacyUI);
                Debug.Log("로그인 실패 - " + loginBro.ToString());
            }
        });
    }

    public void SignUpGuest()
    {
        string id = Backend.BMember.GetGuestID();
        Debug.Log("로컬 기기에 저장된 아이디 :" + id);

        if(string.IsNullOrEmpty(id))
        {
            Enqueue(Backend.BMember.GuestLogin, "게스트 회원가입으로 로그인함", callback => {
                if (callback.IsSuccess())
                {
                    DispatcherAction(() => ShowNickNameUI(true));
                }
            });
        }
        else
        {
            string nickName = Backend.UserNickName;
            if(string.IsNullOrEmpty(nickName))
            {
                Debug.Log("게스트 아이디는 있는데 닉네임이 없는경우");
                //닉네임생성
                DispatcherAction(() => ShowNickNameUI(true));
            }
            else
            {
                Debug.Log("닉네임이 있는데 ?? 로그인이 안된경우?");
            }
        }
    }



    void Update()
    {
        Dispatcher.Instance.InvokePending();
        if (Backend.IsInitialized)
        {
            Backend.AsyncPoll();
        }
    }
    void SuccessLogin()
    {
        bro.Clear();
        OnBackendAuthorized();
    }

    public void OnBackendAuthorized()
    {
        Debug.Log("#### 뒤끝 로그인 성공 후 다음 프로세스 진행 ####");

        GetUserInfo();  // 유저정보 가져오기 후 변수에 저장
        //GetRemoveAds(); // 유저 광고제거 구매정보 가져오기 후 저장
        //UpdateScore2(0);// 최고점수 갱신
    }

    #endregion

    // ====================================================================================================
    #region 유저 정보

    public void GetUserInfo()
    {
        
        Backend.BMember.GetUserInfo(userInfoBro =>
        {
            if (userInfoBro.IsSuccess())
            {
                JsonData Userdata = userInfoBro.GetReturnValuetoJSON()["row"];
                JsonData nicknameJson = Userdata["nickname"];

                // 닉네임 여부를 확인
                if (nicknameJson != null)
                {
                    Debug.Log(">>> NickName is " + nicknameJson.ToString());
                    DispatcherAction(CloseAll);

                    // 닉네임이 존재할 시에만 채팅서버에 접속
                    //DispatcherAction(BackEndChatManager.instance.EnterChatServerInUserInfo);
                    SceneChange();
                }
                else
                {
                    Debug.Log(">>> NickName이 없음.");
                    DispatcherAction(() => ShowNickNameUI(true));
                }
            }
            else
            {
                Debug.Log("[X]유저 정보 가져오기 실패 - " + userInfoBro);
            }
        });
        
    }


    #endregion

    // ====================================================================================================
    #region UI 관련 함수
    private void ShowUI(GameObject uiObejct)
    {
        foreach (GameObject x in popups)
        {
            if (x == uiObejct)
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(false);
            }
        }
    }
    // 해당 UI 닫기
    private void CloseUI(GameObject uiObejct)
    {
        foreach (GameObject x in popups)
        {
            if (x == uiObejct)
            {
                x.SetActive(false);
            }
        }
    }
    public void ShowUpdateUI(bool active)
    {
        updatePopup.GetComponentInChildren<Button>().gameObject.SetActive(active);
        ShowUI(updatePopup);
    }

    public void ShowNickNameUI(bool active)
    {
        ShowUI(nickNameUI);
    }

    public void ShowPrivacyUI()
    {
        ShowUI(privacyUI);
    }
    public void ShowLoginUI()
    {
        CloseUI(privacyUI);
        ShowUI(loginUI);
    }

    public void NicknameCreate()
    {
        Enqueue(Backend.BMember.CreateNickname, nicknameText.text, (callback) =>
        {
            Debug.Log(nicknameText.text + "로 닉네임 생성 시도");

            if(callback.GetStatusCode() == "204")
            {
                SceneChange();
            }
            else if (callback.GetStatusCode() == "409")
            {
                Debug.Log("중복된 닉네임입니다.");
            }
            else
            {
                Debug.Log("잘못된 닉네임입니다." + callback.GetStatusCode());
            }
        });

    }

    // 마켓 링크 열기
    public void GetLatestVersion()
    {
//#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
//#endif
    }

    // 처리상황 반환
    public bool IsProcessing()
    {
        return (coverPanel.activeSelf || updatePopup.activeSelf);
    }

    public void CloseAll()
    {
        foreach (GameObject @object in popups)
        {
            @object.SetActive(false);
        }
    }

    private void DispatcherAction(Action action)
    {
        Dispatcher.Instance.Invoke(action);
    }


    #endregion

    #region 씬 넘기기

    public void SceneChange()
    {
        TestManager.Instance.Init();
    }

    #endregion
}
