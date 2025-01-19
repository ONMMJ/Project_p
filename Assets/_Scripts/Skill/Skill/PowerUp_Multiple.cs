using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Multiple : BaseSkill
{
    public override void Skill()
    {
        List<Pet> targets = BattleController.Instance.GetTarget(target, TARGET_GROUP.TEAM_ALL);
        BattleManager.Instance.GetBuff(targets, skillInfo);
    }
}
