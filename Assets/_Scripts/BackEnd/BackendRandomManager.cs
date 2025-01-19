//using System;
//using System.Collections;
//using System.Collections.Generic;
//using BackEnd;
//using LitJson;
//using UnityEngine;

//public class PetGrade
//{
//    public PetInfo petInfo;
//    public int grade;
//    public PetGrade(PetInfo petInfo, int grade)
//    {
//        this.petInfo = petInfo;
//        this.grade = grade;
//    }
//}

//public class PetPickupInfo
//{
//    public int num;
//    public string petName;
//    public int grade;
//    public string percent;
//    public override string ToString()
//    {
//        return $"Name: {petName} / grade: {grade} / per: {percent}";
//    }
//}

//public class BackendRandomManager
//{
//    private static BackendRandomManager _instance = null;

//    public static BackendRandomManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = new BackendRandomManager();
//            }

//            return _instance;
//        }
//    }

//    public PetGrade PetPickup_Normal()
//    {
//        string selectedProbabilityFileId = "8271";

//        var bro = Backend.Probability.GetProbability(selectedProbabilityFileId);

//        if (!bro.IsSuccess())
//        {
//            Debug.LogError(bro.ToString());
//            return null;
//        }

//        JsonData json = bro.GetFlattenJSON();

//        PetPickupInfo item = new PetPickupInfo();

//        item.petName = json["elements"]["petName"].ToString();
//        item.grade = int.Parse(json["elements"]["grade"].ToString());
//        item.num = int.Parse(json["elements"]["num"].ToString());
//        item.percent = json["elements"]["percent"].ToString();

//        return new PetGrade(PetManager.Instance.GetLv1Pet(item.petName), item.grade);
//        // BackendManager.Instance.GameData.UserPetData.AddPetData(PetManager.Instance.GetLv1Pet(item.petName));
//    }
//}