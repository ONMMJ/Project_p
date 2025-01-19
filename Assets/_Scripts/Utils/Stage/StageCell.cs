using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class StageCell : MonoBehaviour
{
    [Header("StageUI")]
    [SerializeField] Button stageButton;
    [SerializeField] TMPro.TMP_Text stageRoundText;

    MissionData missionData;
    string stageName;
    string stageRound;
    string stageFullName;

    public string stageId => $"{stageName}{stageRound}";

    private void Start()
    {
        stageButton.onClick.AddListener(OnClickStageButton);
    }

    private void OnClickStageButton()
    {
        GameManager.Instance.stageFullName = stageFullName;
        if (stageName.Equals("Mission"))
        {
            GameManager.Instance.isMission = true;
            GameManager.Instance.missionData = missionData;
            UIManager.Instance.OpenBattleStartUI("Mission");
        }
        else
        {
            GameManager.Instance.isMission = false;
            GameManager.Instance.stageId = stageId;
            UIManager.Instance.OpenBattleStartUI(stageId);
        }

    }

    public void Setup(string stageName, string stageRound)
    {
        this.stageName = stageName;
        this.stageRound = stageRound;
        string stageId = stageName + stageRound;
        stageFullName = BackendManager.Instance.Chart.StageChart.GetStageRoundText(stageId);
        stageRoundText.text = stageFullName;
        gameObject.SetActive(true);
    }
    public void Setup(int stageRound, MissionData missionData)
    {
        stageName = "Mission";
        stageFullName = $"Mission {stageRound}";
        stageRoundText.text = stageFullName;
        this.missionData = missionData;
        gameObject.SetActive(true);
    }
    public void Reset()
    {
        stageName = null;
        stageRound = null;
        gameObject.SetActive(false);
    }
}
