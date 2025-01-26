using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 배열의 각 속성값. 
/// 0: 빈곳
/// 1: 캔디
/// </summary>
public class GameBoardManager : MonoBehaviour
{
    public const int TOTAL_ROW = 10; // TOTAL_ROW는 항상 짝수. 항상 게임 영역과 같은 수의 ROW를 가진 생성 영역이 존재하기 때문.
    public const int TOTAL_COL = 5; // 직사각형 구조이기 때문에 COL은 TOTAL_COL만 존재해도 충분.
    public const int ROW_END_CREATE_AREA = TOTAL_ROW - 1;
    public const int ROW_START_CREATE_AREA = TOTAL_ROW / 2;
    public const int ROW_END_GAME_AREA = ROW_START_CREATE_AREA - 1;
    public const int ROW_START_GAME_AREA = 0;
    public const int MIN_MATCH = 3; // 최소 매치 개수

    public const float SWAP_DURATION = 0.3f;

    public event Action OnNeedCandy;

    // 각 셀에 캔디 정보를 담고있는 게임보드 셀 배열
    private GameBoardCell[,] gameBoardCells = new GameBoardCell[TOTAL_ROW, TOTAL_COL];
    [SerializeField] private GameObject cellsParent;

    public MoveCountManager moveCountManager;
    public ScoreManager scoreManager;

    private void Start()
    {
        // GameObject[,] candiesObjectsArray 초기화
        InitCandiesObjectsArray();

        PrintCandyArray();
        CreateCandies();
    }

    /// <summary>
    /// 테스트용 기능입니다.
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

    // 0. 상단 생성 영역 _ 검색 및 생성. 값이 0인 생성 영역에 사탕 생성
    private void CreateCandies()
    {
        OnNeedCandy.Invoke();

        PrintCandyArray();

        // CreateCandies가 완료되면 GAME AREA의 매치여부를 체크함 
        CheckMatches();
    }

    /// <summary>
    /// 1. 매치 확인 & 제거
    /// </summary>
    public void CheckMatches()
    {
        List<GameBoardCell> matchedCandies = new List<GameBoardCell>();

        // 수평 검사
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

        // 수직 검사
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

        // 매치 캔디들 제거
        PopMatches(matchedCandies.Distinct().ToList());
    }

    /// <summary>
    /// 매치 캔디들 제거
    /// </summary>
    private void PopMatches(List<GameBoardCell> matchedCandies)
    {
        Debug.Log($"matchedCandies.Count:{matchedCandies.Count}");
        // 매치된 캔디가 있는만큼 제거
        foreach (GameBoardCell gameBoardCell in matchedCandies)
        {
            // 제거되는 캔디들 점수로 등록
            scoreManager.AddCurrentScore(gameBoardCell.GetCandyObject().GetScore());
            gameBoardCell.PopCandy();
        }

        //이후 Drop작업 시작
        StartCoroutine(DropCandies());
    }


    // 2. 하단 게임 영역 _ 빈 칸 탐색 및 위치 이동. 값이 0인 셀은 해당 열의 상단 요소들 값 중, 가장 가까운 0이 아닌 값과 위치 교환
    private IEnumerator DropCandies()
    {
        int moveCount = 0;

        Debug.Log($"사탕 드랍 시작!");
        yield return new WaitForSeconds(0.3f);
        for (int targetRow = ROW_START_GAME_AREA; targetRow <= ROW_END_GAME_AREA; targetRow++)
        {
            for (int targetCol = 0; targetCol < TOTAL_COL; targetCol++)
            {
                // 빈 곳 확인
                if (gameBoardCells[targetRow, targetCol].GetCandyNumber() == 0)
                {
                    //Debug.Log($"빈 칸 발견! {row},{col}");
                    yield return new WaitForSeconds(0.1f);

                    var result = FindCandyInColumn(targetRow, targetCol);
                    if (!result.success) continue;

                    (int sourceRow, int sourceCol) = result.pos;

                    // 캔디 이동
                    //MoveCandy(sourceRow, sourceCol, targetRow, targetCol);
                    StartCoroutine(MoveCandyWithAnimation(sourceRow, sourceCol, targetRow, targetCol));
                    moveCount++;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (moveCount > 0)
        {
            // DropCandies의 모든 작업이 완료되면 생성 작업 시작
            CreateCandies();
        }
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

        // 원본 셀 초기화
        sourceCell.SetCandyObject(null);
        sourceCell.SetCandyNumber(0);
    }

    private IEnumerator MoveCandyWithAnimation(int fromRow, int fromCol, int toRow, int toCol)
    {
        float moveDuration = 0.3f; // 이동 애니메이션 지속 시간

        var sourceCell = gameBoardCells[fromRow, fromCol];
        var targetCell = gameBoardCells[toRow, toCol];

        RectTransform candyRect = sourceCell.GetCandyObject().GetComponent<RectTransform>();
        //candyRect.SetParent(cellsParent.GetComponent<RectTransform>(), false);
        Vector2 endPosition = targetCell.GetRectTransform().anchoredPosition;

        // 애니메이션 시작
        Tween moveTween = candyRect.DOAnchorPos(endPosition, moveDuration).SetEase(Ease.OutQuad); // 이동 곡선 설정

        yield return moveTween.WaitForCompletion();

        // 애니메이션 완료 후 부모 변경
        //candyRect.SetParent(targetCell.GetRectTransform(), false);
        //candyRect.anchoredPosition = Vector2.zero; // 로컬 위치 리셋
        //candyRect.anchorMin = new Vector2(0.5f, 0.5f);
        //candyRect.anchorMax = new Vector2(0.5f, 0.5f);

        // 데이터 갱신
        targetCell.SetCandyObject(sourceCell.GetCandyObject());
        targetCell.SetCandyNumber(sourceCell.GetCandyNumber());

        // 원본 셀 초기화
        sourceCell.SetCandyObject(null);
        sourceCell.SetCandyNumber(0);
    }

    /// <summary>
    /// 캔디 위치 교대
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

        GameObject tempSourceCellObj = new GameObject("tempSourceCellObj");
        tempSourceCellObj.AddComponent<GameBoardCell>();
        var tempSourceCell = tempSourceCellObj.GetComponent<GameBoardCell>();
        tempSourceCell.SetNewGameBoardCellData(sourceCell);

        var sourceCandy = sourceCell.GetCandyObject();
        var targetCandy = targetCell.GetCandyObject();

        // 캔디 오브젝트 이동
        Vector3 tempPosition = sourceCandy.GetRectTransform().localPosition;
        sourceCandy.GetRectTransform().DOLocalMove(targetCandy.GetRectTransform().localPosition, SWAP_DURATION);
        yield return targetCandy.GetRectTransform().DOLocalMove(tempPosition, SWAP_DURATION).WaitForCompletion();

        //sourceCandy.GetComponent<RectTransform>().SetParent(targetCell.GetRectTransform(), false);
        //targetCandy.GetComponent<RectTransform>().SetParent(sourceCell.GetRectTransform(), false);

        // 데이터 갱신
        sourceCell.SetNewGameBoardCellData(targetCell);
        targetCell.SetNewGameBoardCellData(tempSourceCell);

        Destroy(tempSourceCellObj);

        // 스왑 동작 카운트 증가
        moveCountManager.IncreaseMoveCount();

        // 스왑 이후 매치 확인
        CheckMatches();
    }


    /// <summary>
    /// startRow로 부터 TOTAL_ROW까지 row++ 중 해당 col의 값들을 검색. 가장 먼저 발견되는 0이 아닌 값의 위치를 반환합니다.
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="col"></param>
    /// <returns>실패시 success를 false로 반환합니다</returns>
    private (bool success, (int row, int col) pos) FindCandyInColumn(int startRow, int col)
    {
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
        for (int row = TOTAL_ROW - 1; row >= 0; row--)
        {
            string column = "(";
            for (int col = 0; col < TOTAL_COL; col++)
            {
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

    public RectTransform GetCellsParent() { return cellsParent.GetComponent<RectTransform>(); }
}
