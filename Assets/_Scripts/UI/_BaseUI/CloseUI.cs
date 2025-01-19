using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseUI : MonoBehaviour
{
    [SerializeField] BaseUI closeUIObject;

    Button closeButton;

    void Start()
    {
        closeButton = GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ClosePrevUI(closeUIObject);
        });
    }
}
