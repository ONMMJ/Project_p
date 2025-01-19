using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Self : BaseSkill
{
    public override void Skill()
    {
        pet.GetHeal(new HealInfo(pet, skillInfo.mainDamage, skillInfo.skillTurnCount));
    }
}
