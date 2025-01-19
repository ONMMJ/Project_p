// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

public class PetDefault
{
    public int petNo { get; private set; }       //펫 번호
    public string petName { get; private set; }  //펫 이름
    public string petAssetName { get; private set; }

    public float petBaseCoe { get; private set; }    //초기치 계수
    public float petVitCoe { get; private set; }     //체력 계수 
    public float petAtkCoe { get; private set; }     //공격 계수 
    public float petDefCoe { get; private set; }     //방어 계수  
    public float petDexCoe { get; private set; }     //순발 계수

    public int skillMaxPoint { get; private set; }   //스킬포인트 최대치
    public int petGrade { get; private set; }

    public int[] elementStat { get; private set; }

    public string[] skillsName { get; private set; }
    public ATTACK_RANGE?[] skillsRange { get; private set; }

    public PetDefault(JsonData json)
    {
        petNo = int.Parse(json["petNo"].ToString());
        petName = json["petName"].ToString();
        petAssetName = json["petAssetName"].ToString();
        petBaseCoe = float.Parse(json["petBaseCoe"].ToString());
        petVitCoe = float.Parse(json["petVitCoe"].ToString());
        petAtkCoe = float.Parse(json["petAtkCoe"].ToString());
        petDefCoe = float.Parse(json["petDefCoe"].ToString());
        petDexCoe = float.Parse(json["petDexCoe"].ToString());
        skillMaxPoint = int.Parse(json["skillMaxPoint"].ToString());
        petGrade = int.Parse(json["petGrade"].ToString());

        elementStat = new int[6];
        elementStat[0] = int.Parse(json["Earth"].ToString());
        elementStat[1] = int.Parse(json["Water"].ToString());
        elementStat[2] = int.Parse(json["Fire"].ToString());
        elementStat[3] = int.Parse(json["Wind"].ToString());
        elementStat[4] = int.Parse(json["Light"].ToString());
        elementStat[5] = int.Parse(json["Dark"].ToString());

        skillsName = new string[3];
        skillsName[0] = json["skill_1"].ToString();
        skillsName[1] = json["skill_2"].ToString();
        skillsName[2] = json["skill_3"].ToString();

        skillsRange = new ATTACK_RANGE?[3];
        ATTACK_RANGE temp;
        skillsRange[0] = Enum.TryParse<ATTACK_RANGE>(json["skillRange_1"].ToString(), true, out temp) ? temp : null;
        skillsRange[1] = Enum.TryParse<ATTACK_RANGE>(json["skillRange_2"].ToString(), true, out temp) ? temp : null;
        skillsRange[2] = Enum.TryParse<ATTACK_RANGE>(json["skillRange_3"].ToString(), true, out temp) ? temp : null;
    }
}

public class BackendPetChart : Chart
{

    // 각 차트의 row 정보를 담는 Dictionary
    private readonly Dictionary<string, PetDefault> _dictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    public IReadOnlyDictionary<string, PetDefault> Dictionary => (IReadOnlyDictionary<string, PetDefault>)_dictionary.AsReadOnlyCollection();

    // 이미지 캐싱을 관리하는 Dictionary
    private readonly Dictionary<string, Sprite> _petIcon = new();
    public IReadOnlyDictionary<string, Sprite> PetIcon => (IReadOnlyDictionary<string, Sprite>)_petIcon.AsReadOnlyCollection();

    // 이미지 캐싱을 관리하는 Dictionary
    private readonly Dictionary<int, string> _petName = new();
    public IReadOnlyDictionary<int, string> PetName => (IReadOnlyDictionary<int, string>)_petName.AsReadOnlyCollection();

    // 차트 파일 이름 설정 함수
    // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
    public override string GetChartFileName()
    {
        return "PET_CHART";
    }

    // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
    // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고해주세요
    protected override void LoadChartDataTemplate(JsonData json)
    {
        foreach (JsonData eachItem in json)
        {
            PetDefault info = new PetDefault(eachItem);

            _dictionary.Add(info.petName, info);
            _petName.Add(info.petNo, info.petName);
            base.AddOrGetImageDictionary(_petIcon, "PetIcons/", info.petAssetName);
        }
    }

    public Sprite GetSpriteIcon(string petName)
    {
        Sprite sprite;
        string petAssetName = Dictionary[petName].petAssetName;
        if (PetIcon.ContainsKey(petAssetName))
            sprite = PetIcon[petAssetName];
        else
            sprite = Resources.Load<Sprite>("PetIcons/Null");
        return sprite;
    }

    public string GetPetName(int petNo)
    {
        return PetName[petNo];
    }
    public int GetPetGrade(string petName)
    {
        return Dictionary[petName].petGrade;
    }
}