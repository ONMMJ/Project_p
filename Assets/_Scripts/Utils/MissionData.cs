using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionPet
{
    [SerializeField] public string petName;
    [SerializeField] public int petLevel;
    [SerializeField] public int startSkillPoint;
}

[CreateAssetMenu(fileName = "MissionPetData", menuName = "Scriptable Object/MissionPetData", order = int.MaxValue)]
public class MissionData : ScriptableObject
{
    [SerializeField] List<MissionPet> missionPetList;
    [SerializeField] List<MissionPet> missionEnemyList;

    public List<MissionPet> MissionPetList => missionPetList;
    public List<MissionPet> MissionEnemyList => missionEnemyList;
}