// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendManager : Singleton<BackendManager>
{
    public class BackendProbability
    {
        public AllProbability ProbabilityInfo = new();  // 모든 뽑기
        public BackendGachaPetProbability GachaPetProbability = new();
    }

    public class BackendChart
    {
        public AllChart ChartInfo = new(); // 모든 차트
        public BackendPetChart PetChart = new();
        public BackendEnemyChart EnemyChart = new();
        public BackendStageChart StageChart = new();
        public BackendItemChart ItemChart = new();
        public BackendSkillChart SkillChart = new();
    }

    // 게임 정보 관리 데이터만 모아놓은 클래스
    public class BackendGameData
    {
        public BackendUserData UserData = new();
        public BackendStageData StageData = new();
        public BackendUserPetData UserPetData = new();

        public readonly Dictionary<string, GameData>
            GameDataList = new Dictionary<string, GameData>();

        public BackendGameData()
        {
            GameDataList.Add("내 정보", UserData);
            GameDataList.Add("내 스테이지 정보", StageData);
            GameDataList.Add("내 펫 정보", UserPetData);
        }
    }


    public BackendProbability Probability = new();  // 뽑기 모음 클래스 생성
    public BackendChart Chart = new(); // 차트 모음 클래스 생성
    public BackendGameData GameData = new(); // 게임 모음 클래스 생성
    public BackendPost Post = new();
    public BackendNotice Notice = new();

    void Start()
    {
        Init();
    }


    public void InitInGameData()
    {
        Probability = new();
        Chart = new();
        GameData = new();
        Post = new();
        Notice = new();
    }


    // 뒤끝 매니저 초기화 함수
    public void Init()
    {
        var initializeBro = Backend.Initialize(true);

        // 초기화 성공시
        if (initializeBro.IsSuccess())
        {
            Debug.Log("뒤끝 초기화가 완료되었습니다.");
            CreateSendQueueMgr();
            SetErrorHandler();
        }
        //초기화 실패시
        else
        {
            Debug.LogError("초기화 실패 : " + initializeBro); // 실패일 경우 statusCode 400대 에러 발생 
        }
    }

    //비동기 함수를 메인쓰레드로 보내어 UI에 용이하게 접근하도록 도와주는 Poll 함수
    void Update()
    {
        if (Backend.IsInitialized)
        {
            Backend.AsyncPoll();
            Backend.ErrorHandler.Poll();
        }
    }

    // 모든 뒤끝 함수에서 에러 발생 시, 각 에러에 따라 호출해주는 핸들러
    private void SetErrorHandler()
    {
        Backend.ErrorHandler.InitializePoll(true);

        // 서버 점검 에러 발생 시
        Backend.ErrorHandler.OnMaintenanceError = () => {
            Debug.Log("점검 에러 발생!!!");
            Debug.Log("서버 점검 중 / 현재 서버 점검중입니다.\n타이틀로 돌아갑니다.");
        };
        // 403 에러 발생시
        Backend.ErrorHandler.OnTooManyRequestError = () => {
            Debug.Log("비정상적인 행동 감지 / 비정상적인 행동이 감지되었습니다.\n타이틀로 돌아갑니다.");
        };
        // 액세스토큰 만료 후 리프레시 토큰 실패 시
        Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () => {
            Debug.Log("다른 기기 접속 감지 / 다른 기기에서 로그인이 감지되었습니다.\n타이틀로 돌아갑니다.");
        };
    }

    //SendQueue를 관리해주는 SendQueue 매니저 생성
    private void CreateSendQueueMgr()
    {
        var obj = new GameObject();
        obj.name = "SendQueueMgr";
        obj.transform.SetParent(this.transform);
        obj.AddComponent<SendQueueMgr>();
    }


    // 업데이트가 발생한 이후에 호출에 대한 응답을 반환해주는 대리자 함수
    public delegate void AfterUpdateFunc(BackendReturnObject callback);

    // 값이 바뀐 데이터가 있는지 체크후 바뀐 데이터들은 바로 저장 혹은 트랜잭션에 묶어 저장을 진행하는 함수
    public void UpdateAllGameData(AfterUpdateFunc afterUpdateFunc)
    {
        string info = string.Empty;


        // 바뀐 데이터가 몇개 있는지 체크
        List<GameData> gameDatas = new List<GameData>();

        foreach (var gameData in GameData.GameDataList)
        {
            if (gameData.Value.IsChangedData)
            {
                info += gameData.Value.GetTableName() + "\n";
                gameDatas.Add(gameData.Value);
            }
        }

        if (gameDatas.Count <= 0)
        {
            afterUpdateFunc(null); // 지정한 대리자 함수 호출

            // 업데이트할 목록이 존재하지 않습니다.
        }
        else if (gameDatas.Count == 1)
        {

            //하나라면 찾아서 해당 테이블만 업데이트
            foreach (var gameData in gameDatas)
            {
                if (gameData.IsChangedData)
                {
                    gameData.Update(callback => {

                        //성공할경우 데이터 변경 여부를 false로 변경
                        if (callback.IsSuccess())
                        {
                            gameData.IsChangedData = false;
                        }
                        else
                        {
                            Debug.Log($"변경 실패: {callback}");
                        }
                        Debug.Log($"UpdateV2 : {callback}\n업데이트 테이블 : \n{info}");
                        if (afterUpdateFunc == null)
                        {

                        }
                        else
                        {
                            afterUpdateFunc(callback); // 지정한 대리자 함수 호출
                        }
                    });
                }
            }
        }
        else
        {
            // 2개 이상이라면 트랜잭션에 묶어서 업데이트
            // 단 10개 이상이면 트랜잭션 실패 주의
            List<TransactionValue> transactionList = new List<TransactionValue>();

            // 변경된 데이터만큼 트랜잭션 추가
            foreach (var gameData in gameDatas)
            {
                transactionList.Add(gameData.GetTransactionUpdateValue());
            }

            SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, transactionList, callback => {
                Debug.Log($"Backend.BMember.TransactionWriteV2 : {callback}");

                if (callback.IsSuccess())
                {
                    foreach (var data in gameDatas)
                    {
                        data.IsChangedData = false;
                    }
                }
                else
                {
                    Debug.Log($"변경 실패: {callback}");
                }

                Debug.Log($"TransactionWriteV2 : {callback}\n업데이트 테이블 : \n{info}");

                if (afterUpdateFunc == null)
                {

                }
                else
                {

                    afterUpdateFunc(callback);  // 지정한 대리자 함수 호출
                }
            });
        }
    }
}
