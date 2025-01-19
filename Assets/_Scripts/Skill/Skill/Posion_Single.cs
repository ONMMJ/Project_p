using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posion_Single : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetDotDamage(pet, target, skillInfo.mainDamage, skillInfo.skillTurnCount);
    }
}
