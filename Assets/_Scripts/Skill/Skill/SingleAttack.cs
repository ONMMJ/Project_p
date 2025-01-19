using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttack : BaseSkill
{
    public override void Skill()
    {
        float mainDmg = pet.RealDamage(target);
        mainDmg *= MainDamage;
        target.GetDamaged(mainDmg, pet);
    }
}
