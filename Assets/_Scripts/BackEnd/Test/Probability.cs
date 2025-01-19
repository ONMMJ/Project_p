// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using UnityEngine;

//===============================================================
// 차트 불러오기에 대한 공통적인 로직을 가진 클래스
//===============================================================
public abstract class Probability<T>
{
    public delegate void FinalFuntion<Template>(Template a);
    // Backend.Chart.GetChartContents 호출 시 리턴되는 json(Flatten)을 처리하는 함수
    // 각 자식 객체에서 형태에 맞게 설정해야한다.
    protected abstract T LoadProbabilityDataTemplate(JsonData json);
    public abstract string GetProbabilityFileName(); // 각 자식 객체가 설정한 차트 이름을 불러오는 함수

    //차트에 등록된 이미지 경로를 찾아 이미지를 반환하는 함수
    protected Sprite AddOrGetImageDictionary(Dictionary<string, Sprite> imageDictionary, string imagePath, string imageName)
    {
        if (imageDictionary.ContainsKey(imageName))
        {
            return imageDictionary[imageName];
        }

        imagePath += imageName;
        var sprite = Resources.Load<Sprite>(imagePath);

        if (sprite == null)
        {
            Debug.LogWarning("이미지를 찾지 못했습니다. in " + imageName);
            return null;
        }
        imageDictionary.Add(imageName, sprite);
        return imageDictionary[imageName];
    }

    // Base가 부모인 객체에서 공통적으로 사용되는 뒤끝 차트 로딩 함수
    // 차트 이름에 맞는 함수를 로드하고 LoadChartDataTemplate로 각자의 객체에서 사용 가능하게끔 파싱해준다.
    public void BackendProbabilityDataLoad(FinalFuntion<T> FinalFuntion)
    {

        string probabilityFileName = GetProbabilityFileName();
        string className = GetType().Name;
        bool isSuccess = false;
        string errorInfo = string.Empty;
        string funcName = MethodBase.GetCurrentMethod()?.Name;

        string probabilityId = string.Empty;
        if (!BackendManager.Instance.Probability.ProbabilityInfo.Dictionary.ContainsKey(probabilityFileName))
        {
            Debug.LogError($"차트에 {probabilityFileName}에 대한 정보가 존재하지 않습니다.");
            return;
        }

        probabilityId = BackendManager.Instance.Probability.ProbabilityInfo.Dictionary[probabilityFileName];

        // [뒤끝] 차트 불러오기 함수
        SendQueue.Enqueue(Backend.Probability.GetProbability, probabilityId, callback =>
        {
            try
            {
                if (callback.IsSuccess())
                {
                    JsonData json = callback.GetFlattenJSON();

                    T template = LoadProbabilityDataTemplate(json); // 각 자식 객체가 설정한 리턴값 파싱 함수
                    FinalFuntion(template);
                    isSuccess = true;
                    Debug.Log($"{probabilityFileName}뽑기: {isSuccess}\n {callback}");
                }
                else
                {
                    errorInfo = callback.ToString();
                    Debug.LogError($"{probabilityFileName}뽑기 실패: {errorInfo}");
                }
            }
            catch (Exception e)
            {
                errorInfo = e.ToString();
                Debug.LogError($"{probabilityFileName}뽑기 실패: {errorInfo}");
            }
        });
    }

}