using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd.MultiCharacter;
using System;

public class NoticeUI : BaseUI
{
    [SerializeField] Transform noticeLineParent;
    [SerializeField] NoticeLine noticeLinePrefab;

    [Header("NoticeDetail")]
    [SerializeField] ScrollRect noticeDetailScroll;
    [SerializeField] TMP_Text noticeDetailText;

    List<NoticeLine> noticeList = new();

    protected override void EnableUI()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (noticeList.Count > 0)
        {
            foreach (var obj in noticeList)
            {
                Destroy(obj.gameObject);
            }
            noticeList.Clear();
        }
        noticeDetailScroll.gameObject.SetActive(false);
        BackendManager.Instance.Notice.GetNoticeList();
        SetNotice();
    }

    private void SetNotice()
    {
        foreach (Notice notice in BackendManager.Instance.Notice.NoticeList)
        { 
            NoticeLine noticeLine = Instantiate(noticeLinePrefab, noticeLineParent);
            noticeLine.Setup(notice);
            noticeList.Add(noticeLine);
        }
    }

    public void OpenDetail(Notice notice)
    {
        noticeDetailScroll.gameObject.SetActive(true);
        noticeDetailScroll.verticalNormalizedPosition = 1;
        noticeDetailText.text = notice.ToString();
    }
}
