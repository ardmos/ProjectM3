using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 배열의 각 속성값. 
/// 0: 빈곳
/// 1: 캔디
/// </summary>
public class GameBoardManager : MonoBehaviour
{
    public const int TOTAL_ROW = 6;    // TOTAL_ROW는 항상 짝수. 항상 게임 영역과 같은 수의 ROW를 가진 생성 영역이 존재하기 때문.
    public const int TOTAL_COL = 3;    // 직사각형 구조이기 때문에 COL은 TOTAL_COL만 존재해도 충분.
    public const int ROW_END_CREATE_AREA = TOTAL_ROW - 1;
    public const int ROW_START_CREATE_AREA = TOTAL_ROW / 2;
    public const int ROW_END_GAME_AREA = ROW_START_CREATE_AREA - 1;
    public const int ROW_START_GAME_AREA = 0;

    public event Action OnNeedCandy;

    // 각 셀에 캔디 정보를 담고있는 게임보드 셀 배열
    private GameBoardCell[,] gameBoardCells = new GameBoardCell[TOTAL_ROW, TOTAL_COL];
    public GameObject cellsParent;

    private void Start()
    {
        // GameObject[,] candiesObjectsArray 초기화
        InitCandiesObjectsArray();

        PrintCandyArray();
        CreateCandies();
        StartCoroutine(DropCandies());
    }

    /// <summary>
    /// 테스트용 기능입니다.
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

    // 0. 상단 생성 영역 _ 검색 및 생성. 값이 0인 생성 영역에 사탕 생성
    private void CreateCandies()
    {
        OnNeedCandy.Invoke();

        PrintCandyArray();
    }

    // 1. 하단 게임 영역 _ 검색 및 위치 이동. 값이 0인 셀은 해당 열의 상단 요소들 값 중, 가장 가까운 0이 아닌 값과 위치 교환
    private IEnumerator DropCandies()
    {
        Debug.Log($"사탕 드랍 시작!");
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

                    // 캔디 이동
                    MoveCandy(sourceRow, sourceCol, row, col);
                }
            }
        }
        
        // DropCandies의 모든 작업이 완료되면 생성 작업 시작
        CreateCandies();
    }

    private void MoveCandy(int fromRow, int fromCol, int toRow, int toCol)
    {
        var sourceCell = gameBoardCells[fromRow, fromCol];
        var targetCell = gameBoardCells[toRow, toCol];

        // 캔디 오브젝트 이동
        sourceCell.GetCandyObject().GetComponent<RectTransform>().SetParent(targetCell.GetRectTransform(), false);

        // 데이터 갱신
        targetCell.SetCandyObject(sourceCell.GetCandyObject());
        targetCell.SetCandyNumber(sourceCell.GetCandyNumber());
        targetCell.SetCandyNumberText(sourceCell.GetCandyNumber());

        // 원본 셀 초기화
        sourceCell.SetCandyObject(null);
        sourceCell.SetCandyNumber(0);
        sourceCell.SetCandyNumberText(0);

        // 필요한 경우 애니메이션 추가
        //StartCoroutine(AnimateCandyMove(candyRect, startAnchoredPos, endAnchoredPos));
    }

    // 캔디 이동 애니메이션 (필요한 경우)
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
            if (gameBoardCells[row, col].GetCandyNumber() != 0)
            {
                // 빈칸이 아님! 위치 반환
                return (true, (row, col));
            }
        }

        // 실패. 
        return (false, (0, 0));
    }

    /// <summary>
    /// 전체 캔디 배열을 위에서부터 아래로 출력.
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
