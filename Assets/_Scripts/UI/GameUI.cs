using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DamageNumbersPro;

public enum DamageTpye
{
    Normal,
    Dot,
    Heal,
}

public class GameUI : Singleton<GameUI>
{
    [Header("Battle UI")]
    [SerializeField] StatusBar statusBarPrefab;
    [SerializeField] DamageNumber damagePrefab;
    [SerializeField] DamageNumber missPrefab;
    [SerializeField] DamageNumber dotDamagePrefab;
    [SerializeField] DamageNumber healPrefab;
    [SerializeField] Image targetArrowPrefab;
    [SerializeField] Button attackTargetPrefab;

    [Header("Etc UI")]
    [SerializeField] MouseManualBar mouseMenualBar;

    public StatusBar GetStatusBar(Transform parent)
    {
        return Instantiate(statusBarPrefab, parent);
    }
    public Image GetTargetArrow(Transform parent)
    {
        return Instantiate(targetArrowPrefab, parent);
    }
    public Button GetAttackTargetImage(Transform parent)
    {
        return Instantiate(attackTargetPrefab, parent);
    }

    public DamageNumber GetDamageNumber(DamageTpye type, Vector3 position, int dmg)
    {
        DamageNumber number;
        // Miss UI는 공용으로 사용
        if (dmg <= 0)
        {
            number = missPrefab.Spawn(position);
        }
        else
        {
            switch (type)
            {
                case DamageTpye.Normal:
                    number = damagePrefab.Spawn(position, dmg);
                    break;
                case DamageTpye.Dot:
                    number = dotDamagePrefab.Spawn(position, dmg);
                    break;
                case DamageTpye.Heal:
                    number = healPrefab.Spawn(position, dmg);
                    break;
                default:
                    number = damagePrefab.Spawn(position, dmg); 
                    break;
            }
        }
        number.lifetime *= GameManager.Instance.gameSpeed;
        return number;
    }

    public DamageNumber GetDamageNumber(Vector3 position, int dmg, Transform parent)
    {
        DamageNumber damageNumber = damagePrefab.Spawn(position, dmg);
        damageNumber.transform.parent = parent;
        damageNumber.lifetime *= GameManager.Instance.gameSpeed;
        return damageNumber;
    }
    public DamageNumber GetDamageNumber(Vector3 position, int dmg)
    {
        DamageNumber damageNumber = damagePrefab.Spawn(position, dmg);
        damageNumber.lifetime *= GameManager.Instance.gameSpeed;
        return damageNumber;
    }
    public DamageNumber GetMissNumber(Vector3 position)
    {
        DamageNumber missNumber = missPrefab.Spawn(position);
        missNumber.lifetime *= GameManager.Instance.gameSpeed;
        return missNumber;
    }
    public DamageNumber GetDotDamageNumber(Vector3 position, int dmg)
    {
        DamageNumber damageNumber = dotDamagePrefab.Spawn(position, dmg);
        damageNumber.lifetime *= GameManager.Instance.gameSpeed;
        return damageNumber;
    }
    public DamageNumber GetHealNumber(Vector3 position, int heal)
    {
        DamageNumber healNumber = healPrefab.Spawn(position, heal);
        healNumber.lifetime *= GameManager.Instance.gameSpeed;
        return healNumber;
    }



    public void manualOn(string text)
    {
        mouseMenualBar.ManualOn(text);
    }
    public void manualOff()
    {
        mouseMenualBar.ManualOff();
    }
}
