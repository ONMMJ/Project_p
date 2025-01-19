using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class MainPetButton : MonoBehaviour
{
    //static MainPetButton[] MainPetButtonList = new MainPetButton[3];

    [SerializeField] int index;
    [SerializeField] bool isBattleStartUI;
    [SerializeField] Image icon;
    [SerializeField] Button button;

    private void OnEnable()
    {
        if (GameManager.Instance.isMission)
            SetMission();
        else
            SetIcon();
    }
    private void Start()
    {
        button.onClick.AddListener(SetMainPet);

        if (GameManager.Instance.isMission)
            SetMission();
        else
            SetIcon();

        this.UpdateAsObservable()
            .Select(x => BackendManager.Instance.GameData.UserData.selectPets[index])
            .DistinctUntilChanged()
            .Subscribe(x => { SetIcon(); })
            .AddTo(this);
    }

    private void SetMission()
    {
        Debug.Log(GameManager.Instance.missionData.MissionPetList[index].petName);
        icon.sprite = BackendManager.Instance.Chart.PetChart.GetSpriteIcon(GameManager.Instance.missionData.MissionPetList[index].petName);
    }

    private void SetIcon()
    {
        //int idx = 0;
        //foreach (string petId in BackendManager.Instance.GameData.UserData.selectPets)
        //{
        //    Debug.Log(petId);
        //    if (petId == "")
        //    {
        //        MainPetButtonList[idx].icon.sprite = Resources.Load<Sprite>("PetIcons/Null");
        //        idx++;
        //        continue;
        //    }
        //    string petName = BackendManager.Instance.GameData.UserPetData.Dictionary[petId].petName;
        //    string petAssetName = BackendManager.Instance.Chart.PetChart.Dictionary[petName].petAssetName;
        //    if (BackendManager.Instance.Chart.PetChart.PetIcon.ContainsKey(petAssetName))
        //        MainPetButtonList[idx].icon.sprite = BackendManager.Instance.Chart.PetChart.PetIcon[petAssetName];
        //    else
        //        MainPetButtonList[idx].icon.sprite = Resources.Load<Sprite>("PetIcons/Null");
        //    idx++;
        //}
        if (GameManager.Instance.isMission)
            return;

        string petId = BackendManager.Instance.GameData.UserData.selectPets[index];
        if (petId == "")
        {
            icon.sprite = Resources.Load<Sprite>("PetIcons/Null");
            return;
        }
        string petName = BackendManager.Instance.GameData.UserPetData.Dictionary[petId].petName;
        string petAssetName = BackendManager.Instance.Chart.PetChart.Dictionary[petName].petAssetName;
        if (BackendManager.Instance.Chart.PetChart.PetIcon.ContainsKey(petAssetName))
            icon.sprite = BackendManager.Instance.Chart.PetChart.PetIcon[petAssetName];
        else
            icon.sprite = Resources.Load<Sprite>("PetIcons/Null");
    }


    private void SetMainPet()
    {
        if (GameManager.Instance.isMission)
            return;


        if(isBattleStartUI)
        {
            UIManager.Instance.OpenInvenUI(LEFT_BUTTON_PANEL.Main);
            return;
        }


        if (PetInvenBox.selectPetBox == null)
            return;

        UserPetData petData = PetInvenBox.selectPetBox.userPetData;
        if (System.Array.Exists(BackendManager.Instance.GameData.UserData.selectPets, x => x == petData.petId))
        {
            int findIdx = System.Array.FindIndex(BackendManager.Instance.GameData.UserData.selectPets, x => x == petData.petId);
            BackendManager.Instance.GameData.UserData.SwapPet(index, findIdx);
        }
        else
        {
            BackendManager.Instance.GameData.UserData.selectPets[index] = petData.petId;
        }

        BackendManager.Instance.GameData.UserData.Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                SetIcon();
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
            }
            else
            {
                Debug.LogError($"대표캐릭터 설정 실패: {callback})");
            }
        });
    }
}
