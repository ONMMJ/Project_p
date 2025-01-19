using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvas : SingletonReset<BattleCanvas>
{
    [SerializeField] Transform canvasPivot;
    [SerializeField] List<SkillToggle> skillToggles;
    [SerializeField] ClearPanel clearPanel;
    [SerializeField] BattleSettingUI battleSettingUI;

    Image targetArrow;
    List<Button> attackTargetImages;

    public Transform Pivot => canvasPivot;
    public ClearPanel ClearPanel => clearPanel;

    private void Start()
    {
        targetArrow = GameUI.Instance.GetTargetArrow(canvasPivot);

        attackTargetImages = new List<Button>();
        for(int i =  0; i< BattleController.Instance.TargetCount; i++)
        {
            attackTargetImages.Add(GameUI.Instance.GetAttackTargetImage(canvasPivot));
        }
    }

    public void UpdateSkill(List<BaseSkill> skills)
    {
        foreach (SkillToggle skill in skillToggles)
        {
            skill.Reset();
        }

        // 기본 공격은 무조건 활성화 상태
        skillToggles[0].SetActive(true);

        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] != null)
                skillToggles[i + 1].SetSkill(skills[i]);
        }
    }
    public void GameEnd(string clearText)
    {
        BattleController.Instance.StopCoroutines();
        battleSettingUI.GameEnd();
        clearPanel.gameObject.SetActive(true);
        clearPanel.SetUp(clearText);
    }
    public void RandomSelectSkill()
    {
        System.Random rand = new System.Random();
        SkillToggle randomSkill = skillToggles.Where(x => x.isActive).OrderBy(x => rand.Next()).First();
        //토글 선택//////////
    }

    public void SetTargetArrow(Vector3 position)
    {
        targetArrow.gameObject.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        targetArrow.transform.localPosition = screenPosition;
    }
    public void HideTargetArrow()
    {
        targetArrow.gameObject.SetActive(false);
    }

    public void SetAttackTargetImage(List<Pet> targetList)
    {
        HideAttackTargetImage();
        for (int i = 0; i < targetList.Count; i++)
        {
            attackTargetImages[i].gameObject.SetActive(true);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetList[i].transform.position);
            attackTargetImages[i].transform.localPosition = screenPosition;
            attackTargetImages[i].onClick.RemoveAllListeners();
            Pet target = targetList[i];
            attackTargetImages[i].onClick.AddListener(() => { BattleController.Instance.target = target; });
        }
    }
    public void HideAttackTargetImage()
    {
        foreach (Button image in attackTargetImages)
        {
            image.gameObject.SetActive(false);
        }
    }
}
