// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class EnemyInfo
{
    public class DropItem
    {
        public string itemID { get; private set; }

        public float dropPercent { get; private set; }

        public int itemCount { get; private set; }

        public DropItem(string itemID, float dropPercent, int itemCount)
        {
            this.itemID = itemID;
            this.dropPercent = dropPercent;
            this.itemCount = itemCount;
        }
    }
    public string stageId { get; private set; }
    public string stageName { get; private set; }
    public string stageRound { get; private set; }
    public bool isStage { get; private set; }
    public bool isGround { get; private set; }
    public string petName { get; private set; }
    public int petLevel { get; private set; }
    public float spawnPercent { get; private set; }
    public int exp { get; private set; }
    public int money { get; private set; }

    public List<DropItem> dropItemList { get; private set; }


    public EnemyInfo(JsonData json)
    {
        stageName = json["stageName"].ToString();
        stageRound = json["stageRound"].ToString();
        isStage = bool.Parse(json["isStage"].ToString());
        isGround = bool.Parse(json["isGround"].ToString());
        petName = json["petName"].ToString();
        petLevel = int.Parse(json["petLevel"].ToString());
        spawnPercent = float.Parse(json["spawnPercent"].ToString());
        exp = int.Parse(json["exp"].ToString());
        money = int.Parse(json["money"].ToString());

        stageId = $"{stageName}{stageRound}";

        string dropItemListString = json["dropItemList"].ToString();

        if (string.IsNullOrEmpty(dropItemListString) || dropItemListString == "null")
        {
            return;
        }

        JsonData dropItemListJson = JsonMapper.ToObject(dropItemListString);
        dropItemList = new();
        foreach (JsonData item in dropItemListJson)
        {
            string itemID = item["name"].ToString();
            float percent = float.Parse(item["percent"].ToString());
            int itemCount = int.Parse(item["count"].ToString());
            dropItemList.Add(new DropItem(itemID, percent, itemCount));
        }
    }
}

public class BackendEnemyChart : Chart
{

    // 각 차트의 row 정보를 담는 Dictionary
    //private readonly Dictionary<string, List<string>> _stageDictionary = new();
    private readonly Dictionary<string, List<EnemyInfo>> _stageEnemyDictionary = new();
    private readonly Dictionary<string, List<EnemyInfo>> _groundEnemyDictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    //public IReadOnlyDictionary<string, List<string>> StageDictionary => (IReadOnlyDictionary<string, List<string>>)_stageDictionary.AsReadOnlyCollection();
    public IReadOnlyDictionary<string, List<EnemyInfo>> StageEnemyDictionary => (IReadOnlyDictionary<string, List<EnemyInfo>>)_stageEnemyDictionary.AsReadOnlyCollection();
    public IReadOnlyDictionary<string, List<EnemyInfo>> GroundEnemyDictionary => (IReadOnlyDictionary<string, List<EnemyInfo>>)_groundEnemyDictionary.AsReadOnlyCollection();

    // 차트 파일 이름 설정 함수
    // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
    public override string GetChartFileName()
    {
        return "ENEMY_CHART";
    }

    // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
    // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고해주세요
    protected override void LoadChartDataTemplate(JsonData json)
    {
        List<EnemyInfo> temp = new();
        foreach (JsonData eachItem in json)
        {
            EnemyInfo info = new EnemyInfo(eachItem);
            temp.Add(info);
        }
        var groupEnemy = temp.GroupBy(x => x.stageId);
        foreach (var enemies in groupEnemy)
        {
            string stageId = enemies.Key;
            List<EnemyInfo> stageEnemies = new();
            List<EnemyInfo> groundEnemies = new();
            foreach(var enemy in enemies)
            {
                if (enemy.isStage)
                    stageEnemies.Add(enemy);
                if (enemy.isGround)
                    groundEnemies.Add(enemy);
            }
            _stageEnemyDictionary.Add(stageId, stageEnemies);
            _groundEnemyDictionary.Add(stageId, groundEnemies);
        }

        //var groupStage = temp.GroupBy(x => x.stageName);
        //foreach(var stageList in groupStage)
        //{
        //    string stageName = stageList.Key;
        //    var groupRound = stageList.GroupBy(x => x.stageId);
        //    List<string> stageRoundList = new();
        //    foreach(var Round in groupRound)
        //    {
        //        stageRoundList.Add(Round.ElementAt(0).stageRound);
        //    }
        //    _stageDictionary.Add(stageName, stageRoundList);
        //}

        // StageDictionary 디버그
        //foreach(var a in StageDictionary)
        //{
        //    foreach(var b in a.Value)
        //    {
        //        Debug.Log($"{a.Key}, {b}");
        //    }
        //}
    }
}