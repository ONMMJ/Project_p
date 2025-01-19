using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSettingUI : MonoBehaviour
{
    [Header("AutoUI")]
    [SerializeField] Toggle autoToggle;
    [SerializeField] Toggle jointAttackToggle;

    [Header("SpeedUI")]
    [SerializeField] Button speedUpButton;
    [SerializeField] TMP_Text speedText;

    [Header("SpeedText")]
    [SerializeField] string normalSpeed;
    [SerializeField] string doubleSpeed;
    [SerializeField] string tripleSpeed;

    bool isAuto
    {
        get
        {
            return BattleController.Instance.isAuto;
        }
        set
        {
            BattleController.Instance.isAuto = value;
        }
    }
    bool isJointAttack
    {
        get
        {
            return BattleController.Instance.isJointAttack;
        }
        set
        {
            BattleController.Instance.isJointAttack = value;
        }
    }

    private void Start()
    {
        autoToggle.isOn = false;
        jointAttackToggle.isOn = false;
        autoToggle.onValueChanged.AddListener(delegate { OnToggleAuto(); });
        jointAttackToggle.onValueChanged.AddListener(delegate { OnToggleJointAttack(); });
        speedUpButton.onClick.AddListener(OnClickSpeedUpButton);
    }

    private void OnToggleAuto()
    {
        isAuto = autoToggle.isOn;
    }
    private void OnToggleJointAttack()
    {
        isJointAttack = jointAttackToggle.isOn;
    }
    private void OnClickSpeedUpButton()
    {
        if (GameManager.Instance.speedGrade >= 3)
        {
            GameManager.Instance.speedGrade = 1;
        }
        else
        {
            GameManager.Instance.speedGrade++;
        }

        switch (GameManager.Instance.speedGrade)
        {
            case 1:
                speedText.text = normalSpeed;
                break;
            case 2:
                speedText.text = doubleSpeed;
                break;
            case 3:
                speedText.text = tripleSpeed;
                break;
        }
    }
    public void GameEnd()
    {
        isAuto = false;
        autoToggle.isOn = false;
    }
}
