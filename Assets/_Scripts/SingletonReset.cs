using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonReset<T> : MonoBehaviour
    where T : class
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = this as T;
    }
}
