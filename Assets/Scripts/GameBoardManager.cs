using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;
using Random = UnityEngine.Random;


[DefaultExecutionOrder(-9999)]
public class GameBoardManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameBoardManager Instance;

    public const int MIN_MATCH = 3; // �ּ� ��ġ ����

    public List<Vector3Int> SpawnerPosition = new();
    // ������ �� �� ������ �����ϴ� ��ųʸ�
    public Dictionary<Vector3Int, GameBoardCell> CellContents = new();
    // ĵ�� �����յ�
    public Candy[] CandyPrefabs;
    
    public ScoreManager ScoreManager;

    public Grid Grid => m_Grid; // Tilemap�� �θ� �׸���
    private Grid m_Grid;
    public BoundsInt Bounds => m_BoundsInt;
    // ������ ��踦 ��Ÿ���� ����
    private BoundsInt m_BoundsInt;

    // �� Ÿ�԰� ���� Gem ��ü�� �����ϴ� ��ųʸ�
    private Dictionary<int, Candy> m_CandyLookup;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeBoard();

        //CheckMatches(); �ʱ� ĵ��� ��ġ�� �ʰ� GenerateBoard���� ó���ϱ� ������ ȣ�� ���ص� ��.
    }

    // ���� �ʱ�ȭ �޼���
    public void InitializeBoard()
    {
        // ���� ��� ����
        SetBoardBounds();
        // ĵ�� ��� ���̺� �ʱ�ȭ
        InitializeCandyLookup();
        // ���� ����
        GenerateBoard();
    }

    // ���� ��� ����
    private void SetBoardBounds()
    {
        m_BoundsInt = new BoundsInt();
        var listOfCells = CellContents.Keys.ToList();

        m_BoundsInt.xMin = listOfCells[0].x;
        m_BoundsInt.xMax = m_BoundsInt.xMin;

        m_BoundsInt.yMin = listOfCells[0].y;
        m_BoundsInt.yMax = m_BoundsInt.yMin;

        foreach (var content in listOfCells)
        {
            if (content.x > m_BoundsInt.xMax)
                m_BoundsInt.xMax = content.x;
            else if (content.x < m_BoundsInt.xMin)
                m_BoundsInt.xMin = content.x;

            if (content.y > m_BoundsInt.yMax)
                m_BoundsInt.yMax = content.y;
            else if (content.y < m_BoundsInt.yMin)
                m_BoundsInt.yMin = content.y;
        }

        Debug.Log($"m_BoundsInt.xMin:{m_BoundsInt.xMin}, m_BoundsInt.xMax:{m_BoundsInt.xMax}, m_BoundsInt.yMin:{m_BoundsInt.yMin}, m_BoundsInt.yMax:{m_BoundsInt.yMax}");
    }

    // ĵ�� �˻��� ���̺� �ʱ�ȭ
    private void InitializeCandyLookup()
    {
        //candy type�� candy prefab ��ü�� �����մϴ�. 
        m_CandyLookup = new Dictionary<int, Candy>();
        foreach (var candy in CandyPrefabs)
        {
            m_CandyLookup.Add(candy.CandyType, candy);
        }
    }

    // 0. ����
    // ���� ���� ���� �޼���. ��ġ�� ĵ�� ������ �մϴ�
    private void GenerateBoard()
    {
        // ������ �� ���� ��ȸ�ϸ� �� ����
        for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
        {
            for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
            {
                var idx = new Vector3Int(x, y, 0);

                if (!CellContents.TryGetValue(idx, out var current) || current.ContainingCandy != null)
                    continue;

                var availableGems = m_CandyLookup.Keys.ToList();

                int leftGemType = -1;
                int bottomGemType = -1;
                int rightGemType = -1;
                int topGemType = -1;

                //check if there is two gem of the same type of the left
                if (CellContents.TryGetValue(idx + new Vector3Int(-1, 0, 0), out var leftContent) &&
                    leftContent.ContainingCandy != null)
                {
                    leftGemType = leftContent.ContainingCandy.CandyType;

                    if (CellContents.TryGetValue(idx + new Vector3Int(-2, 0, 0), out var leftLeftContent) &&
                        leftLeftContent.ContainingCandy != null && leftGemType == leftLeftContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the left, so we can't ue that type anymore
                        availableGems.Remove(leftGemType);
                    }
                }

                //check if there is two gem of the same type below
                if (CellContents.TryGetValue(idx + new Vector3Int(0, -1, 0), out var bottomContent) &&
                    bottomContent.ContainingCandy != null)
                {
                    bottomGemType = bottomContent.ContainingCandy.CandyType;

                    if (CellContents.TryGetValue(idx + new Vector3Int(0, -2, 0), out var bottomBottomContent) &&
                        bottomBottomContent.ContainingCandy != null && bottomGemType == bottomBottomContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the bottom, so we can't ue that type anymore
                        availableGems.Remove(bottomGemType);
                    }

                    if (leftGemType != -1 && leftGemType == bottomGemType)
                    {
                        //if the left and bottom gem are the same type, we need to check if the bottom left gem is ALSO
                        //of the same type, as placing that type here would create a square, which is a valid match
                        if (CellContents.TryGetValue(idx + new Vector3Int(-1, -1, 0), out var bottomLeftContent) &&
                            bottomLeftContent.ContainingCandy != null && bottomGemType == leftGemType)
                        {
                            //we already have a corner of gem on left, bottom left and bottom position, so remove that type
                            availableGems.Remove(leftGemType);
                        }
                    }
                }

                //as we fill left to right and bottom to top, we could only test left and bottom, but as we can have
                //manually placed gems, we still need to test in the other 2 direction to make sure

                //check right
                if (CellContents.TryGetValue(idx + new Vector3Int(1, 0, 0), out var rightContent) &&
                    rightContent.ContainingCandy != null)
                {
                    rightGemType = rightContent.ContainingCandy.CandyType;

                    //we have the same type on left and right, so placing that type here would create a 3 line
                    if (rightGemType != -1 && leftGemType == rightGemType)
                    {
                        availableGems.Remove(rightGemType);
                    }

                    if (CellContents.TryGetValue(idx + new Vector3Int(2, 0, 0), out var rightRightContent) &&
                        rightRightContent.ContainingCandy != null && rightGemType == rightRightContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the right, so we can't ue that type anymore
                        availableGems.Remove(rightGemType);
                    }

                    //right and bottom gem are the same, check the bottom right to avoid creating a square
                    if (rightGemType != -1 && rightGemType == bottomGemType)
                    {
                        if (CellContents.TryGetValue(idx + new Vector3Int(1, -1, 0), out var bottomRightContent) &&
                            bottomRightContent.ContainingCandy != null && bottomRightContent.ContainingCandy.CandyType == rightGemType)
                        {
                            availableGems.Remove(rightGemType);
                        }
                    }
                }

                //check up
                if (CellContents.TryGetValue(idx + new Vector3Int(0, 1, 0), out var topContent) &&
                    topContent.ContainingCandy != null)
                {
                    topGemType = topContent.ContainingCandy.CandyType;

                    //we have the same type on top and bottom, so placing that type here would create a 3 line
                    if (topGemType != -1 && topGemType == bottomGemType)
                    {
                        availableGems.Remove(topGemType);
                    }

                    if (CellContents.TryGetValue(idx + new Vector3Int(0, 1, 0), out var topTopContent) &&
                        topTopContent.ContainingCandy != null && topGemType == topTopContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the top, so we can't ue that type anymore
                        availableGems.Remove(topGemType);
                    }

                    //right and top gem are the same, check the top right to avoid creating a square
                    if (topGemType != -1 && topGemType == rightGemType)
                    {
                        if (CellContents.TryGetValue(idx + new Vector3Int(1, 1, 0), out var topRightContent) &&
                            topRightContent.ContainingCandy != null && topRightContent.ContainingCandy.CandyType == topGemType)
                        {
                            availableGems.Remove(topGemType);
                        }
                    }

                    //left and top gem are the same, check the top left to avoid creating a square
                    if (topGemType != -1 && topGemType == leftGemType)
                    {
                        if (CellContents.TryGetValue(idx + new Vector3Int(-1, 1, 0), out var topLeftContent) &&
                            topLeftContent.ContainingCandy != null && topLeftContent.ContainingCandy.CandyType == topGemType)
                        {
                            availableGems.Remove(topGemType);
                        }
                    }
                }


                var chosenGem = availableGems[Random.Range(0, availableGems.Count)];
                NewCandyAt(idx, m_CandyLookup[chosenGem]);
            }
        }
    }

    // 1. ��ġ
    /// <summary>
    /// 1. ��ġ Ȯ�� & ����
    /// </summary>
    private void CheckMatches()
    {
        List<Candy> matchedCandies = new List<Candy>();

        // ���� �˻�
        for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
        {
            for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax - 2; ++x)
            {
                Vector3Int idx = new Vector3Int(x, y, 0);
                Vector3Int idx2 = new Vector3Int(x + 1, y, 0);
                Vector3Int idx3 = new Vector3Int(x + 2, y, 0);

                //Debug.Log($"{Grid.GetCellCenterWorld(idx)}�� ĵ�� �������� ���� Ȯ��");

                if (!CellContents.TryGetValue(idx, out var current) || current.ContainingCandy == null)
                    continue;
                if (!CellContents.TryGetValue(idx2, out var current2) || current2.ContainingCandy == null)
                    continue;
                if (!CellContents.TryGetValue(idx3, out var current3) || current3.ContainingCandy == null)
                    continue;

                Candy candy1 = CellContents[idx].ContainingCandy;
                Candy candy2 = CellContents[idx2].ContainingCandy;
                Candy candy3 = CellContents[idx3].ContainingCandy;

                //Debug.Log($"candy1({Grid.GetCellCenterWorld(idx)}):{candy1}, candy2({Grid.GetCellCenterWorld(idx2)}):{candy2}, candy3({Grid.GetCellCenterWorld(idx3)}):{candy3}");
                if (candy1.CandyType == candy2.CandyType && candy2.CandyType == candy3.CandyType)
                {
                    matchedCandies.AddRange(new[] { candy1, candy2, candy3 });
                }
            }
        }


        // ���� �˻�
        for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
        {
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax-2; ++y)
            {
                Vector3Int idx = new Vector3Int(x, y, 0);
                Vector3Int idx2 = new Vector3Int(x, y + 1, 0);
                Vector3Int idx3 = new Vector3Int(x , y + 2, 0);

                //Debug.Log($"{Grid.GetCellCenterWorld(idx)}�� ĵ�� �������� ���� Ȯ��");

                if (!CellContents.TryGetValue(idx, out var current) || current.ContainingCandy == null)
                    continue;
                if (!CellContents.TryGetValue(idx2, out var current2) || current2.ContainingCandy == null)
                    continue;
                if (!CellContents.TryGetValue(idx3, out var current3) || current3.ContainingCandy == null)
                    continue;

                Candy candy1 = CellContents[idx].ContainingCandy;
                Candy candy2 = CellContents[idx2].ContainingCandy;
                Candy candy3 = CellContents[idx3].ContainingCandy;

                //Debug.Log($"candy1({Grid.GetCellCenterWorld(idx)}):{candy1}, candy2({Grid.GetCellCenterWorld(idx2)}):{candy2}, candy3({Grid.GetCellCenterWorld(idx3)}):{candy3}");

                if (candy1.CandyType == candy2.CandyType && candy2.CandyType == candy3.CandyType)
                {
                    matchedCandies.AddRange(new[] { candy1, candy2, candy3 });
                }
            }
        }
        // ��ġ ĵ��� ����
        PopMatches(matchedCandies);
    }
    /// <summary>
    /// ��ġ ĵ��� ����
    /// </summary>
    private void PopMatches(List<Candy> matchedCandies)
    {
        Debug.Log($"���� Pop!");
        foreach (Candy candy in matchedCandies)
        {
            CellContents[candy.CurrentIndex].ContainingCandy = null;
            candy.Pop();
        }

        PrintCellData();

        // ��ĭ���� ���� �۾� ����
        StartCoroutine(FindEmptyCellAndDropCandies());
    }


    // 2. �ϴ� ���� ���� _ Empty cell Ž�� �� ��ġ �̵�. ����ִ� ���� �ش� ���� ��� ��ҵ� �� ��, ���� ����� ĵ���� ���ο� ��ġ�� �ȴ�
    private IEnumerator FindEmptyCellAndDropCandies()
    {
        Queue<Vector3Int> emptyCellIndexQueue = new Queue<Vector3Int>();
        Debug.Log($"���� ��� ����!");
        yield return new WaitForSeconds(0.3f);
        for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
        {
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
            {
                var emptyCellIndex = new Vector3Int(x, y, 0);
                // �� �� Ȯ��
                if (CellContents[emptyCellIndex].ContainingCandy == null)
                {
                    Debug.Log($"�� ĭ �߰�! {Grid.GetCellCenterWorld(emptyCellIndex)}");
                    yield return new WaitForSeconds(0.1f);

                    var candy = FindCandyToFallInColumn(emptyCellIndex);
                    Debug.Log($"candy.success: {candy.success}");
                    if (candy.success)
                    {
                        // ĵ�� �̵�
                        MakeCandyFall(emptyCellIndex, candy.idx);
                        emptyCellIndexQueue.Enqueue(emptyCellIndex);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        // DropCandies�� ��� �۾��� �Ϸ�Ǹ� ���� �۾� ����
        while (emptyCellIndexQueue.Count > 0)
        {
            Vector3Int emptyCell = emptyCellIndexQueue.Dequeue();
            // emptyCell�� ���� ����� ��� SpawnerPostition �˻�
            Vector3Int closestSpawner = Vector3Int.zero;
            foreach (Vector3Int spawnerPosition in SpawnerPosition)
            {
                // x��ǥ�� ���� �Ÿ���
                if(emptyCell.x == spawnerPosition.x)
                {
                    // ���� y�� ����� �� ����
                    if (closestSpawner.Equals(Vector3Int.zero))
                    {
                        closestSpawner = spawnerPosition;
                        continue;
                    }

                    if(closestSpawner.y - emptyCell.y > spawnerPosition.y - emptyCell.y){
                        closestSpawner = spawnerPosition;
                    }                   
                }
            }

            // ���ο� ĵ�� ���� ����
            //ActivateSpawnerAt(closestSpawner);
        }
    }

    /// <summary>
    /// startRow�� ���� TOTAL_ROW���� row++ �� �ش� col�� ������ �˻�. ���� ���� �߰ߵǴ� 0�� �ƴ� ���� ��ġ�� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>���н� success�� false�� ��ȯ�մϴ�</returns>
    private (bool success, Vector3Int idx) FindCandyToFallInColumn(Vector3Int emptyCellIndex)
    {
        for (int x = emptyCellIndex.x; x <= m_BoundsInt.xMax; ++x)
        {
            if (CellContents[new Vector3Int(x, emptyCellIndex.y)].ContainingCandy != null)
            {
                // ��ĭ�� �ƴ�! ��ġ ��ȯ
                return (true, new Vector3Int(x, emptyCellIndex.y));
            }
        }

        // ����. 
        return (false, Vector3Int.zero);
    }

    // 3. Candy ����
    private void MakeCandyFall(Vector3Int emptyCellIndex, Vector3Int candyCellIndex)
    {
        Debug.Log($"emptyCell:{Grid.GetCellCenterWorld(emptyCellIndex)}, candyCell:{Grid.GetCellCenterWorld(candyCellIndex)}");

        // ĵ�� ������Ʈ �̵�
        CellContents[candyCellIndex].ContainingCandy.transform.DOMove(Grid.GetCellCenterWorld(emptyCellIndex), 0.5f).SetEase(Ease.OutBounce);

        // ������ ����
        CellContents[emptyCellIndex].ContainingCandy = CellContents[candyCellIndex].ContainingCandy;
        CellContents[candyCellIndex].ContainingCandy = null;
    }

    // MakeCandyFall ���� ActivateSpawnerAt ȣ�� ��� Ž��. �׸��� �� ��� ���� �ܰ踦 ������Ʈ �ӽ��� ���� �����ϱ�. 




    // 4. ����
    public void SwapCandies(Vector3Int sourceIndex, Vector3Int targetIndex)
    {
        // ĵ�� ������Ʈ ���� ����
        Candy sourceCandy = CellContents[sourceIndex].ContainingCandy;
        Candy targetCandy = CellContents[targetIndex].ContainingCandy;
        if (sourceCandy == null) return;

        if(targetCandy == null)
        {
            // ��ġ ���� �ִϸ��̼�
            sourceCandy.transform.DOMove(Grid.GetCellCenterWorld(targetIndex), 0.5f).SetEase(Ease.OutBounce);
            sourceCandy.UpdateIndex(targetIndex);
        }
        else
        {
            // ��ġ ���� �ִϸ��̼�
            sourceCandy.transform.DOMove(Grid.GetCellCenterWorld(targetIndex), 0.5f).SetEase(Ease.OutBounce);
            targetCandy.transform.DOMove(Grid.GetCellCenterWorld(sourceIndex), 0.5f).SetEase(Ease.OutBounce);

            // Candy ��ü�� ���� ������ ���� (�ʿ��� ���)
            sourceCandy.UpdateIndex(targetIndex);
            targetCandy.UpdateIndex(sourceIndex);

        }

        // CellContents ������ ����
        CellContents[sourceIndex].ContainingCandy = targetCandy;
        CellContents[targetIndex].ContainingCandy = sourceCandy;

        PrintCellData();

        // ���� �� ��Ī Ȯ��
        CheckMatches();
    }


    /// <summary>
    ///  Candy Placer Tile�κ��� ��ġ ���� �����ϵ��� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <param name="startingCandy"></param>
    public static void RegisterCell(Vector3Int cellPosition, Candy startingCandy = null)
    {
        if (Instance == null)
        {
            Instance = GameObject.Find("GameBoardManager").GetComponent<GameBoardManager>();
            Instance.GetReference();
        }

        if (!Instance.CellContents.ContainsKey(cellPosition))
            Instance.CellContents.Add(cellPosition, new GameBoardCell());
        
        if(startingCandy != null)
            Instance.NewCandyAt(cellPosition, startingCandy);
    }

    // �׸��� ����
    private void GetReference()
    {
        m_Grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    /// <summary>
    /// ���ο� ĵ�� ��ġ�ϴ� �޼��� �Դϴ�.
    /// candyPrefab�� null�̸� ���� ĵ�� ��ġ�մϴ�.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="candyPrefab"></param>
    /// <returns></returns>
    private Candy NewCandyAt(Vector3Int cell, Candy candyPrefab)
    {
        if (candyPrefab == null)
            candyPrefab = CandyPrefabs[Random.Range(0, CandyPrefabs.Length)];

        // ���尡 �ʱ�ȭ�� �������� Ȯ�� �� �����մϴ�.  
        if (CellContents[cell].ContainingCandy != null)
        {
            Destroy(CellContents[cell].ContainingCandy.gameObject);
        }

        var candy = Instantiate(candyPrefab, Grid.GetCellCenterWorld(cell), Quaternion.identity);
        CellContents[cell].ContainingCandy = candy;
        candy.Init(cell);

        return candy;
    }

    /// <summary>
    /// ĵ�� ���� ��ġ Ÿ�Ϸ� ĵ�� ���� ���� �����ϴ� �޼����Դϴ�. 
    /// </summary>
    /// <param name="cell"></param>
    public static void RegisterSpawner(Vector3Int cell)
    {
        if (Instance == null)
        {
            Instance = GameObject.Find("GameBoardManager").GetComponent<GameBoardManager>();
            Instance.GetReference();
        }

        Instance.SpawnerPosition.Add(cell);
    }

    // ĵ�� ���� ���� �۵���Ű�� �޼����Դϴ�
    private void ActivateSpawnerAt(Vector3Int cell)
    {
        var candy = Instantiate(CandyPrefabs[Random.Range(0, CandyPrefabs.Length)], m_Grid.GetCellCenterWorld(cell + Vector3Int.up), Quaternion.identity);
        //CellContents[cell].IncomingCandy = candy;

        //candy.StartMoveTimer();
        //candy.SpeedMultiplier = 1.0f;
        //m_NewTickingCells.Add(cell);

        //if (m_EmptyCells.Contains(cell)) m_EmptyCells.Remove(cell);
    }



    // ������
    private void PrintCellData()
    {
        for (int x = m_BoundsInt.xMax; x >= m_BoundsInt.xMin; --x)
        {
            string row = "";
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
            {
                var idx = new Vector3Int(x, y, 0);

                if (CellContents[idx].ContainingCandy != null)
                    row += $", {CellContents[idx].ContainingCandy.CandyType}";
                else
                    row += $", -1";
            }
            Debug.Log($"{row}");
        }
    }
}
