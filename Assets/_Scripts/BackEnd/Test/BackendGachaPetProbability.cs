using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class GachaPetInfo
{
    public int num { get; private set; }       // 뽑기 번호
    public string petName { get; private set; }  // 펫 이름
    public int grade { get; private set; }   // 펫 등급

    public GachaPetInfo(JsonData json)
    {
        num = int.Parse(json["elements"]["num"].ToString());
        petName = BackendManager.Instance.Chart.PetChart.GetPetName(num);
        grade = BackendManager.Instance.Chart.PetChart.GetPetGrade(petName);
    }
}

public class BackendGachaPetProbability : Probability<GachaPetInfo>
{
    public override string GetProbabilityFileName()
    {
        return "PET_GACHA";
    }

    // 펫 뽑기
    protected override GachaPetInfo LoadProbabilityDataTemplate(JsonData json)
    {
        GachaPetInfo info = new GachaPetInfo(json);
        Debug.Log(info.petName);
        return info;
    }
}
