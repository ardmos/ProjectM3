using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupScorePanelController : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public GameObject[] StarObjects = new GameObject[3];
    public TextMeshProUGUI ScoreText;
    public ScoreManager ScoreManager;
    public CustomClickSoundButton CloseButton; // 스테이지 선택 씬으로 이동
    public CustomClickSoundButton RetryButton; // 현 스테이지 재시작
    public CustomClickSoundButton NextButton; // 다음 스테이지

    private void Start()
    {
        GameManager.Instance.OnWin += GameState_OnWin;
        
        InitPopup();
        Hide();
    }

    private void InitPopup()
    {
        TitleText.text = $"Level{LevelData.Instance.Level}";
        ScoreText.text = "0";

        CloseButton.AddClickListener(() => { LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene); });
        RetryButton.AddClickListener(() => { LoadSceneManager.Instance.Load((LoadSceneManager.Scene)SceneManager.GetActiveScene().buildIndex); });
        NextButton.AddClickListener(() => { LoadSceneManager.Instance.Load((LoadSceneManager.Scene)SceneManager.GetActiveScene().buildIndex + 1); }); // 다음 씬으로 이동
    }

    private void GameState_OnWin()
    {
        gameObject.SetActive(true);
        ScoreText.text = ScoreManager.GetCurrentScore().ToString();
        int starScore = ScoreManager.StarScore;
        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(i < starScore);
        }

        if (NextSceneExists())
        {
            NextButton.gameObject.SetActive(starScore >= 3);
            NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
            NextButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60;
        }
        else
        {
            NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Coming Soon";
            NextButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40;
        }      
    }

    private bool NextSceneExists()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = currentBuildIndex + 1;

        return nextBuildIndex < SceneManager.sceneCountInBuildSettings;
    }


    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
