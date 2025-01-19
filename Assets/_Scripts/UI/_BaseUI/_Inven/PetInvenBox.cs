using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class PetInvenBox : MonoBehaviour
{
    public static PetInvenBox selectPetBox;
    public static bool isMultipleSales;
    [SerializeField] Image petIcon;
    [SerializeField] Image selectIcon;

    [Header("Sprite")]
    [SerializeField] Sprite selectImage;
    [SerializeField] Sprite multipleImage;
    public UserPetData userPetData { get; private set; }

    SELECT_TYPE selectType;

    enum SELECT_TYPE
    {
        NONE,
        SELECT,
        MULTIPLE,
    }

    public string PetName => userPetData.petName;
    public int PetGrade => BackendManager.Instance.Chart.PetChart.Dictionary[userPetData.petName].petGrade;
    public float PetLevel => userPetData.petLevel + (userPetData.petNowExp / (float)userPetData.petNextExp);
    public float PetTotalGrow
    {
        get
        {
            PetInfo petInfo = new(userPetData);
            float total = petInfo.ViewBaseVit;
            total += petInfo.ViewBaseAtk * 4;
            total += petInfo.ViewBaseDef * 4;
            total += petInfo.ViewBaseDex * 4;
            return total;
        }
    }

    private void Start()
    {
        // selectType이 변경되면 선택 아이콘 자동으로 변경
        this.UpdateAsObservable()
            .Select(x => selectType)
            .DistinctUntilChanged()
            .Subscribe(x => SetSelectIcon())
            .AddTo(this);
    }

    private void SetSelectIcon()
    {
        switch (selectType)
        {
            case SELECT_TYPE.NONE:
                selectIcon.enabled = false;
                break;
            case SELECT_TYPE.SELECT:
                selectIcon.enabled = true;
                selectIcon.sprite = selectImage;
                selectIcon.color = Color.yellow;
                break;
            case SELECT_TYPE.MULTIPLE:
                selectIcon.enabled = true;
                selectIcon.sprite = multipleImage;
                selectIcon.color = Color.cyan;
                break;
        }
    }

    public void SetUp(UserPetData userPetData)
    {
        this.userPetData = userPetData;
        string petAssetName = BackendManager.Instance.Chart.PetChart.Dictionary[this.userPetData.petName].petAssetName;
        if (BackendManager.Instance.Chart.PetChart.PetIcon.ContainsKey(petAssetName))
            petIcon.sprite = BackendManager.Instance.Chart.PetChart.PetIcon[petAssetName];
        else
            petIcon.sprite = Resources.Load<Sprite>("PetIcons/Null");
        selectType = SELECT_TYPE.NONE;
    }

    public void OnClick()
    {
        if (isMultipleSales)
        {
            selectType = SELECT_TYPE.MULTIPLE;
        }
        else
        {
            Select();
        }
    }

    public void Sell()
    {
        if ((int)selectType > 0)
        {
            BackendManager.Instance.GameData.UserPetData.DeletePetData(userPetData.petId);
            if (selectPetBox.Equals(this))
                selectPetBox = null;
        }
    }
    public void Select()
    {
        if (selectPetBox != null)
            selectPetBox.Deselect();
        selectPetBox = this;
        selectType = SELECT_TYPE.SELECT;
    }

    public void ReSelect()
    {
        selectType = SELECT_TYPE.SELECT;
    }
    public void Deselect()
    {
        selectType = SELECT_TYPE.NONE;
    }
}