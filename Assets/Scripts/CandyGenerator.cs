using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween 네임스페이스 추가

public class CandyGenerator : MonoBehaviour
{
    public GameObject candyPrefab;
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

    private void GenerateCandy()
    {
        Debug.Log($"사탕 생성 시작!");
        GameBoardCell[,] gameBoardCells = gameBoardManager.GetGameBoardCellsArray();
        for (int row = GameBoardManager.ROW_START_CREATE_AREA; row < GameBoardManager.TOTAL_ROW; row++)
        {
            for (int col = 0; col < GameBoardManager.TOTAL_COL; col++)
            {
                //Debug.Log($"candiesArray[{row}, {col}]:{candiesArray[row, col]}");
                if (gameBoardCells[row, col].GetCandyNumber() == 0)
                {
                    // 사탕 생성! 

                    // 생성 정보 등록(데이터상)
                    gameBoardCells[row, col].SetCandyNumber(1); // 추후 랜덤생성은 이 값 수정
                    GameObject newCandy = Instantiate(candyPrefab, gameBoardCells[row, col].GetRectTransform());
                    gameBoardCells[row, col].SetCandyObject(newCandy);

                    // 생성 정보 등록(게임월드)

                    
                }
            }
        }
    }
}
