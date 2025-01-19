using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

public enum ITEM_TYPE
{
    PET,
    ETC
}

public class AttachedItem
{
    public ITEM_TYPE itemType;
    public int itemCount;
    public string itemName;
    public Sprite icon;
    public AttachedItem(JsonData json)
    {
        string chartName = json["chartName"].ToString();
        itemCount = int.Parse(json["itemCount"].ToString());

        switch (chartName)
        {
            case "PET_CHART":
                itemType = ITEM_TYPE.PET;
                itemName = json["item"]["petName"].ToString();
                icon = BackendManager.Instance.Chart.PetChart.GetSpriteIcon(itemName);
                break;
            default:
                itemType = ITEM_TYPE.ETC;
                break;

        }
    }
    public override string ToString()
    {
        return itemName;
    }
}

public class UPostItem
{
    public PostType postType;
    public string title;
    public string content;
    public DateTime expirationDate;
    public DateTime reservationDate;
    public DateTime sentDate;
    public string nickname;
    public string inDate;
    public string author; // 관리자 우편만 존재합니다.  
    public string rankType; // 랭킹 우편만 존재합니다.  
    public List<AttachedItem> items = new List<AttachedItem>();
    public override string ToString()
    {
        string totalString =
        $"title : {title}\n" +
        $"inDate : {inDate}\n";
        if (postType == PostType.Admin || postType == PostType.Rank)
        {
            totalString +=
            $"content : {content}\n" +
            $"expirationDate : {expirationDate}\n" +
            $"reservationDate : {reservationDate}\n" +
            $"sentDate : {sentDate}\n" +
            $"nickname : {nickname}\n";
            if (postType == PostType.Admin)
            {
                totalString += $"author : {author}\n";
            }
            if (postType == PostType.Rank)
            {
                totalString += $"rankType : {rankType}\n";
            }
        }
        string itemList = string.Empty;
        for (int i = 0; i < items.Count; i++)
        {
            itemList += items[i].ToString();
            itemList += "\n";
        }
        totalString += itemList;
        return totalString;
    }
}

public class BackendPost
{
    private readonly List<UPostItem> _postList = new();
    public IReadOnlyList<UPostItem> PostList => (IReadOnlyList<UPostItem>)_postList.AsReadOnlyCollection();

    public void GetPostList(PostType postType)
    {
        int limit = 100;
        BackendReturnObject bro = Backend.UPost.GetPostList(postType, limit);
        if (!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return;
        }
        _postList.Clear();
        LitJson.JsonData postListJson = bro.GetReturnValuetoJSON()["postList"];
        for (int i = 0; i < postListJson.Count; i++)
        {

            UPostItem postItem = new UPostItem();
            
            postItem.inDate = postListJson[i]["inDate"].ToString();
            postItem.title = postListJson[i]["title"].ToString();
            postItem.postType = postType;

            if (postType == PostType.Admin || postType == PostType.Rank)
            {
                postItem.content = postListJson[i]["content"].ToString();
                postItem.expirationDate = DateTime.Parse(postListJson[i]["expirationDate"].ToString());
                postItem.reservationDate = DateTime.Parse(postListJson[i]["reservationDate"].ToString());
                postItem.nickname = postListJson[i]["nickname"]?.ToString();
                postItem.sentDate = DateTime.Parse(postListJson[i]["sentDate"].ToString());

                if (postListJson[i].ContainsKey("author"))
                {
                    postItem.author = postListJson[i]["author"].ToString();
                }
                if (postListJson[i].ContainsKey("rankType"))
                {
                    postItem.author = postListJson[i]["rankType"].ToString();
                }
            }
            if (postListJson[i]["items"].Count > 0)
            {
                for (int itemNum = 0; itemNum < postListJson[i]["items"].Count; itemNum++)
                {
                    AttachedItem item = new AttachedItem(postListJson[i]["items"][itemNum]);
                    postItem.items.Add(item);
                }
            }

            _postList.Add(postItem);
        }
    }

    public void ReceivePostItem(UPostItem post)
    {
        var receiveBro = Backend.UPost.ReceivePostItem(post.postType, post.inDate);

        if (receiveBro.IsSuccess() == false)
        {
            Debug.LogError("우편 수령중 에러가 발생했습니다. : " + receiveBro);
            return;
        }

        LitJson.JsonData receivePostItemJson = receiveBro.GetReturnValuetoJSON()["postItems"];

        foreach(AttachedItem item in post.items)
        {
            switch (item.itemType)
            {
                case ITEM_TYPE.PET:
                    BackendManager.Instance.GameData.UserPetData.AddPetData(PetManager.Instance.GetLv1Pet(item.itemName));
                    break;
            }
        }
        post.items.Clear();

        // 무결성 검사 안되어 있음 
        //for (int i = 0; i < receivePostItemJson.Count; i++)
        //{
        //    ReceiveItem item = new ReceiveItem();
        //    if (receivePostItemJson[i]["item"].ContainsKey("itemName"))
        //    {
        //        item.itemName = receivePostItemJson[i]["item"]["itemName"].ToString();
        //    }

        //    // 랭킹 보상의 경우 chartFileName이 존재하지 않습니다.  
        //    if (receivePostItemJson[i]["item"].ContainsKey("chartFileName"))
        //    {
        //        item.chartFileName = receivePostItemJson[i]["item"]["chartFileName"].ToString();
        //    }

        //    if (receivePostItemJson[i]["item"].ContainsKey("itemID"))
        //    {
        //        item.itemID = receivePostItemJson[i]["item"]["itemID"].ToString();
        //    }

        //    if (receivePostItemJson[i]["item"].ContainsKey("hpPower"))
        //    {
        //        item.hpPower = int.Parse(receivePostItemJson[i]["item"]["hpPower"].ToString());
        //    }

        //    item.itemCount = int.Parse(receivePostItemJson[i]["itemCount"].ToString());

        //    Debug.Log(item.ToString());

        //}
    }
}
