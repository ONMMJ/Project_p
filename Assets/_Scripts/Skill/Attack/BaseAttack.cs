using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    protected Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public abstract IEnumerator IEAttack(Pet pet, Pet target);
}
