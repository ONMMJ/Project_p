using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleStartUI : BaseUI
{
    [SerializeField] TMP_Text stageName;
    [SerializeField] EnemyBox enemyBoxPrefab;
    [SerializeField] Button startButton;

    [Header("Parents")]
    [SerializeField] Transform enemyContent;
    [SerializeField] Transform rewardContent;

    List<EnemyBox> enemyBoxes = new();

    protected override void EnableUI()
    {
        stageName.text = GameManager.Instance.stageFullName;
    }

    public void SetMission()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => {
            UIManager.Instance.StartBattle();
            SceneManager.LoadScene("Mission");
        });

        if (enemyBoxes.Count > 0)
        {
            foreach (EnemyBox box in enemyBoxes)
            {
                Destroy(box.gameObject);
            }
            enemyBoxes = new();
        }

        foreach (MissionPet missionEnemy in GameManager.Instance.missionData.MissionEnemyList)
        {
            EnemyBox enemyBox = Instantiate(enemyBoxPrefab, enemyContent);
            enemyBox.Setup(missionEnemy.petLevel, BackendManager.Instance.Chart.PetChart.GetSpriteIcon(missionEnemy.petName));
            enemyBoxes.Add(enemyBox);
        }
    }

    public void Setup(string stageId)
    {
        bool isClear = BackendManager.Instance.GameData.StageData.IsClearStage(stageId);

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => {
            if (BackendManager.Instance.GameData.UserData.selectPets.Any(x => x == "" || x == null))
            {
                UIManager.Instance.OpenInvenUI(LEFT_BUTTON_PANEL.Main);
            }
            else
            {
                UIManager.Instance.StartBattle();
                SceneManager.LoadScene(BackendManager.Instance.Chart.StageChart.GetSceneName(stageId));
            }
        });

        if (enemyBoxes.Count > 0)
        {
            foreach(EnemyBox box in enemyBoxes)
            {
                Destroy(box.gameObject);
            }
            enemyBoxes = new();
        }
        if (!isClear)
        {
            // Stage
            List<EnemyInfo> sEnemeyInfoList = BackendManager.Instance.Chart.EnemyChart.StageEnemyDictionary[stageId];
            foreach (EnemyInfo enemyInfo in sEnemeyInfoList)
            {
                EnemyBox enemyBox = Instantiate(enemyBoxPrefab, enemyContent);
                enemyBox.Setup(enemyInfo.petLevel, BackendManager.Instance.Chart.PetChart.GetSpriteIcon(enemyInfo.petName));
                enemyBoxes.Add(enemyBox);
                // 보상 추가
            }
        }
        else
        {
            // Ground
            List<EnemyInfo> gEnemeyInfoList = BackendManager.Instance.Chart.EnemyChart.GroundEnemyDictionary[stageId];
            foreach (EnemyInfo enemyInfo in gEnemeyInfoList)
            {
                EnemyBox enemyBox = Instantiate(enemyBoxPrefab, enemyContent);
                enemyBox.Setup(enemyInfo.petLevel, BackendManager.Instance.Chart.PetChart.GetSpriteIcon(enemyInfo.petName));
                enemyBoxes.Add(enemyBox);
                // 보상 추가
            }
        }
    }

}
