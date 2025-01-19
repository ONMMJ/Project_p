 using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class UserPetData
{
    public string petName;

    public int petNo;
    public string petId;
    public int petRank;
    public bool isEnemy;
    public int resetCount;

    public int petLevel;
    public int petNowExp;
    public int petNextExp;

    public STAT_COE_RANK addVit;
    public STAT_COE_RANK addAtk;
    public STAT_COE_RANK addDef;
    public STAT_COE_RANK addDex;

    public float petVit;
    public float petAtk;
    public float petDef;
    public float petDex;

    public float petBaseVit;
    public float petBaseAtk;
    public float petBaseDef;
    public float petBaseDex;

    public int[] skillsLevel;

    public UserPetData()
    {
        skillsLevel = new int[3];
    }
}

public class BackendUserPetData : GameData
{
    // 아이템을 담는 Dictionary
    private Dictionary<string, UserPetData> _dictionary = new();

    // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
    public IReadOnlyDictionary<string, UserPetData> Dictionary =>
        (IReadOnlyDictionary<string, UserPetData>)_dictionary.AsReadOnlyCollection();

    // 테이블 이름 설정 함수
    public override string GetTableName()
    {
        return "USER_PET_DATA";
    }

    // 컬럼 이름 설정 함수
    public override string GetColumnName()
    {
        return "USER_PET_DATA";
    }

    // 데이터가 존재하지 않을 경우, 초기값 설정
    protected override void InitializeData()
    {
        _dictionary.Clear();
    }


    // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
    // Dictionary 하나만 삽입
    public override Param GetParam()
    {
        Param param = new Param();
        param.Add(GetColumnName(), _dictionary);

        return param;

    }

    // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
    // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
    protected override void SetServerDataToLocal(JsonData gameDataJson)
    {
        for (int i = 0; i < gameDataJson.Count; i++)
        {
            UserPetData userPetData = new UserPetData();
            userPetData.petName = gameDataJson[i]["petName"].ToString();

            userPetData.petNo = int.Parse(gameDataJson[i]["petNo"].ToString());
            userPetData.petId = gameDataJson[i]["petId"].ToString();
            userPetData.petRank = int.Parse(gameDataJson[i]["petRank"].ToString());
            userPetData.isEnemy = bool.Parse(gameDataJson[i]["isEnemy"].ToString());
            userPetData.resetCount = int.Parse(gameDataJson[i]["resetCount"].ToString());

            userPetData.petLevel = int.Parse(gameDataJson[i]["petLevel"].ToString());
            userPetData.petNowExp = int.Parse(gameDataJson[i]["petNowExp"].ToString());
            userPetData.petNextExp = int.Parse(gameDataJson[i]["petNextExp"].ToString());

            userPetData.addVit = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addVit"].ToString());
            userPetData.addAtk = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addAtk"].ToString());
            userPetData.addDef = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addDef"].ToString());
            userPetData.addDex = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addDex"].ToString());

            userPetData.petVit = float.Parse(gameDataJson[i]["petVit"].ToString());
            userPetData.petAtk = float.Parse(gameDataJson[i]["petAtk"].ToString());
            userPetData.petDef = float.Parse(gameDataJson[i]["petDef"].ToString());
            userPetData.petDex = float.Parse(gameDataJson[i]["petDex"].ToString());

            userPetData.petBaseVit = float.Parse(gameDataJson[i]["petBaseVit"].ToString());
            userPetData.petBaseAtk = float.Parse(gameDataJson[i]["petBaseAtk"].ToString());
            userPetData.petBaseDef = float.Parse(gameDataJson[i]["petBaseDef"].ToString());
            userPetData.petBaseDex = float.Parse(gameDataJson[i]["petBaseDex"].ToString());

            JsonCheck(ref userPetData, gameDataJson);

            _dictionary.Add(userPetData.petId, userPetData);
        }
    }

    private void JsonCheck(ref UserPetData userPetData,JsonData gameDataJson)
    {
        if (gameDataJson.ContainsKey("skillsLevel"))
        {
            for(int i = 0; i < gameDataJson["skillsLevel"].Count; i++)
            {
                userPetData.skillsLevel[i] = int.Parse(gameDataJson["skillsLevel"][i].ToString());
            }
        }
        else
        {
            int[] skillsLevel = { 1, 1, 1 };
            userPetData.skillsLevel = skillsLevel;
        }
    }

    public void AddPetData(PetInfo newPetInfo)
    {
        IsChangedData = true;

        string myPetDataId = System.Guid.NewGuid().ToString();
        newPetInfo.petId = myPetDataId;
        _dictionary.Add(myPetDataId, newPetInfo.ExportData());
        Update((callback) => {
            if (callback.IsSuccess())
            {
                IsChangedData = false;
                Debug.Log($"추가 성공: {callback}");
            }
            else
            {
                Debug.LogError($"추가 실패: {callback})");
            }
        });
    }

    public void UpdatePetData(UserPetData updateData)
    {
        IsChangedData = true;

        _dictionary[updateData.petId] = updateData;

        // 외부에서 업데이트 함수 호출
    }

    public void DeletePetData(string petId)
    {
        IsChangedData = true;

        _dictionary.Remove(petId);

        Update((callback) => {
            if (callback.IsSuccess())
            {
                UIManager.Instance.InvenUI.SetContent();
                BackendManager.Instance.GameData.UserData.DeleteSelectPet(petId);
                BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                Debug.Log($"삭제 성공: {callback}");
            }
            else
            {
                Debug.LogError($"삭제 실패: {callback})");
            }
        });
    }
    public void DeletePetData(List<string> petIds)
    {
        IsChangedData = true;
        foreach (string petId in petIds)
            _dictionary.Remove(petId);
    }
    //public static List<PetInfoDB> USER_PET_DB;
    //public static PetInfoDB selectPet;
    //public void GameDataInsert(UserPetData petData)
    //{
    //    Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
    //    Param param = new Param();
    //    param.Add("petName", petData.petName);
    //    param.Add("petNo", petData.petNo);
    //    param.Add("petRank", petData.petRank);
    //    param.Add("isEnemy", petData.isEnemy);
    //    param.Add("resetCount", petData.resetCount);
    //    param.Add("petLevel", petData.petLevel);
    //    param.Add("petNowExp", petData.petNowExp);
    //    param.Add("petNextExp", petData.petNextExp);
    //    param.Add("addVit", (int)petData.addVit);
    //    param.Add("addAtk", (int)petData.addAtk);
    //    param.Add("addDef", (int)petData.addDef);
    //    param.Add("addDex", (int)petData.addDex);
    //    param.Add("petVit", petData.petVit);
    //    param.Add("petAtk", petData.petAtk);
    //    param.Add("petDef", petData.petDef);
    //    param.Add("petDex", petData.petDex);
    //    param.Add("petBaseVit", petData.petBaseVit);
    //    param.Add("petBaseAtk", petData.petBaseAtk);
    //    param.Add("petBaseDef", petData.petBaseDef);
    //    param.Add("petBaseDex", petData.petBaseDex);
    //    param.Add("elementStat", petData.elementStat);

    //    Debug.Log("게임정보 데이터 삽입을 요청합니다.");
    //    var bro = Backend.GameData.Insert("USER_PET_DATA", param);

    //    if (bro.IsSuccess())
    //    {
    //        USER_PET_DB.Add(new PetInfoDB(bro.GetInDate(), new PetInfo(petData)));
    //        Debug.Log("게임정보 데이터 삽입에 성공했습니다. : " + bro);
    //    }
    //    else
    //    {
    //        Debug.LogError("게임정보 데이터 삽입에 실패했습니다. : " + bro);
    //    }
    //}
    //public void GameDataGet()
    //{
    //    Debug.Log("게임 정보 조회 함수를 호출합니다.");

    //    var bro = Backend.GameData.GetMyData("USER_PET_DATA", new Where(), 100);
    //    if (bro.IsSuccess())
    //    {
    //        Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);

    //        List<PetInfoDB> userPetDataList = new List<PetInfoDB>();
    //        while (true)
    //        {
    //            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.

    //            // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.
    //            if (gameDataJson.Count <= 0)
    //            {
    //                USER_PET_DB = new List<PetInfoDB>();
    //                Debug.LogWarning("데이터가 존재하지 않습니다.");
    //            }
    //            else
    //            {
    //                Debug.Log(gameDataJson.Count);
    //                UserPetData userPetData = new UserPetData();
    //                for (int i = 0; i < gameDataJson.Count; i++)
    //                {
    //                    string inData = gameDataJson[i]["inDate"].ToString();

    //                    userPetData.petName = gameDataJson[i]["petName"].ToString();

    //                    userPetData.petNo = int.Parse(gameDataJson[i]["petNo"].ToString());
    //                    userPetData.petRank = int.Parse(gameDataJson[i]["petRank"].ToString());
    //                    userPetData.isEnemy = bool.Parse(gameDataJson[i]["isEnemy"].ToString());
    //                    userPetData.resetCount = int.Parse(gameDataJson[i]["resetCount"].ToString());

    //                    userPetData.petLevel = int.Parse(gameDataJson[i]["petLevel"].ToString());
    //                    userPetData.petNowExp = int.Parse(gameDataJson[i]["petNowExp"].ToString());
    //                    userPetData.petNextExp = int.Parse(gameDataJson[i]["petNextExp"].ToString());

    //                    userPetData.addVit = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addVit"].ToString());
    //                    userPetData.addAtk = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addAtk"].ToString());
    //                    userPetData.addDef = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addDef"].ToString());
    //                    userPetData.addDex = (STAT_COE_RANK)int.Parse(gameDataJson[i]["addDex"].ToString());

    //                    userPetData.petVit = float.Parse(gameDataJson[i]["petVit"].ToString());
    //                    userPetData.petAtk = float.Parse(gameDataJson[i]["petAtk"].ToString());
    //                    userPetData.petDef = float.Parse(gameDataJson[i]["petDef"].ToString());
    //                    userPetData.petDex = float.Parse(gameDataJson[i]["petDex"].ToString());

    //                    userPetData.petBaseVit = float.Parse(gameDataJson[i]["petBaseVit"].ToString());
    //                    userPetData.petBaseAtk = float.Parse(gameDataJson[i]["petBaseAtk"].ToString());
    //                    userPetData.petBaseDef = float.Parse(gameDataJson[i]["petBaseDef"].ToString());
    //                    userPetData.petBaseDex = float.Parse(gameDataJson[i]["petBaseDex"].ToString());

    //                    for (int j = 0; j < gameDataJson[i]["elementStat"].Count; j++)
    //                    {
    //                        userPetData.elementStat[j] = int.Parse(gameDataJson[i]["elementStat"][j].ToString());
    //                    }

    //                    userPetDataList.Add(new PetInfoDB(inData, new PetInfo(userPetData)));
    //                    Debug.Log($"{userPetData.petName}/{userPetData.resetCount}");
    //                }

    //                Debug.LogWarning("데이터 입력 완료.");

    //            }

    //            if (bro.HasFirstKey() == false)
    //                break;

    //            var firstKey = bro.FirstKeystring();

    //            // 다음 데이터 조회
    //            bro = Backend.GameData.GetMyData("USER_PET_DATA", new Where(), 100, firstKey);

    //            if (bro.IsSuccess() == false)
    //            {
    //                // 실패 처리
    //                return;
    //            }
    //        }
    //        USER_PET_DB = userPetDataList;
    //    }
    //    else
    //    {
    //        Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
    //    }
    //}
    //public void GameDataUpdate()
    //{
    //    if (selectPet == null)
    //    {
    //        Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
    //        return;
    //    }
    //    PetInfo petData = selectPet.petInfo;
    //    Param param = new Param();
    //    param.Add("petName", petData.petDefault.petName);
    //    param.Add("petNo", petData.petNo);
    //    param.Add("petRank", petData.petRank);
    //    param.Add("isEnemy", petData.isEnemy);
    //    param.Add("resetCount", petData.resetCount);
    //    param.Add("petLevel", petData.petLevel);
    //    param.Add("petNowExp", petData.petNowExp);
    //    param.Add("petNextExp", petData.petNextExp);
    //    param.Add("addVit", (int)petData.addVit);
    //    param.Add("addAtk", (int)petData.addAtk);
    //    param.Add("addDef", (int)petData.addDef);
    //    param.Add("addDex", (int)petData.addDex);
    //    param.Add("petVit", petData.petVit);
    //    param.Add("petAtk", petData.petAtk);
    //    param.Add("petDef", petData.petDef);
    //    param.Add("petDex", petData.petDex);
    //    param.Add("petBaseVit", petData.petBaseVit);
    //    param.Add("petBaseAtk", petData.petBaseAtk);
    //    param.Add("petBaseDef", petData.petBaseDef);
    //    param.Add("petBaseDex", petData.petBaseDex);
    //    param.Add("elementStat", petData.elementStat);

    //    BackendReturnObject bro = null;

    //    if (string.IsNullOrEmpty(selectPet.inData))
    //    {
    //        Debug.Log("선택된 데이터에 정보가 없습니다.");

    //        return;
    //        //bro = Backend.GameData.Update("USER_PET_DATA", new Where(), param);
    //    }
    //    else
    //    {
    //        Debug.Log($"{selectPet.inData}의 게임정보 데이터 수정을 요청합니다.");

    //        bro = Backend.GameData.UpdateV2("USER_PET_DATA", selectPet.inData, Backend.UserInDate, param);
    //    }

    //    if (bro.IsSuccess())
    //    {
    //        Debug.Log("게임정보 데이터 수정에 성공했습니다. : " + bro);
    //    }
    //    else
    //    {
    //        Debug.LogError("게임정보 데이터 수정에 실패했습니다. : " + bro);
    //    }
    //}

    //public void GameDataDelete(string inDate)
    //{
    //    var bro = Backend.GameData.DeleteV2("USER_PET_DATA", inDate, Backend.UserInDate);
    //    if (bro.IsSuccess())
    //    {
    //        Debug.Log("게임정보 데이터 삭제에 성공했습니다. : " + bro);
    //    }
    //    else
    //    {
    //        Debug.LogError("게임정보 데이터 삭제에 실패했습니다. : " + bro);
    //    }
    //}
}