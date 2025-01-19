using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : Singleton<UIStatus>
{
    [Header("MainStat")]
    [SerializeField] Text petLevel;
    [SerializeField] Text testCount;

    [SerializeField] Text mainVit;
    [SerializeField] Text mainAtk;
    [SerializeField] Text mainDef;
    [SerializeField] Text mainDex;

    [Header("BaseStat")]
    [SerializeField] Text baseVit;
    [SerializeField] Text baseAtk;
    [SerializeField] Text baseDef;
    [SerializeField] Text baseDex;

    [SerializeField] Text vitCoeRank;
    [SerializeField] Text atkCoeRank;
    [SerializeField] Text defCoeRank;
    [SerializeField] Text dexCoeRank;

    [Header("CoeStat")]
    [SerializeField] Text vitCoe;
    [SerializeField] Text atkCoe;
    [SerializeField] Text defCoe;
    [SerializeField] Text dexCoe;

    [SerializeField] Text vitCoeGrade;
    [SerializeField] Text atkCoeGrade;
    [SerializeField] Text defCoeGrade;
    [SerializeField] Text dexCoeGrade;

    [SerializeField] Text vitCoeS;
    [SerializeField] Text atkCoeS;
    [SerializeField] Text defCoeS;
    [SerializeField] Text dexCoeS;

    PetInfo petInfo;


    public void TestCount(int count)
    {
        testCount.text = count.ToString();
    }

    public void SetUp(PetInfo petInfo)
    {
        this.petInfo = petInfo;
        SetStat();
    }

    public void SetStat()
    {
        petLevel.text = $"Lv.{petInfo.petLevel.ToString()}";

        mainVit.text = petInfo.ViewVit.ToString();
        mainAtk.text = petInfo.ViewAtk.ToString();
        mainDef.text = petInfo.ViewDef.ToString();
        mainDex.text = petInfo.ViewDex.ToString();

        baseVit.text = petInfo.ViewBaseVit.ToString();
        baseAtk.text = petInfo.ViewBaseAtk.ToString();
        baseDef.text = petInfo.ViewBaseDef.ToString();
        baseDex.text = petInfo.ViewBaseDex.ToString();

        vitCoeRank.text = petInfo.addVit.ToString();
        atkCoeRank.text = petInfo.addAtk.ToString();
        defCoeRank.text = petInfo.addDef.ToString();
        dexCoeRank.text = petInfo.addDex.ToString();

        vitCoe.text = petInfo.ViewVitGrow.ToString();
        atkCoe.text = petInfo.ViewAtkGrow.ToString();
        defCoe.text = petInfo.ViewDefGrow.ToString();
        dexCoe.text = petInfo.ViewDexGrow.ToString();

        vitCoeGrade.text = petInfo.ViewGradeVit;
        atkCoeGrade.text = petInfo.ViewGradeAtk;
        defCoeGrade.text = petInfo.ViewGradeDef;
        dexCoeGrade.text = petInfo.ViewGradeDex;

        vitCoeS.text = petInfo.petSrankVitCoe.ToString();
        atkCoeS.text = petInfo.petSrankAtkCoe.ToString();
        defCoeS.text = petInfo.petSrankDefCoe.ToString();
        dexCoeS.text = petInfo.petSrankDexCoe.ToString();
    }
}