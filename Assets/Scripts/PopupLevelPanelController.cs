using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevelPanelController : MonoBehaviour
{
    public TextMeshProUGUI TitleTextUI;
    public TextMeshProUGUI TargetScoreTextUI;
    public Button CloseButton;
    public Button PlayButton;
    public GameObject[] StarObjects = new GameObject[3];

    private void Start()
    {
        Hide();
    }

    public void InitPopup(int stageLevel, string stageName, int starScore)
    {
        TitleTextUI.text = stageName;
        TargetScoreTextUI.text = $"{stageLevel*200}";

        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(i < starScore);
        }

        CloseButton.onClick.AddListener(() => {
            Hide();
        });
        PlayButton.onClick.AddListener(() =>
        {
            LoadSceneManager.Scene targetScene = (LoadSceneManager.Scene)((int)LoadSceneManager.Scene.Level1 + stageLevel - 1);
            LoadSceneManager.Load(targetScene);
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
