using TMPro;
using UnityEngine;

/// <summary>
/// 스테이지 선택 씬에서 각 스테이지를 선택하면 상세 정보를 보여주는 팝업입니다.
/// </summary>
public class PopupLevelPanelController : MonoBehaviour
{
    public TextMeshProUGUI TitleTextUI;
    public TextMeshProUGUI TargetScoreTextUI;
    public CustomClickSoundButton CloseButton;
    public CustomClickSoundButton PlayButton;
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

        CloseButton.AddClickListener(() => {
            Hide();
        });
        PlayButton.AddClickListener(() =>
        {
            LoadSceneManager.Scene targetScene = (LoadSceneManager.Scene)((int)LoadSceneManager.Scene.Level1 + stageLevel - 1);
            LoadSceneManager.Instance.Load(targetScene);
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
