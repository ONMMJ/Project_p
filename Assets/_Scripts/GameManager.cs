using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Range(1, 3)]
    public int speedGrade = 1;
    public float gameSpeed => 1f / speedGrade;

    [HideInInspector] public string stageId;
    [HideInInspector] public string stageFullName;
    [HideInInspector] public bool isMission;
    [HideInInspector] public MissionData missionData;
}
