using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum LEFT_BUTTON_PANEL
{
    Info,
    Main,
}

public class LeftButton : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] Button infoButton;
    [SerializeField] Button mainPetButton;

    [Header("Panel")]
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject mainPetPanel;

    List<GameObject> panelList;
    // Start is called before the first frame update
    public void Setup()
    {
        panelList = new();

        panelList.Add(infoPanel);
        panelList.Add(mainPetPanel);

        infoButton.onClick.AddListener(() => ActivePanel(infoPanel));
        mainPetButton.onClick.AddListener(() => ActivePanel(mainPetPanel));
    }

    //private void OnEnable()
    //{
    //    if (panelList == null)
    //        return;
    //        ActivePanel(infoPanel);
    //}

    private void AllOffPanel()
    {
        foreach(GameObject panel in panelList)
        {
            panel.SetActive(false);
        }
    }

    private void ActivePanel(GameObject panel)
    {
        AllOffPanel();
        panel.SetActive(true);
    }

    public void ActivePanel(LEFT_BUTTON_PANEL panel)
    {
        switch (panel)
        {
            case LEFT_BUTTON_PANEL.Info:
                ActivePanel(infoPanel);
                break;
            case LEFT_BUTTON_PANEL.Main:
                ActivePanel(mainPetPanel);
                break;
            default:
                break;
        }
        
    }
}
