using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeAttack : BaseAttack
{
    public bool isAnimation
    {
        get { return pet.isAttackAnimation; }
        set { pet.isAttackAnimation = value; }
    }

    [SerializeField] int AttackCount = 1;
    float dmg;
    Pet pet;
    Pet target;

    public override IEnumerator IEAttack(Pet pet, Pet target)
    {
        this.dmg = pet.RealDamage(target)/AttackCount;
        this.pet = pet;
        this.target = target;

        animator.SetBool("IsMove",true);
        yield return StartCoroutine(Movement.IEMove(transform, target.transform, 1f, 1.5f));

        // 애니메이션 부분
        animator.SetTrigger("OnAttack");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null;
        }
        isAnimation = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return null;
        }
        isAnimation = false;
        if (BattleController.Instance.attackerList.Count > 1)
        {
            //animator.SetBool("IsMove", false);
            while (BattleController.Instance.attackerList.Where(x => x.isAttackAnimation).Count() > 0)
            {
                yield return null;
            }
            //animator.SetBool("IsMove", true);
        }

        yield return StartCoroutine(Movement.IEMove(transform, pet.startPos, 1f));
        transform.rotation = pet.startPos.rotation;
        animator.SetBool("IsMove", false);
    }

    public void Attack()
    {
        target.GetDamaged(dmg, pet);
    }
}
