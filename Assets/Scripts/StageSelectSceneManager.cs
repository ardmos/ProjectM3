using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectSceneManager : MonoBehaviour
{
    public Sprite[] StageImages = new Sprite[4];
    public GameObject StagePanelPrefab;
    public RectTransform Grid;
    public Button ExitButton;

    private void Start()
    {
        InitStageListPanel();
        InitButtons();
    }

    // Stage List √ ±‚»≠
    private void InitStageListPanel()
    {
        var PlayerData = PlayerDataManager.Instance.PlayerData;

        foreach (var stageScorePair in PlayerData.StageScorePairs)
        {
            int stage = stageScorePair.Key;
            int score = stageScorePair.Value;

            StagePanel stagePanel = Instantiate(StagePanelPrefab, Grid).GetComponent<StagePanel>();
            stagePanel.InitPanel(stage, StageImages[stage-1], $"Level {stage}", score);
        }
    }

    private void InitButtons()
    {
        //ExitButton.onClick.AddListener();
    }
}
