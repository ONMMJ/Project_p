using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static List<DontDestroy> Instance = new List<DontDestroy>();

    void Awake()
    {
        if (!Instance.Contains(this))
            Instance.Add(this);
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
