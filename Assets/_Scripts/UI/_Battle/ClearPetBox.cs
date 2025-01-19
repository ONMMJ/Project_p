using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClearPetBox : MonoBehaviour
{
    [SerializeField] Image petIcon;
    [SerializeField] Image ExpBarNow;
    [SerializeField] TMP_Text ExpText;
    [SerializeField] TMP_Text LevelUpText;

    public void SetUp(Pet pet)
    {
        string petAssetName = pet.petInfo.petDefault.petAssetName;
        if (BackendManager.Instance.Chart.PetChart.PetIcon.ContainsKey(petAssetName))
            petIcon.sprite = BackendManager.Instance.Chart.PetChart.PetIcon[petAssetName];
        else
            petIcon.sprite = Resources.Load<Sprite>("PetIcons/Null");
        gameObject.SetActive(true);

        StartCoroutine(FillExp(pet));
    }
    private IEnumerator FillExp(Pet pet)
    {
        float addExp = 0f;
        int levelUp = 0;
        int prevNowExp = pet.prevNowExp;
        int prevNextExp = pet.petInfo.GetNextExp(pet.prevLevel);
        int sumNextExp = 0;
        while (addExp<pet.receivedExp-0.1f)
        {
            addExp = Mathf.Lerp(addExp, pet.receivedExp, Time.deltaTime * 2f);
            float nowExp = prevNowExp + addExp - sumNextExp;
            if (nowExp >= prevNextExp)
            {
                levelUp++;
                sumNextExp += prevNextExp;
                prevNextExp = pet.petInfo.GetNextExp(pet.prevLevel + levelUp);
                LevelUp(levelUp);
            }
            ExpBarNow.fillAmount = nowExp / (float)prevNextExp;
            ExpText.text = pet.petInfo.ExpToString() + $"(+{(int)addExp})";
            yield return null;
        }
        ExpBarNow.fillAmount = pet.petInfo.petNowExp / (float)pet.petInfo.petNextExp;
        ExpText.text = pet.petInfo.ExpToString() + $"(+{pet.receivedExp})";
        LevelUp(pet.petInfo.petLevel - pet.prevLevel);
        Debug.Log("////////////////탈출");
    }

    private void LevelUp(int level)
    {
        if (level <= 0)
        {
            LevelUpText.gameObject.SetActive(false);
            return;
        }

        LevelUpText.text = $"Lv{level} UP!";
        LevelUpText.gameObject.SetActive(true);
    }
}
