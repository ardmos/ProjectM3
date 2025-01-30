using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class StageSelectSceneManager : MonoBehaviour
{
    public static StageSelectSceneManager Instance;

    public Sprite[] StageImages = new Sprite[4];
    public GameObject StagePanelPrefab;
    public RectTransform Grid;
    public CustomClickSoundButton ExitButton;
    public CustomClickSoundButton SettingsButton;
    public PopupSettingsPanelController PopupSettingsPanelController;
    public PopupLevelPanelController PopupLevelPanelController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitStageListPanel();
        InitButtons();
    }

    // Stage List �ʱ�ȭ
    private void InitStageListPanel()
    {
        var PlayerData = PlayerDataManager.Instance.PlayerData;
        var sortedDict = PlayerData.StageScorePairs.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        


        // ������ �ִ� ����������
        foreach (var stageScorePair in PlayerData.StageScorePairs)
        {
            int stage = stageScorePair.Key;
            int score = stageScorePair.Value;

            StagePanel stagePanel = Instantiate(StagePanelPrefab, Grid).GetComponent<StagePanel>();
            stagePanel.InitPanel(stage, StageImages[stage-1], $"Level {stage}", score);
        }

        // ���� ������ ��������
        int nextLevel = sortedDict.Keys.LastOrDefault()+1;
        int lastScore = sortedDict.Values.LastOrDefault();
        if (lastScore >= 2)
        {         
            StagePanel stagePanel = Instantiate(StagePanelPrefab, Grid).GetComponent<StagePanel>();
            stagePanel.InitPanel(nextLevel, StageImages[nextLevel%StageImages.Length], $"Level {nextLevel}", 0);
        }
    }

    private void InitButtons()
    {
        ExitButton.AddClickListener(()=> LoadSceneManager.Instance.Load(LoadSceneManager.Scene.TitleScene));
        SettingsButton.AddClickListener(() => {
            // Settings �˾� Ȱ��ȭ
            PopupSettingsPanelController.Show();
        });
    }
}
