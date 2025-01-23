using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �迭�� �� �Ӽ���. 
/// 0: ���
/// 1: ĵ��
/// </summary>
public class GameBoardManager : MonoBehaviour
{
    public const int TOTAL_ROW = 6;    // TOTAL_ROW�� �׻� ¦��. �׻� ���� ������ ���� ���� ROW�� ���� ���� ������ �����ϱ� ����.
    public const int TOTAL_COL = 3;    // ���簢�� �����̱� ������ COL�� TOTAL_COL�� �����ص� ���.
    public const int ROW_END_CREATE_AREA = TOTAL_ROW - 1;
    public const int ROW_START_CREATE_AREA = TOTAL_ROW / 2;
    public const int ROW_END_GAME_AREA = ROW_START_CREATE_AREA - 1;
    public const int ROW_START_GAME_AREA = 0;

    public event Action OnNeedCandy;

    // �����ͻ� �����ϴ� ĵ�� ��ġ ���� �迭
    private (int candyNumber, GameObject candyObject)[,] candiesArray = new (int, GameObject)[TOTAL_ROW, TOTAL_COL];
    // ���ӿ���� �����ϴ� ĵ�� ��ġ �迭
    private Text[,] cells = new Text[TOTAL_ROW, TOTAL_COL];
    public GameObject cellsParent;

    private void Start()
    {
        // GameObject[,] candiesObjectsArray �ʱ�ȭ
        InitCandiesObjectsArray();

        PrintCandyArray();
        CreateCandies();
        StartCoroutine(DropCandies());
    }

    /// <summary>
    /// �׽�Ʈ�� ����Դϴ�.
    /// </summary>
    private void InitCandiesObjectsArray()
    {
        //candiesParentsObjects
        var childObjects = cellsParent.GetComponentsInChildren<Text>();
        int childArrayindex = 0;
        for (int row = TOTAL_ROW-1; row >= 0; row--)
        {
            for (int col = 0; col < TOTAL_COL; col++)
            {     
                cells[row, col] = childObjects[childArrayindex];
                childArrayindex++;
            }
        }
    }

    // 0. ��� ���� ���� _ �˻� �� ����. ���� 0�� ���� ������ ���� ����
    private void CreateCandies()
    {
        OnNeedCandy.Invoke();

        PrintCandyArray();
    }

    // 1. �ϴ� ���� ���� _ �˻� �� ��ġ �̵�. ���� 0�� ���� �ش� ���� ��� ��ҵ� �� ��, ���� ����� 0�� �ƴ� ���� ��ġ ��ȯ
    private IEnumerator DropCandies()
    {
        Debug.Log($"���� ��� ����!");
        for (int row=ROW_START_GAME_AREA; row<=ROW_END_GAME_AREA; row++)
        {
            for(int col=0; col<TOTAL_COL; col++)
            {
                if (candiesArray[row, col].candyNumber == 0)
                {
                    // ��ĭ! �ش� ���� ��� ��ҵ� �� ���� ����� 0�� �ƴ� ���� ��ġ�� ��ȯ�մϴ�.
                    var result = FindCandyInColumn(row, col);
                    if (result.success == false) continue;  // �˻� ����. �ش� ���� ������ ��� ��ĭ�Դϴ�. ���� ���� �˻��� �Ѿ�ϴ�.

                    (int row, int col) closestCandyPos = result.pos;

                    // �� ��ȯ
                    //Debug.Log($"��ĭ! �ش� ���� ��� ��ҵ� �� ���� ����� 0�� �ƴ� ���� ��ġ�� ��ȯ�մϴ�. ({row},{col}):{candiesArray[row, col]} <-> ({closestCandyPos.row},{closestCandyPos.col}):{candiesArray[closestCandyPos.row, closestCandyPos.col]}");
                    yield return new WaitForSeconds(0.2f);
                    // ��ġ ����
                    (candiesArray[row, col], candiesArray[closestCandyPos.row, closestCandyPos.col]) = (candiesArray[closestCandyPos.row, closestCandyPos.col], candiesArray[row, col]);
                    //Debug.Log($"��ȯ ���. ({row},{col}):{candiesArray[row, col]} / ({closestCandyPos.row},{closestCandyPos.col}):{candiesArray[closestCandyPos.row, closestCandyPos.col]}");
                    PrintCandyArray();
                }
            }
        }
        
        // DropCandies�� ��� �۾��� �Ϸ�Ǹ� ���� �۾� ����
        CreateCandies();
    }

    /// <summary>
    /// startRow�� ���� TOTAL_ROW���� row++ �� �ش� col�� ������ �˻�. ���� ���� �߰ߵǴ� 0�� �ƴ� ���� ��ġ�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="col"></param>
    /// <returns>���н� success�� false�� ��ȯ�մϴ�</returns>
    private (bool success, (int row, int col) pos) FindCandyInColumn(int startRow, int col)
    {
        Debug.Log($"��ĭ ���ο� ���� �˻� ����!");
        for (int row = startRow; row < TOTAL_ROW; row++)
        {
            if (candiesArray[row, col].candyNumber != 0)
            {
                // ��ĭ�� �ƴ�! ��ġ ��ȯ
                return (true, (row, col));
            }
        }

        // ����. 
        return (false, (0, 0));
    }

    /// <summary>
    /// ��ü ĵ�� �迭�� ���������� �Ʒ��� ���
    /// </summary>
    private void PrintCandyArray()
    {
        Debug.Log("=============================================================");
        for(int row = TOTAL_ROW-1;row >= 0; row--)
        {
            string column = "(";
            for(int col=0; col < TOTAL_COL; col++)
            {
                cells[row, col].text = candiesArray[row, col].candyNumber.ToString();
                if (col == TOTAL_COL-1)
                    column += $"{candiesArray[row, col].candyNumber}";
                else
                    column += $"{candiesArray[row, col].candyNumber}, ";

            }
            column += ")";
            Debug.Log(column);
        }
        Debug.Log("=============================================================");
    }

    public (int, GameObject)[,] GetCandiesArray()
    {
        return candiesArray;
    }

    public RectTransform GetCellRectTransform(int row, int col)
    {
        return cells[row, col].rectTransform;
    }
}
