using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Single : BaseSkill
{
    public override void Skill()
    {
        target.GetHeal(new HealInfo(pet, skillInfo.mainDamage, skillInfo.skillTurnCount));
    }
}
