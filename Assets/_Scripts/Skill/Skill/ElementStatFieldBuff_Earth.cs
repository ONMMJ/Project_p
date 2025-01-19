using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementStatFieldBuff_Earth : BaseSkill
{
    public override void Skill()
    {
        BattleController.Instance.fieldBuff.Setup(ELEMENT_TYPE.EARTH, skillInfo.skillTurnCount);
    }
}
