using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캔디 드래그 결과 처리 로직을 관리하는 클래스입니다.
/// </summary>
public class CandyDragHandler : MonoBehaviour
{
    public static CandyDragHandler Instance;

    private GameBoardManager gameBoardManager;
    private Dictionary<Vector3Int, GameBoardCell> gameBoardCells;
    private Vector3 dragStartPosition;
    private Candy draggedCandy;
    [SerializeField] private bool isSwapping = false;
    private float dragThreshold = 0.5f; // 드래그로 인식할 최소 거리

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameBoardManager = GameBoardManager.Instance;
        gameBoardCells = gameBoardManager.CellContents;
    }

    public void OnBeginDrag(Vector3 touchPos, Candy candy)
    {
        if (isSwapping || candy == null) return;

        dragStartPosition = touchPos;
        draggedCandy = candy;
    }

    public void OnEndDrag(Vector3 touchPos, Candy candy)
    {
        if (isSwapping || draggedCandy == null) return;

        Vector2 dragVector = touchPos - dragStartPosition;
        
        if (dragVector.magnitude < dragThreshold) return;

        Vector3Int dragDirection = GetDragDirection(dragVector);
        Vector3Int currentIndex = draggedCandy.CurrentIndex;
        Vector3Int targetIndex = currentIndex + dragDirection;

        if (IsValidGridPosition(targetIndex))
        {
            SwapCandies(currentIndex, targetIndex);
        }
    }

    /// <summary>
    /// 위치를 변경할 목표지점을 찾기위한 메서드입니다. 플레이어의 드래그 방향을 토대로 그리드상의 방향을 계산해 반환합니다.
    /// </summary>
    private Vector3Int GetDragDirection(Vector2 dragVector)
    {
        if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
        {
            return dragVector.x > 0 ? Vector3Int.up : Vector3Int.down;
        }
        else
        {
            return dragVector.y > 0 ? Vector3Int.right : Vector3Int.left;
        }
    }

    /// <summary>
    /// 유효한 그리드 인덱스인지 확인하는 메서드입니다.
    /// </summary>
    private bool IsValidGridPosition(Vector3Int idx)
    {
        return idx.x >= gameBoardManager.Bounds.xMin && idx.x <= gameBoardManager.Bounds.xMax &&
               idx.y >= gameBoardManager.Bounds.yMin && idx.y <= gameBoardManager.Bounds.yMax;
    }

    /// <summary>
    /// GameBoardManager에게 캔디 스왑을 요청하는 메서드입니다.
    /// </summary>
    private void SwapCandies(Vector3Int idx1, Vector3Int idx2)
    {
        isSwapping = true;

        gameBoardManager.StartSwap(idx1, idx2);

        isSwapping = false;
    }
}
