//using System.Collections;
//using System.Collections.Generic;
//using BackEnd;
//using LitJson;
//using UnityEngine;

//public class BackendChartManager
//{
//    private static BackendChartManager _instance = null;

//    public static BackendChartManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = new BackendChartManager();
//            }

//            return _instance;
//        }
//    }

//    public static Dictionary<string, PetDefault> PET_DB;

//    public void PetDataGet()
//    {
//        PET_DB = new Dictionary<string, PetDefault>();

//        var bro = Backend.Chart.GetChartContents("91772");

//        if (bro.IsSuccess() == false)
//        {
//            return;
//        }

//        foreach (JsonData petData in bro.FlattenRows())
//        {
//            PetDefault petDefault = new PetDefault();
//            int[] elementStat = new int[6];
//            petDefault.petNo = int.Parse(petData["petNo"].ToString());
//            petDefault.petName = petData["petName"].ToString();
//            petDefault.petAssetName = petData["petAssetName"].ToString();
//            petDefault.petBaseCoe = float.Parse(petData["petBaseCoe"].ToString());
//            petDefault.petVitCoe = float.Parse(petData["petVitCoe"].ToString());
//            petDefault.petAtkCoe = float.Parse(petData["petAtkCoe"].ToString());
//            petDefault.petDefCoe = float.Parse(petData["petDefCoe"].ToString());
//            petDefault.petDexCoe = float.Parse(petData["petDexCoe"].ToString());
//            petDefault.skillMaxPoint = int.Parse(petData["skillMaxPoint"].ToString());

//            elementStat[0] = int.Parse(petData["Earth"].ToString());
//            elementStat[1] = int.Parse(petData["Water"].ToString());
//            elementStat[2] = int.Parse(petData["Fire"].ToString());
//            elementStat[3] = int.Parse(petData["Wind"].ToString());
//            elementStat[4] = int.Parse(petData["Light"].ToString());
//            elementStat[5] = int.Parse(petData["Dark"].ToString());
//            System.Array.Copy(elementStat, petDefault.elementStat, 6);
//            PET_DB.Add(petDefault.petName, petDefault);
//        }
//        Debug.Log("차트 업데이트 완료!");
//        foreach (string key in PET_DB.Keys)
//        {
//            Debug.Log($"{key} / {PET_DB[key].petName}");
//        }
//    }
//}