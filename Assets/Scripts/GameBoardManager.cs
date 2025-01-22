using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;


/// <summary>
/// �迭�� �� �Ӽ���. 
/// 0: ���
/// 1: ĵ��
/// </summary>
public class GameBoardManager : MonoBehaviour
{
    private const int TOTAL_ROW = 6;    // TOTAL_ROW�� �׻� ¦��. �׻� ���� ������ ���� ���� ROW�� ���� ���� ������ �����ϱ� ����.
    private const int TOTAL_COL = 3;    // ���簢�� �����̱� ������ COL�� TOTAL_COL�� �����ص� ���.
    private const int ROW_END_CREATE_AREA = TOTAL_ROW - 1;
    private const int ROW_START_CREATE_AREA = TOTAL_ROW / 2;
    private const int ROW_END_GAME_AREA = ROW_START_CREATE_AREA - 1;
    private const int ROW_START_GAME_AREA = 0;

    public event EventHandler<(int column, int count)> OnNeedCandy;

    private int[,] candyArray = new int[TOTAL_ROW, TOTAL_COL];

    private void Start()
    {
        PrintCandyArray();
        CreateCandies();
        DropCandies();
    }

    // 0. ��� ���� ���� _ �˻� �� ����. ���� 0�� ���� ������ ���� ����
    private void CreateCandies()
    {
        Debug.Log($"���� ���� ����!");
        for (int row=ROW_START_CREATE_AREA; row<TOTAL_ROW; row++)
        {
            for(int col=0; col<TOTAL_COL; col++)
            {
                Debug.Log($"candyArray[{row}, {col}]:{candyArray[row, col]}");
                if (candyArray[row, col] == 0)
                {
                    // ���� ����!
                    candyArray[row, col] = 1;
                    Debug.Log($"���� ����! row:{row}, col:{col}");
                }
            }
        }

        PrintCandyArray();
    }

    // 1. �ϴ� ���� ���� _ �˻� �� ��ġ �̵�. ���� 0�� ���� �ش� ���� ��� ��ҵ� �� ��, ���� ����� 0�� �ƴ� ���� ��ġ ��ȯ
    private void DropCandies()
    {
        Debug.Log($"���� ��� ����!");
        for (int row=ROW_START_GAME_AREA; row<=ROW_END_GAME_AREA; row++)
        {
            for(int col=0; col<TOTAL_COL; col++)
            {
                if (candyArray[row, col] == 0)
                {
                    // ��ĭ! �ش� ���� ��� ��ҵ� �� ���� ����� 0�� �ƴ� ���� ��ġ�� ��ȯ�մϴ�.
                    var result = FindCandyInColumn(row, col);
                    if (result.success == false) continue;  // �˻� ����. �ش� ���� ������ ��� ��ĭ�Դϴ�. ���� ���� �˻��� �Ѿ�ϴ�.

                    (int row, int col) closestCandyPos = result.pos;

                    Debug.Log($"��ĭ! �ش� ���� ��� ��ҵ� �� ���� ����� 0�� �ƴ� ���� ��ġ�� ��ȯ�մϴ�. ({row},{col}):{candyArray[row,col]} <-> ({closestCandyPos.row},{closestCandyPos.col}):{candyArray[closestCandyPos.row, closestCandyPos.col]}");
                    // �� ��ȯ
                    (candyArray[row,col], candyArray[closestCandyPos.row, closestCandyPos.col]) = (candyArray[closestCandyPos.row, closestCandyPos.col], candyArray[row, col]);
                    Debug.Log($"��ȯ ���. ({row},{col}):{candyArray[row, col]} / ({closestCandyPos.row},{closestCandyPos.col}):{candyArray[closestCandyPos.row, closestCandyPos.col]}");
                }
            }
        }
        PrintCandyArray();

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
            if (candyArray[row, col] != 0)
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
                if(col == TOTAL_COL-1)
                    column += $"{candyArray[row, col]}";
                else
                    column += $"{candyArray[row, col]}, ";

            }
            column += ")";
            Debug.Log(column);
        }
        Debug.Log("=============================================================");
    }

}
