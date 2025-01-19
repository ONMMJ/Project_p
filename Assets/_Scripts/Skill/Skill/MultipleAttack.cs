using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAttack : BaseSkill
{
    public override void Skill()
    {
        float mainDmg = pet.RealDamage(target);
        mainDmg *= MainDamage;
        target.GetDamaged(mainDmg, pet);

        List<Pet> otherEnemies = BattleController.Instance.GetTarget(pet, TARGET_GROUP.ENEMY_OTHER);
        foreach (Pet otherEnemy in otherEnemies)
        {
            float otherDmg = pet.RealDamage(otherEnemy);
            otherDmg *= SubDamage;
            otherEnemy.GetDamaged(otherDmg, pet);
        }
    }
}
