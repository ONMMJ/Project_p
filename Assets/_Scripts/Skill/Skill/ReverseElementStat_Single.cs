using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseElementStat_Single : BaseSkill
{
    public override void Skill()
    {
        BattleManager.Instance.GetReverseElementStat(target, Count);
    }
}
