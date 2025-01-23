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

    // �� ���� ĵ�� ������ ����ִ� ���Ӻ��� �� �迭
    private GameBoardCell[,] gameBoardCells = new GameBoardCell[TOTAL_ROW, TOTAL_COL];
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
        var cells = cellsParent.GetComponentsInChildren<GameBoardCell>();
        int childArrayindex = 0;
        for (int row = TOTAL_ROW-1; row >= 0; row--)
        {
            for (int col = 0; col < TOTAL_COL; col++)
            {     
                gameBoardCells[row, col] = cells[childArrayindex];
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
                if (gameBoardCells[row, col].GetCandyNumber() == 0)
                {
                    yield return new WaitForSeconds(0.2f);

                    var result = FindCandyInColumn(row, col);
                    if (!result.success) continue;

                    (int sourceRow, int sourceCol) = result.pos;

                    // ĵ�� �̵�
                    MoveCandy(sourceRow, sourceCol, row, col);
                }
            }
        }
        
        // DropCandies�� ��� �۾��� �Ϸ�Ǹ� ���� �۾� ����
        CreateCandies();
    }

    private void MoveCandy(int fromRow, int fromCol, int toRow, int toCol)
    {
        var sourceCell = gameBoardCells[fromRow, fromCol];
        var targetCell = gameBoardCells[toRow, toCol];

        // ĵ�� ������Ʈ �̵�
        sourceCell.GetCandyObject().GetComponent<RectTransform>().SetParent(targetCell.GetRectTransform(), false);

        // ������ ����
        targetCell.SetCandyObject(sourceCell.GetCandyObject());
        targetCell.SetCandyNumber(sourceCell.GetCandyNumber());
        targetCell.SetCandyNumberText(sourceCell.GetCandyNumber());

        // ���� �� �ʱ�ȭ
        sourceCell.SetCandyObject(null);
        sourceCell.SetCandyNumber(0);
        sourceCell.SetCandyNumberText(0);

        // �ʿ��� ��� �ִϸ��̼� �߰�
        //StartCoroutine(AnimateCandyMove(candyRect, startAnchoredPos, endAnchoredPos));
    }

    // ĵ�� �̵� �ִϸ��̼� (�ʿ��� ���)
    private IEnumerator AnimateCandyMove(RectTransform candyRect, Vector2 startAnchoredPos, Vector2 endAnchoredPos)
    {
        float animationTime = 0.2f;

        for (float t = 0; t < animationTime; t += Time.deltaTime)
        {
            candyRect.anchoredPosition = Vector2.Lerp(startAnchoredPos, endAnchoredPos, t / animationTime);
            yield return null;
        }

        candyRect.anchoredPosition = endAnchoredPos;
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
            if (gameBoardCells[row, col].GetCandyNumber() != 0)
            {
                // ��ĭ�� �ƴ�! ��ġ ��ȯ
                return (true, (row, col));
            }
        }

        // ����. 
        return (false, (0, 0));
    }

    /// <summary>
    /// ��ü ĵ�� �迭�� ���������� �Ʒ��� ���.
    /// </summary>
    private void PrintCandyArray()
    {
        Debug.Log("=============================================================");
        for(int row = TOTAL_ROW-1;row >= 0; row--)
        {
            string column = "(";
            for(int col=0; col < TOTAL_COL; col++)
            {
                gameBoardCells[row, col].SetCandyNumberText(gameBoardCells[row, col].GetCandyNumber());
                if (col == TOTAL_COL-1)
                    column += $"{gameBoardCells[row, col].GetCandyNumber()}";
                else
                    column += $"{gameBoardCells[row, col].GetCandyNumber()}, ";

            }
            column += ")";
            Debug.Log(column);
        }
        Debug.Log("=============================================================");
    }

    public GameBoardCell[,] GetGameBoardCellsArray()
    {
        return gameBoardCells;
    }
}
