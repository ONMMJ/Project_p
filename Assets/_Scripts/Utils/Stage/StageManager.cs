using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseUI
{
    [SerializeField] List<StageIcon> stageIcons;

    [Header("StageCell")]
    [SerializeField] StageCell stageCellPrefab;
    [SerializeField] Transform content;

    [Header("Mission")]
    [SerializeField] List<MissionData> missionDataList;
    [SerializeField] StageIcon missionStageIcon;


    List<StageCell> stageCells;

    protected override void EnableUI()
    {

    }

    private void Start()
    {
        stageCells = new List<StageCell>();
        foreach(StageIcon stageIcon in stageIcons)
        {
            stageIcon.Setup(() =>
            {
                int addCellCount = stageIcon.roundList.Count - stageCells.Count;
                for(int i = 0; i<addCellCount; i++)
                {
                    StageCell cell = Instantiate(stageCellPrefab, content);
                    stageCells.Add(cell);
                }

                for(int i = 0; i < stageIcon.roundList.Count; i++)
                {
                    stageCells[i].Setup(stageIcon.StageName, stageIcon.roundList[i]);
                }
                for(int i = stageIcon.roundList.Count; i < stageCells.Count; i++)
                {
                    stageCells[i].Reset();
                }
            });
        }

        // 미션 버튼
        missionStageIcon.Setup(() =>
        {
            int addCellCount = missionDataList.Count - stageCells.Count;
            for (int i = 0; i < addCellCount; i++)
            {
                StageCell cell = Instantiate(stageCellPrefab, content);
                stageCells.Add(cell);
            }

            for (int i = 0; i < missionDataList.Count; i++)
            {
                stageCells[i].Setup(i + 1, missionDataList[i]);
            }
            for (int i = missionDataList.Count; i < stageCells.Count; i++)
            {
                stageCells[i].Reset();
            }
        });
    }
}
