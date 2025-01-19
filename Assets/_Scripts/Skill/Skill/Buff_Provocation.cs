using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Provocation : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetProvocation(pet, skillInfo.skillTurnCount);
        BattleManager.Instance.GetBuff(pet, skillInfo);
    }
}
