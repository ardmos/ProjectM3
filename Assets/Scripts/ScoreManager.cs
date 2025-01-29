using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI CurrentScoreText;
    public GameObject[] StarObjects = new GameObject[3];

    [SerializeField] private int currentScore = 0;

    private void Start()
    {
        AllStarsOff();

        GameBoardManager.Instance.OnPopped += AddScore;
    }

    private void AddScore(List<Candy> poppedCandies)
    {
        foreach (Candy poppedCandy in poppedCandies)
        {
            currentScore += poppedCandy.CandyScore;
        }

        CurrentScoreText.text = $"{currentScore}";

        CheckStarScore();
    }

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
    }

    private void AllStarsOff()
    {
        StarObjects[2].SetActive(false);
        StarObjects[1].SetActive(false);
        StarObjects[0].SetActive(false);
    }

    public int GetCurrentScore() => currentScore;
}
