using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public const int TOTAL_ROW = 10; // TOTAL_ROW�� �׻� ¦��. �׻� ���� ������ ���� ���� ROW�� ���� ���� ������ �����ϱ� ����.
    public const int TOTAL_COL = 5; // ���簢�� �����̱� ������ COL�� TOTAL_COL�� �����ص� ���.
    public const int ROW_END_CREATE_AREA = TOTAL_ROW - 1;
    public const int ROW_START_CREATE_AREA = TOTAL_ROW / 2;
    public const int ROW_END_GAME_AREA = ROW_START_CREATE_AREA - 1;
    public const int ROW_START_GAME_AREA = 0;
    public const int MIN_MATCH = 3; // �ּ� ��ġ ����

    public const float SWAP_DURATION = 0.3f;

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
    }

    /// <summary>
    /// �׽�Ʈ�� ����Դϴ�.
    /// </summary>
    private void InitCandiesObjectsArray()
    {
        //candiesParentsObjects
        var cells = cellsParent.GetComponentsInChildren<GameBoardCell>();
        int childArrayindex = 0;
        for (int row = TOTAL_ROW - 1; row >= 0; row--)
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

        // CreateCandies�� �Ϸ�Ǹ� GAME AREA�� ��ġ���θ� üũ�� 
        CheckMatches();
    }

    /// <summary>
    /// 1. ��ġ Ȯ�� & ����
    /// </summary>
    public void CheckMatches()
    {
        List<GameBoardCell> matchedCandies = new List<GameBoardCell>();

        // ���� �˻�
        for (int row = ROW_START_GAME_AREA; row <= ROW_END_GAME_AREA; row++)
        {
            for (int col = 0; col < TOTAL_COL - 2; col++)
            {
                GameBoardCell candy1 = gameBoardCells[row, col];
                GameBoardCell candy2 = gameBoardCells[row, col + 1];
                GameBoardCell candy3 = gameBoardCells[row, col + 2];

                if (candy1.GetCandyNumber() != 0 && candy1.GetCandyNumber() == candy2.GetCandyNumber() && candy2.GetCandyNumber() == candy3.GetCandyNumber())
                {
                    matchedCandies.AddRange(new[] { candy1, candy2, candy3 });
                }
            }
        }

        // ���� �˻�
        for (int col = 0; col < TOTAL_COL; col++)
        {
            for (int row = ROW_START_GAME_AREA; row <= ROW_END_GAME_AREA - 2; row++)
            {
                GameBoardCell candy1 = gameBoardCells[row, col];
                GameBoardCell candy2 = gameBoardCells[row + 1, col];
                GameBoardCell candy3 = gameBoardCells[row + 2, col];

                if (candy1.GetCandyNumber() != 0 && candy1.GetCandyNumber() == candy2.GetCandyNumber() && candy2.GetCandyNumber() == candy3.GetCandyNumber())
                {
                    matchedCandies.AddRange(new[] { candy1, candy2, candy3 });
                }
            }
        }

        // ��ġ ĵ��� ����
        PopMatches(matchedCandies.Distinct().ToList());
    }

    /// <summary>
    /// ��ġ ĵ��� ����
    /// </summary>
    private void PopMatches(List<GameBoardCell> matchedCandies)
    {
        Debug.Log($"matchedCandies.Count:{matchedCandies.Count}");
        // ��ġ�� ĵ�� �ִ¸�ŭ ����
        foreach (GameBoardCell gameBoardCell in matchedCandies)
        {
            gameBoardCell.PopCandy();
        }

        //���� Drop�۾� ����
        StartCoroutine(DropCandies());
    }


    // 2. �ϴ� ���� ���� _ �� ĭ Ž�� �� ��ġ �̵�. ���� 0�� ���� �ش� ���� ��� ��ҵ� �� ��, ���� ����� 0�� �ƴ� ���� ��ġ ��ȯ
    private IEnumerator DropCandies()
    {
        int moveCount = 0;

        Debug.Log($"���� ��� ����!");
        yield return new WaitForSeconds(0.3f);
        for (int row = ROW_START_GAME_AREA; row <= ROW_END_GAME_AREA; row++)
        {
            for (int col = 0; col < TOTAL_COL; col++)
            {
                // �� �� Ȯ��
                if (gameBoardCells[row, col].GetCandyNumber() == 0)
                {
                    //Debug.Log($"�� ĭ �߰�! {row},{col}");
                    yield return new WaitForSeconds(0.1f);

                    var result = FindCandyInColumn(row, col);
                    if (!result.success) continue;

                    (int sourceRow, int sourceCol) = result.pos;

                    // ĵ�� �̵�
                    MoveCandy(sourceRow, sourceCol, row, col);
                    moveCount++;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (moveCount > 0)
        {
            // DropCandies�� ��� �۾��� �Ϸ�Ǹ� ���� �۾� ����
            CreateCandies();
        }
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

    /// <summary>
    /// ĵ�� ��ġ ����
    /// </summary>
    /// <param name="fromRow"></param>
    /// <param name="fromCol"></param>
    /// <param name="toRow"></param>
    /// <param name="toCol"></param>
    public IEnumerator SwapCandies(int fromRow, int fromCol, int toRow, int toCol)
    {
        var sourceCell = gameBoardCells[fromRow, fromCol];
        var targetCell = gameBoardCells[toRow, toCol];

        if (sourceCell == null || targetCell == null) yield break;

        var tempSourceCell = new GameBoardCell(gameBoardCells[fromRow, fromCol]);

        var candy1 = sourceCell.GetCandyObject();
        var candy2 = targetCell.GetCandyObject();

        // ĵ�� ������Ʈ �̵�
        Vector3 tempPosition = candy1.GetRectTransform().localPosition;
        sourceCell.GetCandyObject().GetRectTransform().DOLocalMove(candy2.GetRectTransform().localPosition, SWAP_DURATION);
        yield return candy2.GetRectTransform().DOLocalMove(tempPosition, SWAP_DURATION).WaitForCompletion();

        candy1.GetComponent<RectTransform>().SetParent(targetCell.GetRectTransform(), false);
        candy2.GetComponent<RectTransform>().SetParent(tempSourceCell.GetRectTransform(), false);

        // ������ ����
        sourceCell.SetNewGameBoardCellData(targetCell);
        targetCell.SetNewGameBoardCellData(tempSourceCell);

        // ���� ���� ��ġ Ȯ��
        CheckMatches();
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
        for (int row = TOTAL_ROW - 1; row >= 0; row--)
        {
            string column = "(";
            for (int col = 0; col < TOTAL_COL; col++)
            {
                gameBoardCells[row, col].SetCandyNumberText(gameBoardCells[row, col].GetCandyNumber());
                if (col == TOTAL_COL - 1)
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
