using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManagerSample : MonoBehaviour
{
    private const int MAX_ROW = 11;
    private const int MAX_COL = 11;

    public event EventHandler<(int column, int count)> OnNeedCandy;

    private int[,] candyArray = new int[MAX_ROW, MAX_COL];

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        Array.Clear(candyArray, 0, candyArray.Length);
        CheckEmptyCandy();
    }

    public void CheckEmptyCandy()
    {
        for (int column = 0; column < MAX_COL; column++)
        {
            int emptyCandyCount = CountEmptyCandiesInColumn(column);
            if (emptyCandyCount > 0)
            {
                OnNeedCandy?.Invoke(this, (column, emptyCandyCount));
            }
        }
    }

    private int CountEmptyCandiesInColumn(int column)
    {
        int emptyCount = 0;
        for (int row = 0; row < MAX_ROW; row++)
        {
            if (candyArray[row, column] == 0)
                emptyCount++;
        }
        return emptyCount;
    }

    public void UpdateCandyArray(int column, int count)
    {
        int row = MAX_ROW - 1;
        while (count > 0 && row >= 0)
        {
            if (candyArray[row, column] == 0)
            {
                candyArray[row, column] = UnityEngine.Random.Range(1, 6); // 1-5 »çÀÌÀÇ ·£´ý Äµµð Å¸ÀÔ
                count--;
            }
            row--;
        }
    }

    public void RemoveCandy(int row, int column)
    {
        candyArray[row, column] = 0;
        CheckEmptyCandy();
    }

    public void MatchCandies(List<Vector2Int> matchedPositions)
    {
        foreach (Vector2Int pos in matchedPositions)
        {
            RemoveCandy(pos.x, pos.y);
        }
        CheckEmptyCandy();
    }
}
