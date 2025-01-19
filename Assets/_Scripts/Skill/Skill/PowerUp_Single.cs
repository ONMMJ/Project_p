using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Single : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetBuff(target, skillInfo);
    }
}
