using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public const int TARGET_SCORE_STAGE1 = 150;

    [SerializeField] private int targetScore = 0;

    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private int currentScore = 0;

    private void Start()
    {
        targetScore = TARGET_SCORE_STAGE1;
    }

    public void AddCurrentScore(int value)
    {
        Debug.Log($"value:{value}");
        currentScore += value;
        currentScoreText.text = $"{currentScore}";

        CheckScore();
    }

    private void CheckScore()
    {
        if (currentScore >= targetScore)
        {
            // 스테이지 클리어!
            GameManager.Instance.UpdateState(GameManager.State.Win);
        }
    }

    public int GetCurrentScore() => currentScore;
}
