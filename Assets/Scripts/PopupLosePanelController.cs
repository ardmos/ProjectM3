using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupLosePanelController : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI ScoreText;
    public ScoreManager ScoreManager;
    public CustomClickSoundButton CloseButton; // 스테이지 선택 씬으로 이동
    public CustomClickSoundButton RetryButton; // 현 스테이지 재시작

    private void Start()
    {
        GameManager.Instance.OnLose += GameState_OnLose;

        InitPopup();
        Hide();
    }

    private void InitPopup()
    {
        TitleText.text = $"Level{LevelData.Instance.Level}";
        ScoreText.text = "0";

        CloseButton.AddClickListener(() => { LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene); });
        RetryButton.AddClickListener(() => { LoadSceneManager.Instance.Load((LoadSceneManager.Scene)SceneManager.GetActiveScene().buildIndex); });
    }

    private void GameState_OnLose()
    {
        gameObject.SetActive(true);
        ScoreText.text = ScoreManager.GetCurrentScore().ToString();
        int starScore = ScoreManager.StarScore;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
