using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;

public class Notice
{
    public string title;
    public string contents;
    public DateTime postingDate;
    public string imageKey;
    public string inDate;
    public string uuid;
    public string linkUrl;
    public bool isPublic;
    public string linkButtonName;
    public string author;

    public override string ToString()
    {
        return $"title : {title}\n" +
        $"contents : {contents}\n" +
        $"postingDate : {postingDate}\n" +
        $"imageKey : {imageKey}\n" +
        $"inDate : {inDate}\n" +
        $"uuid : {uuid}\n" +
        $"linkUrl : {linkUrl}\n" +
        $"isPublic : {isPublic}\n" +
        $"linkButtonName : {linkButtonName}\n" +
        $"author : {author}\n";
    }
}

public class BackendNotice
{
    private readonly List<Notice> _noticeList = new();
    public IReadOnlyList<Notice> NoticeList => (IReadOnlyList<Notice>)_noticeList.AsReadOnlyCollection();
    public void GetNoticeList()
    {
        BackendReturnObject bro = Backend.Notice.NoticeList(100);
        if (bro.IsSuccess())
        {
            Debug.Log("리턴값 : " + bro);
            _noticeList.Clear();

            LitJson.JsonData jsonList = bro.FlattenRows();
            for (int i = 0; i < jsonList.Count; i++)
            {
                Notice notice = new Notice();

                notice.title = jsonList[i]["title"].ToString();
                notice.contents = jsonList[i]["content"].ToString();
                notice.postingDate = DateTime.Parse(jsonList[i]["postingDate"].ToString());
                notice.inDate = jsonList[i]["inDate"].ToString();
                notice.uuid = jsonList[i]["uuid"].ToString();
                notice.isPublic = jsonList[i]["isPublic"].ToString() == "y" ? true : false;
                notice.author = jsonList[i]["author"].ToString();

                if (jsonList[i].ContainsKey("imageKey"))
                {
                    notice.imageKey = "http://upload-console.thebackend.io" + jsonList[i]["imageKey"].ToString();
                }
                if (jsonList[i].ContainsKey("linkUrl"))
                {
                    notice.linkUrl = jsonList[i]["linkUrl"].ToString();
                }
                if (jsonList[i].ContainsKey("linkButtonName"))
                {
                    notice.linkButtonName = jsonList[i]["linkButtonName"].ToString();
                }

                _noticeList.Add(notice);
                Debug.Log(notice.ToString());
            }
        }
    }
}
