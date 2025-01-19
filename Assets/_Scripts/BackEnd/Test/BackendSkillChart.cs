using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

public enum ATTACK_RANGE
{
    MELEE,
    RANGED,
    BUFF,
}

public class SkillInfo
{
    public string skillName;
    public int skillLevel; 
    public Sprite icon;

    public int skillUsePoint;
    public int skillTurnCount;
    public ATTACK_TYPE attackType;

    // Active Skill
    public float mainDamage;
    public float subDamage;

    // Buff Skill
    public float buffVit;
    public float buffAtk;
    public float buffDef;
    public float buffDex;

    public string skillManual;

    public SkillInfo(JsonData json)
    {
        skillName = json["skillName"].ToString();
        skillLevel = int.Parse(json["skillLevel"].ToString());

        skillUsePoint = int.Parse(json["skillUsePoint"].ToString());
        skillTurnCount = int.Parse(json["skillTurnCount"].ToString());
        attackType = (ATTACK_TYPE)Enum.Parse(typeof(ATTACK_TYPE), json["attackType"].ToString());

        float.TryParse(json["mainDamage"].ToString(), out mainDamage);
        float.TryParse(json["subDamage"].ToString(), out subDamage);

        float.TryParse(json["buffVit"].ToString(), out buffVit);
        float.TryParse(json["buffAtk"].ToString(), out buffAtk);
        float.TryParse(json["buffDef"].ToString(), out buffDef);
        float.TryParse(json["buffDex"].ToString(), out buffDex);

        skillManual= json["skillManual"].ToString();

        Debug.Log(buffVit);
    }

    public void SetIcon(Sprite icon)
    {
        this.icon = icon;
    }
}

public class BackendSkillChart : Chart
{
    // 각 차트의 row 정보를 담는 Dictionary
    private readonly Dictionary<string, SkillInfo> _dictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    public IReadOnlyDictionary<string, SkillInfo> Dictionary => (IReadOnlyDictionary<string, SkillInfo>)_dictionary.AsReadOnlyCollection();

    // 이미지 캐싱을 관리하는 Dictionary
    private readonly Dictionary<string, Sprite> _skillIcon = new();
    public IReadOnlyDictionary<string, Sprite> SkillIcon => (IReadOnlyDictionary<string, Sprite>)_skillIcon.AsReadOnlyCollection();

    // 차트 파일 이름 설정 함수
    // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
    public override string GetChartFileName()
    {
        return "SKILL_CHART";
    }

    // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
    // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고해주세요
    protected override void LoadChartDataTemplate(JsonData json)
    {
        foreach (JsonData eachItem in json)
        {
            SkillInfo info = new SkillInfo(eachItem);

            Sprite icon = base.AddOrGetImageDictionary(_skillIcon, "SkillIcons/", info.skillName);
            info.SetIcon(icon);

            string key = $"{info.skillName}{info.skillLevel}";
            _dictionary.Add(key, info);
        }
    }

    public Sprite GetSpriteIcon(string skillName)
    {
        Sprite sprite;
        if (SkillIcon.ContainsKey(skillName))
            sprite = SkillIcon[skillName];
        else
            sprite = Resources.Load<Sprite>("Null");
        return sprite;
    }
}