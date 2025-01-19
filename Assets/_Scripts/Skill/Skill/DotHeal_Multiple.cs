using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotHeal_Multiple : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetDotHeal(pet, target, skillInfo.mainDamage, skillInfo.skillTurnCount);

        List<Pet> otherTargets = BattleController.Instance.GetTarget(pet, TARGET_GROUP.TEAM_OTHER);
        BattleManager.Instance.GetDotHeal(pet, otherTargets, skillInfo.subDamage, skillInfo.skillTurnCount);
    }
}
