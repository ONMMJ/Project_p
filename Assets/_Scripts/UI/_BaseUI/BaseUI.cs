using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public abstract class BaseUI : MonoBehaviour
{
    public bool activeSelf => gameObject.activeSelf;

    private void OnEnable()
    {
        UIManager.Instance.OpenUI(this);
        transform.localPosition = Vector3.back * 1500;
        EnableUI();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void CloseUI()
    {
        ResetPosition();
        gameObject.SetActive(false);
    }

    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    protected abstract void EnableUI();
}
