using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

public class UItem
{
    public string itemName;
    public int itemCount;
    public Item item;

    public UItem(Item item, int itemCount)
    {
        itemName = item.itemName;
        this.itemCount = itemCount;
        this.item = item;
    }
    public UItem(JsonData json)
    {

        itemName = json["itemName"].ToString();
        itemCount = int.Parse(json["itemCount"].ToString());
        item = BackendManager.Instance.Chart.ItemChart.Dictionary[itemName];
    }
    public void AddItem(int itemCount)
    {
        this.itemCount += itemCount;
    }
    public bool UseItem(int itemCount)
    {
        if (this.itemCount < itemCount)
            return false;
        else
            this.itemCount -= itemCount;

        return true;
    }
}

[System.Serializable]
public partial class BackendUserData
{
    public int gold { get; private set; }
    public int diamond { get; private set; }
    public int maxInvenCount { get; private set; }
    public string[] selectPets { get; private set; } = new string[3];
    public Dictionary<string, UItem> items { get; private set; } = new();
}

public partial class BackendUserData : GameData
{
    // 테이블 이름 설정 함수
    public override string GetTableName()
    {
        return "USER_DATA";
    }

    // 컬럼 이름 설정 함수
    public override string GetColumnName()
    {
        return null;
    }

    // 데이터가 존재하지 않을 경우, 초기값 설정
    protected override void InitializeData()
    {
        gold = 1000;
        diamond = 0;
        maxInvenCount = 10;
        selectPets = new string[3] { "", "", "" };
        items = new();
    }

    private void JsonCheck(JsonData gameDataJson)
    {
        List<string> newColumnList = new();
        if (gameDataJson.ContainsKey("gold"))
        {
            gold = int.Parse(gameDataJson["gold"].ToString());
        }
        else
        {
            gold = 1000;
            newColumnList.Add("gold");
        }


        if (gameDataJson.ContainsKey("diamond"))
        {
            diamond = int.Parse(gameDataJson["diamond"].ToString());
        }
        else
        {
            diamond = 0;
            newColumnList.Add("diamond");
        }


        if (gameDataJson.ContainsKey("maxInvenCount"))
        {
            maxInvenCount = int.Parse(gameDataJson["maxInvenCount"].ToString());
        }
        else
        {
            maxInvenCount = 10;
            newColumnList.Add("maxInvenCount");
        }


        if (gameDataJson.ContainsKey("selectPets"))
        {
            for (int i = 0; i < gameDataJson["selectPets"].Count; i++)
            {
                selectPets[i] = gameDataJson["selectPets"][i].ToString();
            }
        }
        else
        {
            selectPets = new string[3] { "", "", "" };
            newColumnList.Add("selectPets");
        }


        if (gameDataJson.ContainsKey("items"))
        {
            JsonData itemJson = gameDataJson["items"];
            for (int i = 0; i < itemJson.Count; i++)
            {
                UItem item = new(itemJson[i]);
                items.Add(item.itemName, item);
            }
        }
        else
        {
            items = new();
            newColumnList.Add("items");
        }

        Update((callback) => {
            if (callback.IsSuccess())
            {
                Debug.Log($"컬럼 추가: {callback}");
            }
            else
            {
                Debug.LogError($"컬럼추가 에러: {callback})");
            }
        });
    }

    // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
    // Dictionary 하나만 삽입
    public override Param GetParam()
    {
        Param param = new Param();
        param.Add("gold", gold);
        param.Add("diamond", diamond);
        param.Add("maxInvenCount", maxInvenCount);
        param.Add("selectPets", selectPets);
        param.Add("items", items);
        return param;

    }

    // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
    // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
    protected override void SetServerDataToLocal(JsonData gameDataJson)
    {
        try
        {
            gold = int.Parse(gameDataJson["gold"].ToString());
            diamond = int.Parse(gameDataJson["diamond"].ToString());
            maxInvenCount = int.Parse(gameDataJson["maxInvenCount"].ToString());

            for (int i = 0; i < gameDataJson["selectPets"].Count; i++)
            {
                selectPets[i] = gameDataJson["selectPets"][i].ToString();
            }

            JsonData itemJson = gameDataJson["items"];
            for (int i = 0; i < itemJson.Count; i++)
            {
                UItem item = new(itemJson[i]);
                items.Add(item.itemName, item);
            }
        }
        catch
        {
            JsonCheck(gameDataJson);
        }
    }

    public void DeleteSelectPet(string petId)
    {
        int idx = Array.IndexOf(selectPets, petId);
        if (idx == -1)
            return;

        IsChangedData = true;
        selectPets[idx] = "";

        Update((callback) => {
            if (callback.IsSuccess())
            {
                IsChangedData = false;
                Debug.Log($"펫 삭제시 대표캐릭터 동시 삭제 성공: {callback}");
            }
            else
            {
                Debug.LogError($"펫 삭제 시 대표캐릭터 동시 삭제 에러: {callback})");
            }
        });
    }

    public void SwapPet(int idx1, int idx2)
    {
        string temp = selectPets[idx1];
        selectPets[idx1] = selectPets[idx2];
        selectPets[idx2] = temp;
    }

    public void AddItem(Item item, int itemCount)
    {
        if (items.ContainsKey(item.itemName))
        {
            items[item.itemName].AddItem(itemCount);
        }
        else
        {
            items.Add(item.itemName, new UItem(item, itemCount));
        }
        Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                UIManager.Instance.SetUserUI();
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                Debug.Log($"아이템 추가 적용 성공: {callback}");
            }
            else
            {
                Debug.LogError($"아이템 추가 적용 실패: {callback})");
            }
        });
    }

    public bool UseItem(Item item, int itemCount)
    {
        bool isSuccess = true;
        if (items.ContainsKey(item.itemName))
        {
            if (items[item.itemName].UseItem(itemCount))
            {
                if (items[item.itemName].itemCount <= 0)
                {
                    items.Remove(item.itemName);
                }
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }
        }
        else
        {
            isSuccess = false;
        }
        Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                UIManager.Instance.SetUserUI();
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                Debug.Log($"아이템 사용 적용 성공: {callback}");
                isSuccess = true;
            }
            else
            {
                Debug.LogError($"아이템 사용 적용 실패: {callback})");
                isSuccess = false;
            }
        });
        /////////////////////////////// 위에 bool 적용이 안되고 리턴되는 문제 수정필요
        return isSuccess;
    }

    public void AddGold(int gold)
    {
        this.gold += gold;
        Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                UIManager.Instance.SetUserUI();
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                Debug.Log($"골드 적용 성공: {callback}");
            }
            else
            {
                Debug.LogError($"골드 적용 실패: {callback})");
            }
        });
    }

    public bool UseGold(int gold)
    {
        if (this.gold < gold)
            return false;

        this.gold -= gold;
        bool isSuccess = true;
        Update((callback) =>
        {
            if (callback.IsSuccess())
            {
                UIManager.Instance.SetUserUI();
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                Debug.Log($"골드 적용 성공: {callback}");
                isSuccess = true;
            }
            else
            {
                Debug.LogError($"골드 적용 실패: {callback})");
                isSuccess = false;
            }
        });
        /////////////////////////////// 위에 bool 적용이 안되고 리턴되는 문제 수정필요
        return isSuccess;
    }
}