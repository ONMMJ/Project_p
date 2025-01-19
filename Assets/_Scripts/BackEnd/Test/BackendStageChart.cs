using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyInfo;

public class StageInfo
{
    public string stageId;
    public string stageName;
    public string stageRound;
    public string sceneName;
    public string stageText;

    public StageInfo(JsonData json)
    {
        stageName = json["stageName"].ToString();
        stageRound = json["stageRound"].ToString();
        sceneName = json["sceneName"].ToString();
        stageText = json["stageText"].ToString();

        stageId = $"{stageName}{stageRound}";
    }
}

public class BackendStageChart : Chart
{

    // 각 차트의 row 정보를 담는 Dictionary
    private readonly Dictionary<string, StageInfo> _dictionary = new();
    private readonly Dictionary<string, string> _stageDictionary = new();
    private readonly Dictionary<string, List<string>> _roundDictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    public IReadOnlyDictionary<string, StageInfo> Dictionary => (IReadOnlyDictionary<string, StageInfo>)_dictionary.AsReadOnlyCollection();
    public IReadOnlyDictionary<string, string> StageTextDictionary => (IReadOnlyDictionary<string, string>)_stageDictionary.AsReadOnlyCollection();
    public IReadOnlyDictionary<string, List<string>> RoundDictionary => (IReadOnlyDictionary<string, List<string>>)_roundDictionary.AsReadOnlyCollection();


    // 차트 파일 이름 설정 함수
    // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
    public override string GetChartFileName()
    {
        return "STAGE_CHART";
    }

    // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
    // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고해주세요
    protected override void LoadChartDataTemplate(JsonData json)
    {
        List<StageInfo> temp = new();
        foreach (JsonData eachItem in json)
        {
            StageInfo info = new StageInfo(eachItem);
            _dictionary.Add(info.stageId, info);
            temp.Add(info);
        }

        var groupStage = temp.GroupBy(x => x.stageName);
        foreach(var stageList in groupStage)
        {
            string stageName = stageList.Key;
            List<string> stageRoundList = new();
            foreach (var stage in stageList)
            {
                stageRoundList.Add(stage.stageRound);
            }
            _stageDictionary.Add(stageName, stageList.ElementAt(0).stageText);
            _roundDictionary.Add(stageName, stageRoundList);
        }
    }

    public string GetSceneName(string stageId)
    {
        return Dictionary[stageId].sceneName;
    }
    public string GetStageNameText(string stageName)
    {
        return StageTextDictionary[stageName];
    }
    public string GetStageRoundText(string stageId)
    {
        return $"{Dictionary[stageId].stageText} {Dictionary[stageId].stageRound}";
    }
    public List<string> GetStageRoundList(string stageName)
    {
        return RoundDictionary[stageName];
    }
}