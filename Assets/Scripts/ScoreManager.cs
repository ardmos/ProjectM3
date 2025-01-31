using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 게임씬에서 플레이어의 스코어 계산을 담당하는 매니저입니다.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI CurrentScoreText;
    public GameObject[] StarObjects = new GameObject[3];
    public int StarScore = 0;
    [SerializeField] private int currentScore = 0;

    private void Start()
    {
        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(false);
        }
    }

    /// <summary>
    /// 스코어 추가
    /// </summary>
    public void AddScore(int value)
    {
        currentScore += value;
        CheckStarScore();
    }
    public void AddScore(List<Candy> poppedCandies)
    {
        foreach (Candy poppedCandy in poppedCandies)
        {
            currentScore += poppedCandy.CandyScore;
        }

        CurrentScoreText.text = $"{currentScore}";

        CheckStarScore();
    }

    /// <summary>
    /// 획득한 점수를 기반으로 획득 가능한 별 점수를 계산해주는 메서드입니다.
    /// </summary>
    private void CheckStarScore()
    {
        int[] targetScores = LevelData.Instance.TargetScore;
        int starsEarned = 0;

        for (int i = 0; i < targetScores.Length; i++)
        {
            if (currentScore >= targetScores[i])
            {
                starsEarned = i + 1;
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(i < starsEarned);
        }

        StarScore = starsEarned;
    }

    public int GetCurrentScore() => currentScore;
}
