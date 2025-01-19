using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PetInvenUI : BaseUI
{
    [Header("Prefabs")]
    [SerializeField] PetInvenBox petInvenBoxPrefab;

    [Header("UI")]
    [SerializeField] Transform content;
    [SerializeField] Button multipleSelectButton;
    [SerializeField] Button saleButton;
    [SerializeField] TMP_Text Grade_DButtonText;

    [Header("FilterButton")]
    [SerializeField] FilterButton filterButtonGrade; // 등급별
    [SerializeField] FilterButton filterButtonLevel; // 레벨별
    [SerializeField] FilterButton filterButtonGrow; // 성장률
    [SerializeField] Button filterButtonGrade_D; // 각 등급별
    [SerializeField] Button filterButtonPet;      // 한가지 펫만 분류
    [SerializeField] Button filterResetButton;

    [Header("FilterText")]
    [SerializeField] TMP_InputField filterText;     // 입력된 텍스트로 분류
    //[SerializeField] Button filterTextButton;

    [Header("ButtonSprite")]
    [SerializeField] Sprite SelectButtonImage;
    [SerializeField] Sprite DeSelectButtonImage;

    List<PetInvenBox> boxes = new List<PetInvenBox>();

    int filterPetGrage = 0;
    FILTER_TYPE nowFilterType;

    enum FILTER_TYPE
    {
        DEFAULT,
        GRADE,
        LEVEL,
        GROW,
    }

    bool isMultipleSales
    {
        get
        {
            return PetInvenBox.isMultipleSales;
        }
        set
        {
            PetInvenBox.isMultipleSales = value;
        }
    }
    PetInvenBox selectPetBox
    {
        get
        {
            return PetInvenBox.selectPetBox;
        }
        set
        {
            PetInvenBox.selectPetBox = value;
        }
    }
    private void Awake()
    {
        SetButtonListener();
        nowFilterType = FILTER_TYPE.DEFAULT;
    }
    private void BoxSort(List<PetInvenBox> boxes)
    {
        if (boxes == null)
            return;
        foreach(PetInvenBox box in boxes)
            box.transform.SetAsFirstSibling();
    }

    protected override void EnableUI()
    {
        SetContent();
        isMultipleSales = false;
    }

    private void Reset()
    {
        this.filterText.text = "";
        Grade_DButtonText.text = "ALL";
        selectPetBox = null;
        nowFilterType = FILTER_TYPE.DEFAULT;
        EnableUI();
    }

    public void SetContent()
    {
        if (BackendManager.Instance.GameData.UserPetData.Dictionary == null)
            return;
        if (boxes.Count > 0)
        {
            foreach (PetInvenBox box in boxes)
            {
                Destroy(box.gameObject);
            }
        }
        boxes = new List<PetInvenBox>();
        foreach (UserPetData userPetData in BackendManager.Instance.GameData.UserPetData.Dictionary.Values)
        {
            PetInvenBox petBox = GetInvenBox(userPetData);
            if(selectPetBox != null)
            {
                if (selectPetBox.userPetData.petId == petBox.userPetData.petId)
                    petBox.Select();
            }
            boxes.Add(petBox);
        }
        InvenBoxFilter(nowFilterType, true);
        TextFilter(this.filterText.text);
    }
    private PetInvenBox GetInvenBox(UserPetData userPetData)
    {
        PetInvenBox petInvenBox = Instantiate(petInvenBoxPrefab, content);
        petInvenBox.SetUp(userPetData);
        return petInvenBox;
    }

    // ====================================================================================================
    #region 필터 함수
    private void InvenBoxFilter(FILTER_TYPE filterType, bool isOpenUI)
    {
        SetButtonSpriteReset();
        switch (filterType)
        {
            case FILTER_TYPE.DEFAULT:
                filterButtonGrade.Reset();
                filterButtonLevel.Reset();
                filterButtonGrow.Reset();
                GradeFilter(false);
                break;
            case FILTER_TYPE.GRADE:
                filterButtonGrade.GetComponent<Image>().sprite = SelectButtonImage;
                GradeFilter(isOpenUI);
                filterButtonLevel.Reset();
                filterButtonGrow.Reset();
                break;
            case FILTER_TYPE.LEVEL:
                filterButtonLevel.GetComponent<Image>().sprite = SelectButtonImage;
                LevelFilter(isOpenUI);
                filterButtonGrade.Reset();
                filterButtonGrow.Reset();
                break;
            case FILTER_TYPE.GROW:
                filterButtonGrow.GetComponent<Image>().sprite = SelectButtonImage;
                GrowFilter(isOpenUI);
                filterButtonGrade.Reset();
                filterButtonLevel.Reset();
                break;
        }
    }
    private void GradeFilter(bool isOpenUI)
    {
        FILTER state;
        if (isOpenUI)
            state = filterButtonGrade.NowState();
        else
            state = filterButtonGrade.NextState();

        List<PetInvenBox> filterBoxes = null;
        switch (state)
        {
            case FILTER.DEFAULT:
                break;
            case FILTER.ASC:
                filterBoxes = boxes.OrderBy(x => x.PetGrade).ThenBy(x => x.PetName).ToList();
                break;
            case FILTER.DESC:
                filterBoxes = boxes.OrderByDescending(x => x.PetGrade).ThenBy(x => x.PetName).ToList();
                break;
        }
        BoxSort(filterBoxes);
    }
    private void LevelFilter(bool isOpenUI)
    {
        FILTER state;
        if (isOpenUI)
            state = filterButtonLevel.NowState();
        else
            state = filterButtonLevel.NextState();

        List<PetInvenBox> filterBoxes = null;
        switch (state)
        {
            case FILTER.DEFAULT:
                break;
            case FILTER.ASC:
                filterBoxes = boxes.OrderBy(x => x.PetLevel).ThenBy(x => x.PetName).ToList();
                break;
            case FILTER.DESC:
                filterBoxes = boxes.OrderByDescending(x => x.PetLevel).ThenBy(x => x.PetName).ToList();
                break;
        }
        BoxSort(filterBoxes);
    }
    private void GrowFilter(bool isOpenUI)
    {
        FILTER state;
        if (isOpenUI)
            state = filterButtonGrow.NowState();
        else
            state = filterButtonGrow.NextState();

        List<PetInvenBox> filterBoxes = null;
        switch (state)
        {
            case FILTER.DEFAULT:
                break;
            case FILTER.ASC:
                filterBoxes = boxes.OrderBy(x => x.PetTotalGrow).ThenBy(x => x.PetName).ToList();
                break;
            case FILTER.DESC:
                filterBoxes = boxes.OrderByDescending(x => x.PetTotalGrow).ThenBy(x => x.PetName).ToList();
                break;
        }
        BoxSort(filterBoxes);
    }
    private void PetFilter()
    {
        if (PetInvenBox.selectPetBox == null)
            return;

        foreach (PetInvenBox box in boxes)
        {
            box.gameObject.SetActive(false);
        }
        var filterBoxes = boxes.Where(x => x.PetName.Equals(PetInvenBox.selectPetBox.PetName));
        foreach (PetInvenBox box in filterBoxes)
        {
            box.gameObject.SetActive(true);
        }
    }
    private void TextFilter(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            foreach (PetInvenBox box in boxes)
            {
                box.gameObject.SetActive(true);
            }
            return;
        }
        foreach (PetInvenBox box in boxes)
        {
            box.gameObject.SetActive(false);
        }
        var filterBoxes = boxes.Where(x => x.PetName.Contains(text));
        foreach (PetInvenBox box in filterBoxes)
        {
            box.gameObject.SetActive(true);
        }
    }
    private void SetButtonSpriteReset()
    {
        filterButtonGrade_D.image.sprite = DeSelectButtonImage;
        filterButtonGrade.GetComponent<Image>().sprite = DeSelectButtonImage;
        filterButtonGrow.GetComponent<Image>().sprite = DeSelectButtonImage;
        filterButtonLevel.GetComponent<Image>().sprite = DeSelectButtonImage;
    }
    #endregion

    // ====================================================================================================
    #region 버튼 함수
    public void OnClickMultipleSelectButton()
    {
        isMultipleSales = !isMultipleSales;
        if (!isMultipleSales)
        {
            foreach (PetInvenBox box in boxes)
            {
                box.Deselect();
            }
            if (selectPetBox != null)
                selectPetBox.ReSelect();
        }
        else
        {
            selectPetBox.Deselect();
        }
    }
    public void OnClickSaleButton()
    {
        //if (!isMultipleSales)
        //    return;

        foreach (PetInvenBox box in boxes)
        {
            box.Sell();
        }

        SetContent();
    }
    #endregion

    // ====================================================================================================

    // 버튼 리스너 적용
    public void SetButtonListener()
    {
        // 각 등급 필터
        filterButtonGrade_D.onClick.AddListener(() =>
        {
            filterPetGrage = (filterPetGrage + 1) % 5;
            if (filterPetGrage == 0)
            {
                foreach (PetInvenBox box in boxes)
                {
                    box.gameObject.SetActive(true);
                }
                Grade_DButtonText.text = "ALL";
            }
            else
            {
                foreach (PetInvenBox box in boxes)
                {
                    box.gameObject.SetActive(false);
                }
                var filterBoxes = boxes.Where(x => x.PetGrade == filterPetGrage);
                foreach (PetInvenBox box in filterBoxes)
                {
                    box.gameObject.SetActive(true);
                }
                Grade_DButtonText.text = new string('O', filterPetGrage);
            }
            SetButtonSpriteReset();
            filterButtonGrade_D.image.sprite = SelectButtonImage;
        });
        // 필터 버튼 리스너
        filterButtonGrade.Button.onClick.AddListener(() => { InvenBoxFilter(FILTER_TYPE.GRADE, false); nowFilterType = FILTER_TYPE.GRADE; });      // 등급별 필터
        filterButtonLevel.Button.onClick.AddListener(() => { InvenBoxFilter(FILTER_TYPE.LEVEL, false); nowFilterType = FILTER_TYPE.LEVEL; });      // 레벨별 필터
        filterButtonGrow.Button.onClick.AddListener(() => { InvenBoxFilter(FILTER_TYPE.GROW, false); nowFilterType = FILTER_TYPE.GROW; });         // 성장률 필터
        filterButtonPet.onClick.AddListener(() => { PetFilter(); });                                                                        // 펫 필터
        filterText.onSubmit.AddListener((filter) => { TextFilter(this.filterText.text); });                                                 // 텍스트 필터
        //filterTextButton.onClick.AddListener(() => { TextFilter(this.filterText.text); });                                                // 텍스트 필터 적용 버튼


        // 기타 버튼 리스너
        filterResetButton.onClick.AddListener(Reset);   // 리셋 버튼
        multipleSelectButton.onClick.AddListener(OnClickMultipleSelectButton);  // 다중선택 버튼
        saleButton.onClick.AddListener(OnClickSaleButton);  // 판매 버튼
    }
}