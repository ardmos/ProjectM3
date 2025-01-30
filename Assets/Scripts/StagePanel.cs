using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    public TextMeshProUGUI StageLevelTextUI;
    public Image StageImageUI;
    public TextMeshProUGUI StageNameTextUI;
    public GameObject[] StarObjects = new GameObject[3];
    public Button StageButton;

    public void InitPanel(int stageLevel, Sprite stageImage, string stageName, int starScore)
    {
        StageLevelTextUI.text = stageLevel.ToString();
        StageImageUI.sprite = stageImage;
        StageNameTextUI.text = stageName;

        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(i < starScore);
        }


        //
        // 레벨 팝업이 열린 다음 씬 이동하도록!
        StageButton.onClick.AddListener(() => {
            StageSelectSceneManager.Instance.PopupLevelPanelController.Show();
            StageSelectSceneManager.Instance.PopupLevelPanelController.InitPopup(stageLevel, stageName, starScore);
        });
    }
}
