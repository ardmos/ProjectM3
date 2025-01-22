using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;


/// <summary>
/// 배열의 각 속성값. 
/// 0: 빈곳
/// 1: 캔디
/// </summary>
public class GameBoardManager : MonoBehaviour
{
    private const int TOTAL_ROW = 6;    // TOTAL_ROW는 항상 짝수. 항상 게임 영역과 같은 수의 ROW를 가진 생성 영역이 존재하기 때문.
    private const int TOTAL_COL = 3;    // 직사각형 구조이기 때문에 COL은 TOTAL_COL만 존재해도 충분.
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

    // 0. 상단 생성 영역 _ 검색 및 생성. 값이 0인 생성 영역에 사탕 생성
    private void CreateCandies()
    {
        Debug.Log($"사탕 생성 시작!");
        for (int row=ROW_START_CREATE_AREA; row<TOTAL_ROW; row++)
        {
            for(int col=0; col<TOTAL_COL; col++)
            {
                Debug.Log($"candyArray[{row}, {col}]:{candyArray[row, col]}");
                if (candyArray[row, col] == 0)
                {
                    // 사탕 생성!
                    candyArray[row, col] = 1;
                    Debug.Log($"사탕 생성! row:{row}, col:{col}");
                }
            }
        }

        PrintCandyArray();
    }

    // 1. 하단 게임 영역 _ 검색 및 위치 이동. 값이 0인 셀은 해당 열의 상단 요소들 값 중, 가장 가까운 0이 아닌 값과 위치 교환
    private void DropCandies()
    {
        Debug.Log($"사탕 드랍 시작!");
        for (int row=ROW_START_GAME_AREA; row<=ROW_END_GAME_AREA; row++)
        {
            for(int col=0; col<TOTAL_COL; col++)
            {
                if (candyArray[row, col] == 0)
                {
                    // 빈칸! 해당 열의 상단 요소들 중 가장 가까운 0이 아닌 값과 위치를 교환합니다.
                    var result = FindCandyInColumn(row, col);
                    if (result.success == false) continue;  // 검색 실패. 해당 열의 위쪽은 모두 빈칸입니다. 다음 열로 검색을 넘어갑니다.

                    (int row, int col) closestCandyPos = result.pos;

                    Debug.Log($"빈칸! 해당 열의 상단 요소들 중 가장 가까운 0이 아닌 값과 위치를 교환합니다. ({row},{col}):{candyArray[row,col]} <-> ({closestCandyPos.row},{closestCandyPos.col}):{candyArray[closestCandyPos.row, closestCandyPos.col]}");
                    // 값 교환
                    (candyArray[row,col], candyArray[closestCandyPos.row, closestCandyPos.col]) = (candyArray[closestCandyPos.row, closestCandyPos.col], candyArray[row, col]);
                    Debug.Log($"교환 결과. ({row},{col}):{candyArray[row, col]} / ({closestCandyPos.row},{closestCandyPos.col}):{candyArray[closestCandyPos.row, closestCandyPos.col]}");
                }
            }
        }
        PrintCandyArray();

        // DropCandies의 모든 작업이 완료되면 생성 작업 시작
        CreateCandies();
    }

    /// <summary>
    /// startRow로 부터 TOTAL_ROW까지 row++ 중 해당 col의 값들을 검색. 가장 먼저 발견되는 0이 아닌 값의 위치를 반환합니다.
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="col"></param>
    /// <returns>실패시 success를 false로 반환합니다</returns>
    private (bool success, (int row, int col) pos) FindCandyInColumn(int startRow, int col)
    {
        Debug.Log($"빈칸 세로열 사탕 검색 시작!");
        for (int row = startRow; row < TOTAL_ROW; row++)
        {
            if (candyArray[row, col] != 0)
            {
                // 빈칸이 아님! 위치 반환
                return (true, (row, col));
            }
        }

        // 실패. 
        return (false, (0, 0));
    }


    /// <summary>
    /// 전체 캔디 배열을 위에서부터 아래로 출력
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
