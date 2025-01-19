using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAttack_Stun : BaseSkill
{
    public override void Skill()
    {
        // 데미지
        float mainDmg = pet.RealDamage(target);
        mainDmg *= skillInfo.mainDamage;
        target.GetDamaged(mainDmg, pet);

        List<Pet> otherEnemies = BattleController.Instance.GetTarget(pet, TARGET_GROUP.ENEMY_OTHER);
        foreach (Pet otherEnemy in otherEnemies)
        {
            float otherDmg = pet.RealDamage(otherEnemy);
            otherDmg *= skillInfo.subDamage;
            otherEnemy.GetDamaged(otherDmg, pet);
        }

        // 스턴
        List<Pet> allEnemies = BattleController.Instance.GetTarget(pet, TARGET_GROUP.ENEMY_ALL);
        BattleManager.Instance.GetStun(allEnemies, skillInfo.skillTurnCount);
    }
}
