using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Multiple : BaseSkill
{
    public override void Skill()
    {
        target.GetHeal(new HealInfo(pet, skillInfo.mainDamage, skillInfo.skillTurnCount));

        List<Pet> otherTargets = BattleController.Instance.GetTarget(pet, TARGET_GROUP.TEAM_OTHER);
        foreach (Pet otherTarget in otherTargets)
        {
            otherTarget.GetHeal(new HealInfo(pet, skillInfo.subDamage, skillInfo.skillTurnCount));
        }
    }
}
