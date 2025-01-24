using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class CandyDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float dragThreshold = 30f; // 드래그로 인식할 최소 거리

    public GameBoardManager gameBoardManager;

    private GameBoardCell[,] gameBoardCells;
    private Vector2 dragStartPosition;
    private Candy draggedCandy;
    private bool isSwapping = false;

    void Start()
    {
        gameBoardCells = gameBoardManager.GetGameBoardCellsArray();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSwapping) return;

        dragStartPosition = eventData.position;
        draggedCandy = eventData.pointerCurrentRaycast.gameObject?.GetComponent<Candy>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중에는 아무 것도 하지 않습니다.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSwapping || draggedCandy == null) return;

        Vector2 dragVector = eventData.position - dragStartPosition;
        if (dragVector.magnitude < dragThreshold) return;

        Vector2 dragDirection = GetDragDirection(dragVector);
        Vector2 currentPos = GetCandyGridPosition(draggedCandy.GetComponent<RectTransform>());
        Vector2 targetPos = currentPos + dragDirection;

        if (IsValidGridPosition(targetPos))
        {
            SwapCandies(currentPos, targetPos);
        }
    }

    private Vector2 GetDragDirection(Vector2 dragVector)
    {
        if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
        {
            return dragVector.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return dragVector.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    private Vector2 GetCandyGridPosition(RectTransform candyRectTransform)
    {
        for (int x = GameBoardManager.ROW_START_GAME_AREA; x < GameBoardManager.ROW_END_GAME_AREA; x++)
        {
            for (int y = 0; y < GameBoardManager.TOTAL_COL; y++)
            {
                if (gameBoardCells[x, y].GetCandyObject().GetRectTransform() == candyRectTransform)
                {
                    return new Vector2(x, y);
                }
            }
        }
        return Vector2.negativeInfinity;
    }

    private bool IsValidGridPosition(Vector2 pos)
    {
        return pos.x >= 0 && pos.x <= GameBoardManager.ROW_END_GAME_AREA &&
               pos.y >= 0 && pos.y < GameBoardManager.TOTAL_COL;
    }

    private void SwapCandies(Vector2 pos1, Vector2 pos2)
    {
        isSwapping = true;

        StartCoroutine(gameBoardManager.SwapCandies((int)pos1.x, (int)pos1.y, (int)pos2.x, (int)pos2.y));

        isSwapping = false;
    }
}
