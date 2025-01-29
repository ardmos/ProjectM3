using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveCountManager : MonoBehaviour
{
    public TextMeshProUGUI moveCountText;
    public int moveCount;

    private void Start()
    {
        moveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";
    }

    public void IncreaseMoveCount()
    {
        moveCount++;
        moveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";

        if(moveCount >= LevelData.Instance.MoveMaxCount)
        {
            GameManager.Instance.SetGameState(GameManager.State.Lose);
        }
    }
}
