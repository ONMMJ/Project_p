using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ClearPanel : MonoBehaviour
{
    [SerializeField] List<ClearPetBox> clearPetBoxes;
    [SerializeField] TMP_Text clearText;
    [SerializeField] Button backButton;

    public void SetUp(string clearText)
    {
        SetButtonClick();
        this.clearText.text = clearText;
        UIManager.Instance.HideUI(true);
        BattleController.Instance.EndBattle();
        List<Pet> pets = BattleController.Instance.Pets;

        OffBoxes();

        for (int i = 0; i < pets.Count; i++)
        {
            clearPetBoxes[i].SetUp(pets[i]);
        }
    }

    private void OffBoxes()
    {
        foreach (ClearPetBox box in clearPetBoxes)
        {
            box.gameObject.SetActive(false);
        }
    }

    private void SetButtonClick()
    {

        backButton.onClick.AddListener(() => { UIManager.Instance.HideUI(false); SceneManager.LoadScene("MainScene"); gameObject.SetActive(false); });
    }
}
