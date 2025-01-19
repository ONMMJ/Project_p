using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using TMPro;
using System;

public class Hash : MonoBehaviour
{
    [SerializeField] TMP_InputField text;
    public void GetHash()
    {
        try
        {
            string googlehash = Backend.Utils.GetGoogleHash();

            text.text = googlehash;
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

}
