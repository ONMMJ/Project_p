using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("UIPanel")]
    [SerializeField] Transform Canvas;
    [SerializeField] BaseUI petInvenUIPrefab;
    [SerializeField] BaseUI pickUpPrefab;
    [SerializeField] BaseUI stageUIPrefab;
    [SerializeField] BaseUI battleStartUIPrefab;
    [SerializeField] BaseUI postUIPrefab;
    [SerializeField] BaseUI noticeUIPrefab;
    [SerializeField] BaseUI settingUIPrefab;


    [Header("MenuButton")]
    [SerializeField] Button shopButton;
    [SerializeField] Button postButton;
    [SerializeField] Button noticeButton;
    [SerializeField] Button invenButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button backButton;
    [SerializeField] Button battleButton;

    [Header("UserUI")]
    [SerializeField] TMP_Text goldText;

    public PetInvenUI InvenUI { get; private set; }
    public InfoUI InfoUI { get; private set; }
    public LeftButton LeftButtonUI { get; private set; }

    public PickUpUI PickUpUI { get; private set; }
    public StatusDetailUI StatusUI { get; private set; }
    public StageManager StageUI { get; private set; }
    public BattleStartUI BattleStartUI { get; private set; }

    public PostUI PostUI { get; private set; }
    public NoticeUI NoticeUI { get; private set; }
    public SettingUI SettingUI { get; private set; }

    [HideInInspector] public Stack<BaseUI> activeUIStack;

    //Dictionary<KeyCode, Action> keyDictionary;

    void Start()
    {
        activeUIStack = new();
        //keyDictionary = new Dictionary<KeyCode, Action>
        //{
        //    { KeyCode.I, ()=>OpenInvenUI(LEFT_BUTTON_PANEL.Info) },
        //    {KeyCode.Escape, CloseActiveUI }
        //};
        SetButtonListener();

        Init();
    }

    void Init()
    {
        backButton.gameObject.SetActive(false);
        SetUserUI();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.anyKeyDown)
    //    {
    //        foreach (var dic in keyDictionary)
    //        {
    //            if (Input.GetKeyDown(dic.Key))
    //            {
    //                dic.Value();
    //            }
    //        }
    //    }
    //}

    public void OpenInvenUI(LEFT_BUTTON_PANEL panel)
    {
        if (InvenUI == null)
        {
            BaseUI obj = Instantiate(petInvenUIPrefab, Canvas);
            InfoUI = obj.GetComponent<InfoUI>();
            InvenUI = obj.GetComponent<PetInvenUI>();
            LeftButtonUI = obj.GetComponent<LeftButton>();
            LeftButtonUI.Setup();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = InvenUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
                obj.SetActive(true);
        }
        LeftButtonUI.ActivePanel(panel);
    }

    public void OpenPickUpUI()
    {
        if (PickUpUI == null)
        {
            BaseUI obj = Instantiate(pickUpPrefab, Canvas);
            PickUpUI = obj.GetComponent<PickUpUI>();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = PickUpUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
                obj.SetActive(true);
        }
    }

    public void OpenStageUI()
    {
        if (StageUI == null)
        {
            BaseUI obj = Instantiate(stageUIPrefab, Canvas);
            StageUI = obj.GetComponent<StageManager>();
            CloseAllUI();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = StageUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
            {
                CloseAllUI();
                obj.SetActive(true);
            }
        }
    }
    public void OpenPostUI()
    {
        if (PostUI == null)
        {
            BaseUI obj = Instantiate(postUIPrefab, Canvas);
            PostUI = obj.GetComponent<PostUI>();
            CloseAllUI();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = PostUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
            {
                CloseAllUI();
                obj.SetActive(true);
            }
        }
    }
    public void OpenNoticeUI()
    {
        if (NoticeUI == null)
        {
            BaseUI obj = Instantiate(noticeUIPrefab, Canvas);
            NoticeUI = obj.GetComponent<NoticeUI>();
            CloseAllUI();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = NoticeUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
            {
                CloseAllUI();
                obj.SetActive(true);
            }
        }
    }
    public void OpenSettingUI()
    {
        if (SettingUI == null)
        {
            BaseUI obj = Instantiate(settingUIPrefab, Canvas);
            SettingUI = obj.GetComponent<SettingUI>();
            CloseAllUI();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = SettingUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
            {
                CloseAllUI();
                obj.SetActive(true);
            }
        }
    }

    private void CloseActiveUI()
    {
        if (activeUIStack.Count > 0)
        {
            GameObject obj = activeUIStack.Pop().gameObject;
            obj.SetActive(false);
        }
        else
        {
            // 환경설정 창
        }
    }
    public void OpenUI(BaseUI obj)
    {
        if (activeUIStack.Count > 0)
        {
            var prev = activeUIStack.Peek();
            prev.ResetPosition();
        }
        obj.transform.SetAsLastSibling();
        activeUIStack.Push(obj);
    }
    public void ClosePrevUI(BaseUI obj)
    {
        Debug.Log(obj.name);
        while (activeUIStack.Count > 0)
        {
            BaseUI prev = activeUIStack.Pop();
            if(prev == obj)
            {
                obj.CloseUI();
                Debug.Log(obj.name);
                break;
            }
            else
            {
                prev.CloseUI();
            }
        }
    }
    public void CloseAllUI()
    {
        while (activeUIStack.Count > 0)
        {
            activeUIStack.Pop().CloseUI();
        }
    }

    private void SetButtonListener()
    {
        shopButton.onClick.AddListener(OpenPickUpUI);
        invenButton.onClick.AddListener(()=>OpenInvenUI(LEFT_BUTTON_PANEL.Info));
        battleButton.onClick.AddListener(OpenStageUI);
        postButton.onClick.AddListener(OpenPostUI);
        noticeButton.onClick.AddListener(OpenNoticeUI);
        settingButton.onClick.AddListener(OpenSettingUI);
        backButton.onClick.AddListener(()=> { EndBattle("GameEnd"); });
    }
    public void EndBattle(string clearText)
    {
        if (BattleCanvas.Instance == null)
            return;
        BattleCanvas.Instance.GameEnd(clearText);
        battleButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
    }

    public void OpenBattleStartUI(string stageId)
    {
        if (BattleStartUI == null)
        {
            BaseUI obj = Instantiate(battleStartUIPrefab, Canvas);
            BattleStartUI = obj.GetComponent<BattleStartUI>();
            obj.SetActive(true);
        }
        else
        {
            BaseUI obj = BattleStartUI;
            if (obj.activeSelf)
                ClosePrevUI(obj);
            else
                obj.SetActive(true);
        }
        if (stageId.Equals("Mission"))
            BattleStartUI.SetMission();
        else
            BattleStartUI.Setup(stageId);
    }
    public void StartBattle()
    {
        battleButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        CloseAllUI();
    }
    public void HideUI(bool isHide)
    {
        Canvas.gameObject.SetActive(!isHide);
    }
    public void SetUserUI()
    {
        goldText.text = BackendManager.Instance.GameData.UserData.gold.ToString();
    }
}
