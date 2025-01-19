using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] Text levelText;
    [SerializeField] Image hpImage;
    [SerializeField] Image[] skillPointImages;
    [SerializeField] Animation levelUpAnim;
    [SerializeField] List<Image> elementStatImages;

    public void SetPosition(Vector3 position)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        transform.localPosition = screenPosition;
    }
    public void UpdateSkillPoint(int skillPoint)
    {
        int point = skillPoint;
        if (skillPoint < 0)
            point = 0;
        if (skillPoint > skillPointImages.Length)
            skillPoint = skillPointImages.Length;

        for (int i = 0; i < point; i++)
        {
            skillPointImages[i].gameObject.SetActive(true);
        }

        for (int i = point; i < skillPointImages.Length; i++)
        {
            skillPointImages[i].gameObject.SetActive(false);
        }
    }
    public void SetHp(float hp, float maxHp)
    {
        hpImage.fillAmount = hp / maxHp;
    }
    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }
    public void PlayLevelUpAnim()
    {
        levelUpAnim.Play();
    }
    public void SetElementStat(int[] elementStat)
    {
        for(int i = 0; i < elementStat.Length; i++)
        {
            if(elementStat[i]>0)
                elementStatImages[i].gameObject.SetActive(true);
            else
                elementStatImages[i].gameObject.SetActive(false);
        }
    }
}
