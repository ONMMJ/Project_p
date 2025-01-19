using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorPopupScript : MonoBehaviour
{
    [SerializeField] TMP_Text errorMessage;

    public void SetErrorMessage(string errorMsg)
    {
        errorMessage.text = errorMsg;
    }
}
