using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : BaseUI
{
    [SerializeField] Button couponButton;

    protected override void EnableUI()
    {

    }

    private void Start()
    {
        SetButtonListener();
    }

    private void SetButtonListener()
    {
        couponButton.onClick.AddListener(CouponButton);
    }

    private void CouponButton()
    {
        string url = "https://storage.thebackend.io/1ea3f14d34e89530ea88b3245bc82dc17d5f52ce1554049f19fce9219a847cfce18bb889159dfac6b160e8b9a3bdd68c65c5513c1dd200af980a58a6bd86a0ec453f3e7a253d8248e09b7847/coupon.html?lng=ko";
        string uid = $"&uid={Backend.UID}";
        Application.OpenURL(url + uid);
    }

}
