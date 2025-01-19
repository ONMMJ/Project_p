using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseManualBar : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    private bool isActive;

    private void Start()
    {
        isActive = false;
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }

    public void ManualOn(string text)
    {
        isActive = true;
        gameObject.SetActive(true);
        this.text.text = text;
    }
    public void ManualOff()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        //if (isActive)
        //{
        //    ObjectMove();
        //}
    }

    private void ObjectMove()
    {
        // Screen 좌표계인 mousePosition을 World 좌표계로 바꾼다
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.y = mousePos.y + 2f;

        Vector3 pos = Camera.main.WorldToViewportPoint(mousePos);
        if (pos.x < 0f) pos.x = 0f;
        if (pos.x > 1f) pos.x = 1f;
        if (pos.y < 0f) pos.y = 0f;
        if (pos.y > 1f) pos.y = 1f;
        transform.localPosition = Camera.main.ViewportToWorldPoint(pos);


    }
}
