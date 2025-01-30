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
            StartCoroutine(StageLose());
        }
    }

    private void UpdateMoveCountTextUI(int moveCount)
    {
        moveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";
    }

    private IEnumerator StageLose()
    {
        yield return new WaitForSeconds(1f);
        // 스테이지 실패!
        GameManager.Instance.UpdateState(GameManager.State.Lose);
    }
}
