using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    int animationNum;
    protected SkillInfo skillInfo;

    public Sprite Icon => skillInfo.icon;
    public ATTACK_TYPE AttackType => skillInfo.attackType;

    protected Pet pet;
    protected Pet target => pet.target;
    protected ATTACK_RANGE? attackRange;

    protected int attackNum = 1;

    protected float MainDamage => skillInfo.mainDamage / attackNum;
    protected float SubDamage => skillInfo.subDamage / attackNum;
    protected int Count => skillInfo.skillTurnCount;

    protected int skillMaxPoint => pet.skillMaxPoint;
    protected int skillPoint
    {
        get { return pet.skillPoint; }
        set { pet.skillPoint = value; }
    }

    public bool isAnimation
    {
        get { return pet.isAttackAnimation; }
        set { pet.isAttackAnimation = value; }
    }

    public bool IsReady => pet.skillPoint >= skillInfo.skillUsePoint;

    public virtual void Setup(string skillName, int skillLevel, int animationNum, ATTACK_RANGE? attackRange)
    {
        this.animationNum = animationNum;
        this.attackRange = attackRange;
        string key = $"{skillName}{skillLevel}";
        if (BackendManager.Instance.Chart.SkillChart.Dictionary.ContainsKey(key))
            skillInfo = BackendManager.Instance.Chart.SkillChart.Dictionary[key];
        else
        {
            // 스킬이 스킬 차트에 없을 때
        }
    }

    public void SetPet(Pet pet)
    {
        this.pet = pet;
        isAnimation = false;
    }
    public IEnumerator UseSkill()
    {

        // 공격 타입에 따라 처음 위치이동
        switch (attackRange)
        {
            case ATTACK_RANGE.MELEE:
                pet.animator.SetBool("IsMove", true);
                yield return StartCoroutine(Movement.IEMove(transform, pet.target.transform, 1f, 1.5f));
                break;
            case ATTACK_RANGE.RANGED:
                pet.animator.SetBool("IsMove", true);
                yield return StartCoroutine(Movement.IEMove(transform, BattleController.Instance.CenterPos, 1f));
                transform.LookAt(pet.target.transform);
                break;
            case ATTACK_RANGE.BUFF:
                break;

        }

        // 애니메이션 부분
        pet.animator.SetTrigger($"OnSkill_{animationNum}");
        while (!pet.animator.GetCurrentAnimatorStateInfo(0).IsName($"Skill_{animationNum}"))
        {
            yield return null;
        }
        isAnimation = true;
        while (pet.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return null;
        }
        isAnimation = false;
        skillPoint = 0;
        // 같이 공격하는 공격자가 애니메이션이 끝났는지 확인
        if (BattleController.Instance.attackerList.Count > 1)
        {
            while (BattleController.Instance.attackerList.Where(x => x.isAttackAnimation).Count() > 0)
            {
                yield return null;
            }
        }

        if (attackRange != ATTACK_RANGE.BUFF)
        {
            yield return StartCoroutine(Movement.IEMove(transform, pet.startPos, 1f));
            transform.rotation = pet.startPos.rotation;
            pet.animator.SetBool("IsMove", false);
        }

    }

    public abstract void Skill();
    public virtual void Skill(int attackNum)
    {
        this.attackNum = attackNum;
        Skill();
    }

    public virtual void AttackTurn()
    {
        if(skillMaxPoint>skillPoint)
            skillPoint++;
        Debug.Log($"보유 스킬포인트: {skillPoint} / 필요 스킬포인트: {skillInfo.skillUsePoint}");
    }

    public string GetManual()
    {
        return skillInfo.skillManual;
    }
}
