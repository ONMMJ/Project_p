using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotHeal_Single : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetDotHeal(pet, target, skillInfo.mainDamage, skillInfo.skillTurnCount);
    }
}
