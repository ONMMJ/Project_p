using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    bool isShot = false;
    float dmg;
    Pet pet;
    Pet target;
    public void Setup(float dmg, Pet pet, Pet target)
    {
        this.dmg = dmg;
        this.pet = pet;
        this.target = target;

        this.isShot = true;
    }
    private void Update()
    {
        if (!isShot)
            return;

        if (target == null)
            Destroy(gameObject);
        else
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed / GameManager.Instance.gameSpeed);

        if(Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            // 타겟 공격
            target.GetDamaged(dmg, pet);

            Destroy(gameObject);
        }
    }
}
