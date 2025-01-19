using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoticeLine : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text text;

    Notice notice;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Setup(Notice notice)
    {
        this.notice = notice;
        text.text = notice.ToString();
    }

    private void OnClick()
    {
        UIManager.Instance.NoticeUI.OpenDetail(notice);
    }

}
