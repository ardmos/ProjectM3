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
        UpdateMoveCountTextUI(moveCount);

        GameBoardManager.Instance.OnMoved += IncreaseMoveCount;
    }

    private void IncreaseMoveCount()
    {
        moveCount++;
        UpdateMoveCountTextUI(moveCount);

        if (moveCount >= LevelData.Instance.MoveMaxCount)
        {
            GameManager.Instance.UpdateState(GameManager.State.Lose);
        }
    }

    private void UpdateMoveCountTextUI(int moveCount)
    {
        moveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";
    }
}
