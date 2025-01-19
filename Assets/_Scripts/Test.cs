using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] InputField inputField;
    public void GetGold()
    {
        BackendManager.Instance.GameData.UserData.AddGold(100000000);
    }
    public void Coupon()
    {
        var bro = Backend.Coupon.UseCoupon(inputField.text);

        if (!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return;
        }
    }
}
