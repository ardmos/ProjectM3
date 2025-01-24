using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween 네임스페이스 추가

public class CandyGenerator : MonoBehaviour
{
    public GameObject[] candyPrefab;
    public GameBoardManager gameBoardManager;
    public RectTransform areaCandyGenerate;
    public float dropDuration = 0.5f; // 캔디가 떨어지는 시간

    private void Awake()
    {
        gameBoardManager.OnNeedCandy += GameBoardManager_OnNeedCandy;
    }

    private void GameBoardManager_OnNeedCandy()
    {
        GenerateCandy();
    }

    // 사탕 생성 영역에 빈곳이 있으면 사탕을 생성합니다.
    private void GenerateCandy()
    {
        Debug.Log($"사탕 생성 시작!");
        GameBoardCell[,] gameBoardCells = gameBoardManager.GetGameBoardCellsArray();
        for (int row = GameBoardManager.ROW_START_CREATE_AREA; row < GameBoardManager.TOTAL_ROW; row++)
        {
            for (int col = 0; col < GameBoardManager.TOTAL_COL; col++)
            {
                // 빈 곳 발견!
                if (gameBoardCells[row, col].GetCandyNumber() == 0)
                {
                    // 사탕 생성! 
                    //int ranNum = Random.Range(1, candyPrefab.Length);
                    int ranNum = Random.Range(1, 4);
                    gameBoardCells[row, col].SetCandyNumber(ranNum);
                    GameObject newCandy = Instantiate(candyPrefab[ranNum], gameBoardCells[row, col].GetRectTransform());
                    gameBoardCells[row, col].SetCandyObject(newCandy.GetComponent<Candy>());
                }
            }
        }
    }
}
