using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween ���ӽ����̽� �߰�

public class CandyGenerator : MonoBehaviour
{
    public GameObject candyPrefab;
    public GameBoardManager gameBoardManager;
    public RectTransform areaCandyGenerate;
    public float dropDuration = 0.5f; // ĵ�� �������� �ð�

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
        Debug.Log($"���� ���� ����!");
        (int candyNumber, GameObject candyObject)[,] candiesArray = gameBoardManager.GetCandiesArray();
        for (int row = GameBoardManager.ROW_START_CREATE_AREA; row < GameBoardManager.TOTAL_ROW; row++)
        {
            for (int col = 0; col < GameBoardManager.TOTAL_COL; col++)
            {
                //Debug.Log($"candiesArray[{row}, {col}]:{candiesArray[row, col]}");
                if (candiesArray[row, col].candyNumber == 0)
                {
                    // ���� ����!
                    candiesArray[row, col].candyNumber = 1;

                    GameObject newCandy = Instantiate(candyPrefab, gameBoardManager.GetCellRectTransform(row, col));
                    candiesArray[row, col].candyObject = newCandy;
                    //Debug.Log($"���� ����! row:{row}, col:{col}");
                }
            }
        }
    }
}
