// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageId;
    public string stageName;
    public string stageRound;
    public bool isClear;
    public StageData(string stageName, string stageRound, bool isClear)
    {
        this.stageId = $"{stageName}{stageRound}";
        this.stageName = stageName;
        this.stageRound = stageRound;
        this.isClear = isClear;
    }
    public StageData(string stageId, string stageName, string stageRound, bool isClear)
    {
        this.stageId = stageId;
        this.stageName = stageName;
        this.stageRound = stageRound;
        this.isClear = isClear;
    }
}

public class BackendStageData : GameData
{

    // 아이템을 담는 Dictionary
    private Dictionary<string, StageData> _dictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    public IReadOnlyDictionary<string, StageData> Dictionary =>
        (IReadOnlyDictionary<string, StageData>)_dictionary.AsReadOnlyCollection();

    // 테이블 이름 설정 함수
    public override string GetTableName()
    {
        return "STAGE_DATA";
    }

    // 컬럼 이름 설정 함수
    public override string GetColumnName()
    {
        return "STAGE_DATA";
    }

    // 데이터가 존재하지 않을 경우, 초기값 설정
    protected override void InitializeData()
    {
        _dictionary.Clear();
        // 차트에서 불러온 스테이지 정보 입력
        foreach(var info in BackendManager.Instance.Chart.StageChart.Dictionary)
        {
            StageInfo stageInfo = info.Value;
            AddStageData(new StageData(stageInfo.stageId, stageInfo.stageName, stageInfo.stageRound, false));
        }
    }


    // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
    // Dictionary 하나만 삽입
    public override Param GetParam()
    {
        Param param = new Param();
        param.Add(GetColumnName(), _dictionary);

        return param;

    }

    // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
    // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
    protected override void SetServerDataToLocal(JsonData gameDataJson)
    {
        for (int i = 0; i < gameDataJson.Count; i++)
        {
            string stageId = gameDataJson[i]["stageId"].ToString();
            string stageName = gameDataJson[i]["stageName"].ToString();
            string stageRound = gameDataJson[i]["stageRound"].ToString();
            bool isClear = bool.Parse(gameDataJson[i]["isClear"].ToString());

            _dictionary.Add(stageId,
                new StageData(stageId, stageName, stageRound, isClear));
        }
        CheckDictionary();
    }

    private void CheckDictionary()
    {
        foreach (var info in BackendManager.Instance.Chart.StageChart.Dictionary)
        {
            StageInfo stageInfo = info.Value;
            if (!_dictionary.ContainsKey(stageInfo.stageId))
                AddStageData(new StageData(stageInfo.stageId, stageInfo.stageName, stageInfo.stageRound, false));
        }

        if (!IsChangedData)
            return;

        Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                IsChangedData = false;
                Debug.Log($"추가 스테이지 적용 성공: {callback}");
            }
            else
            {
                Debug.LogError($"추가 스테이지 적용 실패: {callback})");
            }
        });
    }


    public string AddStageData(StageData stageData)
    {
        IsChangedData = true;

        //DateTime now = DateTime.Now;
        //var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //string myStageDataId = Convert.ToString(Convert.ToInt64((now - epoch).TotalMilliseconds));
        string myStageDataId = $"{stageData.stageName}{stageData.stageRound}";
        Debug.Log(myStageDataId);

        _dictionary.Add(myStageDataId, stageData);

        return myStageDataId;
    }
    public void ClearStage(string stageId)
    {
        IsChangedData = true;
        _dictionary[stageId].isClear = true;
        Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                Debug.Log($"스테이지 클리어 적용 성공: {callback}");
            }
            else
            {
                Debug.LogError($"스테이지 클리어 적용 실패: {callback})");
            }
        });
    }
    public void ClearToggle(string stageId)
    {
        IsChangedData = true;
        _dictionary[stageId].isClear = !_dictionary[stageId].isClear;
    }
    public bool IsClearStage(string stageId)
    {
        return Dictionary[stageId].isClear;
    }
}