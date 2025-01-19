using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class InfoUI : MonoBehaviour
{
    [SerializeField] GameObject panel;

    [Header("Preview")]
    [SerializeField] Transform prefabSpawnPoint;
    GameObject previewObject;

    [Header("Status")]
    [SerializeField] Text petName;
    [SerializeField] Text petLevel;

    [SerializeField] Text baseVit;
    [SerializeField] Text baseAtk;
    [SerializeField] Text baseDef;
    [SerializeField] Text baseDex;

    [SerializeField] Text baseVitDiff;
    [SerializeField] Text baseAtkDiff;
    [SerializeField] Text baseDefDiff;
    [SerializeField] Text baseDexDiff;

    [SerializeField] Text realVit;
    [SerializeField] Text realAtk;
    [SerializeField] Text realDef;
    [SerializeField] Text realDex;

    [SerializeField] Text realVitDiff;
    [SerializeField] Text realAtkDiff;
    [SerializeField] Text realDefDiff;
    [SerializeField] Text realDexDiff;

    [SerializeField] Text vitCoeGrade;
    [SerializeField] Text atkCoeGrade;
    [SerializeField] Text defCoeGrade;
    [SerializeField] Text dexCoeGrade;

    [Header("Button")]
    [SerializeField] Button statusDetailButton;
    [SerializeField] Button collectionButton;
    [SerializeField] Button marketButton;

    [Header("ConnectUI")]
    [SerializeField] StatusDetailUI statusDetailUI;

    PetInfo petInfo;

    private void Start()
    {
        // selectPet이 변경되면 자동으로 Info세팅
        this.UpdateAsObservable()
            .Select(x => PetInvenBox.selectPetBox)
            .DistinctUntilChanged()
            .Subscribe(x => SelectPet())
            .AddTo(this);
    }

    private void SetPreview()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = Instantiate(Resources.Load<GameObject>($"PetPrefabs/{petInfo.petDefault.petAssetName}"), prefabSpawnPoint);
        //SetLayer(previewObject.transform, 3);
        previewObject.transform.localPosition = Vector3.zero;
        previewObject.transform.localScale = Vector3.one * 135;
    }

    //private void SetLayer(Transform obj, int layer)
    //{
    //    obj.gameObject.layer = layer;
    //    foreach (Transform child in previewObject.transform)
    //    {
    //        SetLayer(child, layer);
    //    }
    //}

    private void SelectPet()
    {
        if (PetInvenBox.selectPetBox == null)
            Reset();
        else
        {
            panel.SetActive(true);
            SetStat();
            SetPreview();
        }
    }

    private void SetStat()
    {
        petInfo = new(PetInvenBox.selectPetBox.userPetData);
        petName.text = petInfo.petDefault.petName.ToString();
        petLevel.text = petInfo.petLevel.ToString();

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

        vitCoeGrade.text = petInfo.ViewGradeVit;
        atkCoeGrade.text = petInfo.ViewGradeAtk;
        defCoeGrade.text = petInfo.ViewGradeDef;
        dexCoeGrade.text = petInfo.ViewGradeDex;
    }

    public void Reset()
    {
        petInfo = null;
        Destroy(previewObject);
        petName.text = "-";
        petLevel.text = "-";

        baseVit.text = "-";
        baseAtk.text = "-";
        baseDef.text = "-";
        baseDex.text = "-";

        baseVitDiff.text = "-";
        baseAtkDiff.text = "-";
        baseDefDiff.text = "-";
        baseDexDiff.text = "-";

        realVit.text = "-";
        realAtk.text = "-";
        realDef.text = "-";
        realDex.text = "-";

        realVitDiff.text = "-";
        realAtkDiff.text = "-";
        realDefDiff.text = "-";
        realDexDiff.text = "-";

        vitCoeGrade.text = "-";
        atkCoeGrade.text = "-";
        defCoeGrade.text = "-";
        dexCoeGrade.text = "-";
    }

    public void OnClickStatusDetailButton()
    {
        if (petInfo == null)
            return;

        statusDetailUI.SetUp(petInfo);
        statusDetailUI.gameObject.SetActive(true);
    }
}