using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyBox : MonoBehaviour
{
    [SerializeField] TMP_Text levelText;
    [SerializeField] Image icon;

    public void Setup(int level, Sprite icon)
    {
        this.levelText.text = level.ToString();
        this.icon.sprite = icon;
    }
}
