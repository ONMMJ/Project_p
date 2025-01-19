using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posion_Multiple : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetDotDamage(pet, target, skillInfo.mainDamage, skillInfo.skillTurnCount);

        List<Pet> otherEnemies = BattleController.Instance.GetTarget(pet, TARGET_GROUP.ENEMY_OTHER);
        BattleManager.Instance.GetDotDamage(pet, otherEnemies, skillInfo.subDamage, skillInfo.skillTurnCount);
    }
}
