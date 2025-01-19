using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager : Singleton<TestManager>
{
    private delegate void BackendLoadStep();
    private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();

    public void Init()
    {
        Debug.Log($"{GetType()}//////////////////////////");
        _initializeStep.Clear();

        // 랜덤차트 정보 불러오기 함수
        _initializeStep.Enqueue(() => { BackendManager.Instance.Probability.ProbabilityInfo.BackendLoad(NextStep); });

        //// 차트정보 불러오기 함수 Insert
        _initializeStep.Enqueue(() => { BackendManager.Instance.Chart.ChartInfo.BackendLoad(NextStep); });
        _initializeStep.Enqueue(() => { BackendManager.Instance.Chart.PetChart.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { BackendManager.Instance.Chart.EnemyChart.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { BackendManager.Instance.Chart.StageChart.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { BackendManager.Instance.Chart.ItemChart.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { BackendManager.Instance.Chart.SkillChart.BackendChartDataLoad(NextStep); });


        // 트랜잭션으로 불러온 후, 안불러질 경우 각자 Get 함수로 불러오는 함수 *중요*
        _initializeStep.Enqueue(() => { TransactionRead(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("적 정보"); StaticManager.Backend.Chart.Enemy.BackendChartDataLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("스테이지 정보"); StaticManager.Backend.Chart.Stage.BackendChartDataLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("아이템 정보"); StaticManager.Backend.Chart.Item.BackendChartDataLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("상점 정보"); StaticManager.Backend.Chart.Shop.BackendChartDataLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("퀘스트 정보"); StaticManager.Backend.Chart.Quest.BackendChartDataLoad(NextStep); });

        //// 랭킹 정보 불러오기 함수 Insert
        //_initializeStep.Enqueue(() => { ShowDataName("랭킹 정보 불러오기"); StaticManager.Backend.Rank.BackendLoad(NextStep); });
        //// 우편 정보 불러오기 함수 Insert
        //_initializeStep.Enqueue(() => { ShowDataName("관리자 우편 정보 불러오기"); StaticManager.Backend.Post.BackendLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("랭킹 우편 정보 불러오기"); StaticManager.Backend.Post.BackendLoadForRank(NextStep); });

        ////다음 씬으로 넘어가는 함수 Insert


        ////게이지 바 지정
        //_maxLoadingCount = _initializeStep.Count;
        //loadingSlider.maxValue = _maxLoadingCount;

        //_currentLoadingCount = 0;
        //loadingSlider.value = _currentLoadingCount;

        //// 로딩아이콘 활성화
        //StaticManager.UI.SetLoadingIcon(true);
        //Queue에 저장된 함수 순차적으로 실행
        BackendManager.Instance.InitInGameData();
        NextStep(true, string.Empty, string.Empty, string.Empty);
    }

    // 각 뒤끝 함수를 호출하는 BackendGameDataLoad에서 실행한 결과를 처리하는 함수
    // 성공하면 다음 스텝으로 이동, 실패하면 에러 UI를 띄운다.
    private void NextStep(bool isSuccess, string className, string funcName, string errorInfo)
    {
        if (isSuccess)
        {

            if (_initializeStep.Count > 0)
            {
                _initializeStep.Dequeue().Invoke();
            }
            else
            {
                // 로딩끝
                SceneManager.LoadScene("MainScene");
            }
        }
        else
        {
            Debug.LogError($"데이터 로드 실패!: {errorInfo}");
        }
    }

    private void TransactionRead(Normal.AfterBackendLoadFunc func)
    {
        bool isSuccess = false;
        string className = GetType().Name;
        string functionName = MethodBase.GetCurrentMethod()?.Name;
        string errorInfo = string.Empty;

        //트랜잭션 리스트 생성
        List<TransactionValue> transactionList = new List<TransactionValue>();

        // 게임 테이블 데이터만큼 트랜잭션 불러오기
        foreach (var gameData in BackendManager.Instance.GameData.GameDataList)
        {
            transactionList.Add(gameData.Value.GetTransactionGetValue());
        }

        // [뒤끝] 트랜잭션 읽기 함수
        SendQueue.Enqueue(Backend.GameData.TransactionReadV2, transactionList, callback => {
            try
            {
                Debug.Log($"Backend.GameData.TransactionReadV2 : {callback}");

                // 데이터를 모두 불러왔을 경우
                if (callback.IsSuccess())
                {
                    JsonData gameDataJson = callback.GetFlattenJSON()["Responses"];
                    int index = 0;

                    foreach (var gameData in BackendManager.Instance.GameData.GameDataList)
                    {

                        _initializeStep.Enqueue(() => {
                            // 불러온 데이터를 로컬에서 파싱
                            gameData.Value.BackendGameDataLoadByTransaction(gameDataJson[index++], NextStep);
                        });

                    }
                    isSuccess = true;
                }
                else
                {
                    // 트랜잭션으로 데이터를 찾지 못하여 에러가 발생한다면 개별로 GetMyData로 호출
                    foreach (var gameData in BackendManager.Instance.GameData.GameDataList)
                    {
                        _initializeStep.Enqueue(() => {
                            // GetMyData 호출
                            gameData.Value.BackendGameDataLoad(NextStep);
                        });
                    }
                    // 최대 작업 개수 증가
                    //
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                errorInfo = e.ToString();
            }
            finally
            {
                func.Invoke(isSuccess, className, functionName, errorInfo);
            }
        });
    }

    //private void ShowDataName(string text)
    //{
    //    string info = $"{text} 불러오는 중...({_currentLoadingCount}/{_maxLoadingCount})";
    //    loadingText.text = info;
    //    Debug.Log(info);
    //}

    public void Test1()
    {
        BackendManager.Instance.GameData.UserData.BackendGameDataLoad(NextStep1);
        BackendManager.Instance.GameData.StageData.BackendGameDataLoad(NextStep1);
        BackendManager.Instance.GameData.UserPetData.BackendGameDataLoad(NextStep1);
    }
    public void Test2(string stageId)
    {
        BackendManager.Instance.GameData.StageData.ClearToggle(stageId);
        BackendManager.Instance.UpdateAllGameData(null);
    }
    private void NextStep1(bool isSuccess, string className, string funcName, string errorInfo)
    {
        if (isSuccess)
        {
            Debug.Log("로딩 성공");
        }
        else
        {
            Debug.Log($"로딩 실패: {errorInfo}");
        }
    }
}
