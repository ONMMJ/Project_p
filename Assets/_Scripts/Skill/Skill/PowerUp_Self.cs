using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Self : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetBuff(pet, skillInfo);
    }
}
