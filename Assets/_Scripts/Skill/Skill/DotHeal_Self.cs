using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotHeal_Self : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetDotHeal(pet, pet, skillInfo.mainDamage, skillInfo.skillTurnCount);
    }
}
