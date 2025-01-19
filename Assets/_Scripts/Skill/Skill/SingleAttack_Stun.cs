using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttack_Stun : BaseSkill
{
    public override void Skill()
    {
        float mainDmg = pet.RealDamage(target);
        mainDmg *= MainDamage;
        target.GetDamaged(mainDmg, pet);

        BattleManager.Instance.GetStun(target, Count);
    }
}
