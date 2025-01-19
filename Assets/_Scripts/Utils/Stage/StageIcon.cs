using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageIcon : MonoBehaviour
{
    [Header("StageInfo")]
    [SerializeField] string stageName;
    public string StageName => stageName;

    [Header("StageUI")]
    [SerializeField] TMPro.TMP_Text stageNameText;

    Button stageButton;
    public List<string> roundList { get; private set; }

    public delegate void SetStageRound();


    private void Awake()
    {
        stageButton = GetComponent<Button>();
        if (!stageName.Equals("Mission"))
        {
            roundList = BackendManager.Instance.Chart.StageChart.GetStageRoundList(StageName);
            stageNameText.text = BackendManager.Instance.Chart.StageChart.GetStageNameText(stageName);
        }
    }

    public void Setup(SetStageRound SetRound)
    {
        stageButton.onClick.AddListener(()=>SetRound()); 
    }
}
