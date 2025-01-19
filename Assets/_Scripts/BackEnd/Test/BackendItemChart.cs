// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

public class Item
{
    public string itemName { get; private set; }  //펫 이름
    public string koreanName { get; private set; }
    public string explanation { get; private set; }

    public Item(JsonData json)
    {
        itemName = json["itemName"].ToString();
        koreanName = json["koreanName"].ToString();
        explanation = json["explanation"].ToString();
    }
}

public class BackendItemChart : Chart
{

    // 각 차트의 row 정보를 담는 Dictionary
    private readonly Dictionary<string, Item> _dictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    public IReadOnlyDictionary<string, Item> Dictionary => (IReadOnlyDictionary<string, Item>)_dictionary.AsReadOnlyCollection();

    // 이미지 캐싱을 관리하는 Dictionary
    private readonly Dictionary<string, Sprite> _itemIcon = new();
    public IReadOnlyDictionary<string, Sprite> ItemIcon => (IReadOnlyDictionary<string, Sprite>)_itemIcon.AsReadOnlyCollection();

    // 차트 파일 이름 설정 함수
    // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
    public override string GetChartFileName()
    {
        return "ITEM_CHART";
    }

    // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
    // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고해주세요
    protected override void LoadChartDataTemplate(JsonData json)
    {
        foreach (JsonData eachItem in json)
        {
            Item info = new Item(eachItem);

            _dictionary.Add(info.itemName, info);
            base.AddOrGetImageDictionary(_itemIcon, "ItemIcons/", info.itemName);
        }
    }

    public Sprite GetSpriteIcon(string itemName)
    {
        Sprite sprite;
        if (ItemIcon.ContainsKey(itemName))
            sprite = ItemIcon[itemName];
        else
            sprite = Resources.Load<Sprite>("Null");
        return sprite;
    }
}