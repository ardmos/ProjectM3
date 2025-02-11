using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĵ�� �巡�� ��� ó�� ������ �����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class CandyDragHandler : MonoBehaviour
{
    public static CandyDragHandler Instance;

    private GameBoardManager gameBoardManager;
    private Dictionary<Vector3Int, GameBoardCell> gameBoardCells;
    private Vector3 dragStartPosition;
    private Candy draggedCandy;
    [SerializeField] private bool isSwapping = false;
    private float dragThreshold = 0.5f; // �巡�׷� �ν��� �ּ� �Ÿ�

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
    /// ��ġ�� ������ ��ǥ������ ã������ �޼����Դϴ�. �÷��̾��� �巡�� ������ ���� �׸������ ������ ����� ��ȯ�մϴ�.
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
    /// ��ȿ�� �׸��� �ε������� Ȯ���ϴ� �޼����Դϴ�.
    /// </summary>
    private bool IsValidGridPosition(Vector3Int idx)
    {
        return idx.x >= gameBoardManager.Bounds.xMin && idx.x <= gameBoardManager.Bounds.xMax &&
               idx.y >= gameBoardManager.Bounds.yMin && idx.y <= gameBoardManager.Bounds.yMax;
    }

    /// <summary>
    /// GameBoardManager���� ĵ�� ������ ��û�ϴ� �޼����Դϴ�.
    /// </summary>
    private void SwapCandies(Vector3Int idx1, Vector3Int idx2)
    {
        isSwapping = true;

        gameBoardManager.StartSwap(idx1, idx2);

        isSwapping = false;
    }
}
