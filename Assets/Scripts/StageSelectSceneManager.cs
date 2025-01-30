using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

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

    // Stage List 초기화
    private void InitStageListPanel()
    {
        var PlayerData = PlayerDataManager.Instance.PlayerData;
        var sortedDict = PlayerData.StageScorePairs.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        // 깨본적 있는 스테이지들
        foreach (var stageScorePair in PlayerData.StageScorePairs)
        {
            int stage = stageScorePair.Key;
            int score = stageScorePair.Value;

            StagePanel stagePanel = Instantiate(StagePanelPrefab, Grid).GetComponent<StagePanel>();
            stagePanel.InitPanel(stage, StageImages[(stage - 1 + StageImages.Length) % StageImages.Length], $"Level {stage}", score);
        }

 

        // 새로 열리는 스테이지
        int nextLevel = sortedDict.Keys.LastOrDefault()+1;
        if(nextLevel >= (int)LoadSceneManager.Scene.Max-1)
        {
            return;
        }

        int lastScore = sortedDict.Values.LastOrDefault();
        if (lastScore >= 3)
        {         
            StagePanel stagePanel = Instantiate(StagePanelPrefab, Grid).GetComponent<StagePanel>();
            stagePanel.InitPanel(nextLevel, StageImages[(nextLevel - 1 + StageImages.Length) % StageImages.Length], $"Level {nextLevel}", 0);
        }
    }

    private void InitButtons()
    {
        ExitButton.AddClickListener(()=> LoadSceneManager.Instance.Load(LoadSceneManager.Scene.TitleScene));
        SettingsButton.AddClickListener(() => {
            // Settings 팝업 활성화
            PopupSettingsPanelController.Show();
        });
    }
}
