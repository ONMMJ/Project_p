using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class StatusDetailUI : BaseUI
{
    [Header("PetLevel")]
    [SerializeField] Text petLevel;
    [SerializeField] Text expText;
    [SerializeField] Image expImage;

    [Header("BaseStat")]
    [SerializeField] Text baseVit;
    [SerializeField] Text baseAtk;
    [SerializeField] Text baseDef;
    [SerializeField] Text baseDex;

    [SerializeField] Text baseVitDiff;
    [SerializeField] Text baseAtkDiff;
    [SerializeField] Text baseDefDiff;
    [SerializeField] Text baseDexDiff;

    [Header("RealStat")]
    [SerializeField] Text realVit;
    [SerializeField] Text realAtk;
    [SerializeField] Text realDef;
    [SerializeField] Text realDex;

    [SerializeField] Text realVitDiff;
    [SerializeField] Text realAtkDiff;
    [SerializeField] Text realDefDiff;
    [SerializeField] Text realDexDiff;


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

    [Header("Button")]
    [SerializeField] Button ResetButton;
    [SerializeField] Text ResetButtonText;

    [Header("Skill")]
    [SerializeField] List<Image> skillImageList;

    PetInfo petInfo;

    protected override void EnableUI()
    {
        SetStat();
    }

    private void Start()
    {
        ResetButton.onClick.AddListener(ResetNormal);

        // 선택된 펫의 리셋 카운드가 변하면 함수 호출
        this.UpdateAsObservable()
            .Select(x => petInfo.resetCount)
            .DistinctUntilChanged()
            .Subscribe(x => { ResetButtonActive(); ResetButtonTextUpdate(); })
            .AddTo(this);
    }

    public void SetUp(PetInfo petInfo)
    {
        this.petInfo = petInfo;
    }

    private void ResetNormal()
    {
        petInfo.ResetNoraml();
    }
    private void ResetButtonActive()
    {
        if(petInfo.resetCount >= PetManager.Instance.maxResetCount)
        {
            ResetButton.enabled = false;
        }
        else
        {
            ResetButton.enabled = true;
        }
        SetStat();
    }
    private void ResetButtonTextUpdate()
    {
        ResetButtonText.text = $"Reset\n{petInfo.resetCount} / {PetManager.Instance.maxResetCount}";
    }

    public void SetStat()
    {
        if (petInfo == null)
            return;

        petLevel.text = petInfo.petLevel.ToString();
        expText.text = petInfo.ExpToString();
        expImage.fillAmount = petInfo.ExpPersent();

        baseVit.text = petInfo.ViewBaseVit.ToString();
        baseAtk.text = petInfo.ViewBaseAtk.ToString();
        baseDef.text = petInfo.ViewBaseDef.ToString();
        baseDex.text = petInfo.ViewBaseDex.ToString();

        baseVitDiff.text = petInfo.ViewBaseVitDiff.ToString();
        baseAtkDiff.text = petInfo.ViewBaseAtkDiff.ToString();
        baseDefDiff.text = petInfo.ViewBaseDefDiff.ToString();
        baseDexDiff.text = petInfo.ViewBaseDexDiff.ToString();

        realVit.text = petInfo.ViewVit.ToString();
        realAtk.text = petInfo.ViewAtk.ToString();
        realDef.text = petInfo.ViewDef.ToString();
        realDex.text = petInfo.ViewDex.ToString();

        realVitDiff.text = petInfo.ViewVitDiff.ToString();
        realAtkDiff.text = petInfo.ViewAtkDiff.ToString();
        realDefDiff.text = petInfo.ViewDefDiff.ToString();
        realDexDiff.text = petInfo.ViewDexDiff.ToString();

        vitCoe.text = petInfo.ViewVitGrow.ToString();
        atkCoe.text = petInfo.ViewAtkGrow.ToString();
        defCoe.text = petInfo.ViewDefGrow.ToString();
        dexCoe.text = petInfo.ViewDexGrow.ToString();

        vitCoeGrade.text = petInfo.ViewGradeVit;
        atkCoeGrade.text = petInfo.ViewGradeAtk;
        defCoeGrade.text = petInfo.ViewGradeDef;
        dexCoeGrade.text = petInfo.ViewGradeDex;

        vitCoeS.text = System.Math.Round(petInfo.petSrankVitCoe, 2).ToString();
        atkCoeS.text = System.Math.Round(petInfo.petSrankAtkCoe, 2).ToString();
        defCoeS.text = System.Math.Round(petInfo.petSrankDefCoe, 2).ToString();
        dexCoeS.text = System.Math.Round(petInfo.petSrankDexCoe, 2).ToString();

        foreach(Image skillImage in skillImageList)
        {
            skillImage.gameObject.SetActive(false);
        }
        for (int i = 0; i < petInfo.petDefault.skillsName.Length; i++)
        {
            if (!string.IsNullOrEmpty(petInfo.petDefault.skillsName[i]))
            {
                skillImageList[i].sprite = BackendManager.Instance.Chart.SkillChart.GetSpriteIcon(petInfo.petDefault.skillsName[i]);
                skillImageList[i].gameObject.SetActive(true);
            }
        }
        ResetButtonTextUpdate();
    }

}
