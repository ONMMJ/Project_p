using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillToggle : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] Image icon;
    [SerializeField] bool isFirst;

    BaseSkill skill;

    public bool isActive => toggle.interactable;

    private void Start()
    {
        toggle.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public void Reset()
    {
        if (isFirst)
            toggle.isOn = true;
        else
            toggle.isOn = false;

        skill = null;
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void SetSkill(BaseSkill skill)
    {
        Reset();
        this.skill = skill;
        icon.sprite = skill.Icon;
        toggle.interactable = skill.IsReady;
        SetActive(true);
    }
    void OnValueChanged()
    {
        if (toggle.isOn)
        {
            BattleController.Instance.skill = skill;
        }
    }

    public void ManualOn()
    {
        if(skill == null)
            GameUI.Instance.manualOn("기본 공격.");
        else
        GameUI.Instance.manualOn(skill.GetManual());
    }
    public void ManualOff()
    {
        GameUI.Instance.manualOff();
    }
}
