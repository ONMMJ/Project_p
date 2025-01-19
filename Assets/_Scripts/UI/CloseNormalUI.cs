using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseNormalUI : MonoBehaviour
{
    [SerializeField] GameObject obj;

    Button closeButton;

    void Start()
    {
        closeButton = GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            obj.SetActive(false);
        });
    }
}
