using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween ���ӽ����̽� �߰�

public class CandyGenerator : MonoBehaviour
{
    public const int CANDY_TYPES_COUNT_STAGE1 = 5;

    public GameObject[] candyPrefab;
    public GameBoardManager gameBoardManager;

    private void Awake()
    {
        gameBoardManager.OnNeedCandy += GameBoardManager_OnNeedCandy;
    }

    private void GameBoardManager_OnNeedCandy()
    {
        GenerateCandy();
    }

    // ���� ���� ������ ����� ������ ������ �����մϴ�.
    private void GenerateCandy()
    {
        Debug.Log($"���� ���� ����!");
        GameBoardCell[,] gameBoardCells = gameBoardManager.GetGameBoardCellsArray();
        for (int row = GameBoardManager.ROW_START_CREATE_AREA; row < GameBoardManager.TOTAL_ROW; row++)
        {
            for (int col = 0; col < GameBoardManager.TOTAL_COL; col++)
            {
                // �� �� �߰�!
                if (gameBoardCells[row, col].GetCandyNumber() == 0)
                {
                    // ���� ����! 
                    int ranNum = Random.Range(1, CANDY_TYPES_COUNT_STAGE1); // ��������1 ����. 5���� ĵ�� ����
                    gameBoardCells[row, col].SetCandyNumber(ranNum);
                    GameObject newCandy = Instantiate(candyPrefab[ranNum], gameBoardCells[row, col].GetRectTransform());
                    gameBoardCells[row, col].SetCandyObject(newCandy.GetComponent<Candy>());
                }
            }
        }
    }
}
