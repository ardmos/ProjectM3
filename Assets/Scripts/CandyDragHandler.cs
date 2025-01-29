using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (isSwapping || gameBoardManager.state != GameBoardManager.State.Idle) return;

        dragStartPosition = touchPos;
        draggedCandy = candy;

        //Debug.Log($"dragStartPosition:{dragStartPosition}, draggedCandy:{draggedCandy}");
    }

    public void OnEndDrag(Vector3 touchPos, Candy candy)
    {
        //Debug.Log($"(isSwapping || draggedCandy == null):{(isSwapping || draggedCandy == null)}");
        if (isSwapping || draggedCandy == null) return;

        Vector2 dragVector = touchPos - dragStartPosition;
        //Debug.Log($"dragVector:{dragVector}, dragThreshold:{dragThreshold}, (dragVector.magnitude < dragThreshold):{(dragVector.magnitude < dragThreshold)}");
        if (dragVector.magnitude < dragThreshold) return;

        Vector3Int dragDirection = GetDragDirection(dragVector);
        Vector3Int currentIndex = draggedCandy.CurrentIndex; //GetCandyGridPosition(draggedCandy);
        Vector3Int targetIndex = currentIndex + dragDirection;

        //Debug.Log($"currentIndex:{currentIndex}, targetIndex:{targetIndex}, IsValidGridPosition(targetIndex):{IsValidGridPosition(targetIndex)}");

        if (IsValidGridPosition(targetIndex))
        {
            SwapCandies(currentIndex, targetIndex);
        }
    }

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

    private Vector2 GetCandyGridPosition(Candy draggedCandy)
    {
        for (int x = gameBoardManager.Bounds.xMin; x <= gameBoardManager.Bounds.xMax; ++x)
        {
            for (int y = gameBoardManager.Bounds.yMin; y <= gameBoardManager.Bounds.yMax; ++y)
            {
                var idx = new Vector3Int(x, y);

                if (gameBoardCells[idx].ContainingCandy == draggedCandy)
                {
                    return new Vector2(idx.x, idx.y);
                }
            }
        }
        return Vector2.negativeInfinity;
    }

    private bool IsValidGridPosition(Vector3Int idx)
    {
        return idx.x >= gameBoardManager.Bounds.xMin && idx.x <= gameBoardManager.Bounds.xMax &&
               idx.y >= gameBoardManager.Bounds.yMin && idx.y <= gameBoardManager.Bounds.yMax;
    }

    private void SwapCandies(Vector3Int idx1, Vector3Int idx2)
    {
        isSwapping = true;

        //gameBoardManager.SwapCandies(idx1, idx2);
        gameBoardManager.StartSwap(idx1, idx2);

        isSwapping = false;
    }
}
