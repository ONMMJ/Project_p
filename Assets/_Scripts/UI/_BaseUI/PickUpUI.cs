using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpUI : BaseUI
{
    [SerializeField] Image petIcon;
    [SerializeField] List<GameObject> starIconList;
    [SerializeField] Button pickUpButton;

    private void Start()
    {
        pickUpButton.onClick.AddListener(GachaPet);
    }
    protected override void EnableUI()
    {
        OffIcon();
    }

    private void GachaPet()
    {
        const int gold = 100;
        if (BackendManager.Instance.GameData.UserData.gold < gold)
            return;

        BackendManager.Instance.Probability.GachaPetProbability.BackendProbabilityDataLoad(
            gachaPetInfo => {
                if (!BackendManager.Instance.GameData.UserData.UseGold(gold))
                    return;
                PetInfo petInfo = PetManager.Instance.GetLv1Pet(gachaPetInfo.petName);
                BackendManager.Instance.GameData.UserPetData.AddPetData(petInfo);

                int grade = gachaPetInfo.grade;
                petIcon.sprite = petInfo.petIcon; ;
                OffIcon();
                StartCoroutine(SetIcon(grade));
            });
    }

    IEnumerator SetIcon(int grade)
    {
        petIcon.gameObject.SetActive(true);
        for (int i = 0; i < grade; i++)
        {
            starIconList[i].SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OffIcon()
    {
        petIcon.gameObject.SetActive(false);
        foreach(GameObject icon in starIconList)
        {
            icon.SetActive(false);
        }
    }
}
