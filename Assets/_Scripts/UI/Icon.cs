using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField] Image image;

    public void SetIcon(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
