using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitSizeUI : MonoBehaviour
{
    public enum FIT_TYPE
    {
        FIT_X,
        FIT_Y,
    }
    [SerializeField] FIT_TYPE fitType;
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float x = rectTransform.sizeDelta.x;
        float y = rectTransform.sizeDelta.y;
        switch (fitType)
        {
            case FIT_TYPE.FIT_X:
                rectTransform.sizeDelta = new Vector2(x, x);
                break;
            case FIT_TYPE.FIT_Y:
                rectTransform.sizeDelta = new Vector2(y, y);
                break;
        }
    }
}
