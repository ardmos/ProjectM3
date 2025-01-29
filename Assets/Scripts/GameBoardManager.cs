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
using UnityEngine.XR;


[DefaultExecutionOrder(-9999)]
public class GameBoardManager : MonoBehaviour
{
    public enum State
    {
        Init,
        Idle,
        MatchCheck,
        Pop,
        EmptyCheck,
        Spawn,
        Fall,
        Swap
    }

    // �̱��� �ν��Ͻ�
    public static GameBoardManager Instance;

    public const int MIN_MATCH = 3; // �ּ� ��ġ ����

    public List<Vector3Int> SpawnerPosition = new();
    public Dictionary<Vector3Int, Queue<Candy>> SpawnerContents = new();
    // ������ �� �� ������ �����ϴ� ��ųʸ�
    public Dictionary<Vector3Int, GameBoardCell> CellContents = new();
    // ĵ�� �����յ�
    //public Candy[] CandyPrefabs;

    public ScoreManager ScoreManager;

    public Grid Grid => m_Grid; // Tilemap�� �θ� �׸���
    private Grid m_Grid;
    public BoundsInt Bounds => m_BoundsInt;
    // ������ ��踦 ��Ÿ���� ����
    private BoundsInt m_BoundsInt;

    // �� Ÿ�԰� ���� Gem ��ü�� �����ϴ� ��ųʸ�
    private Dictionary<CandyType, Candy> m_CandyLookup;

    public State state = State.Init;

    // ���� ���� ����
    private Vector3Int swapSourceIdx = new();
    private Vector3Int swapTargetIdx = new();

    // ��ġ üũ ��� ����
    private List<Candy> matchedCandies = new List<Candy>();

    // ��ĭ �˻� ��� ����
    private List<Vector3Int> emptyCells = new List<Vector3Int>();

    public event Action OnMoved;
    public event Action<List<Candy>> OnPopped;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateState(State.Init);
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


    private void RunStateMachine()
    {
        switch (state)
        {
            case State.Init:
                InitializeBoard();
                UpdateState(State.Idle);
                break;
            case State.Idle:
                // �⺻ ����
                //Debug.Log($"Idle State:{state}");
                break;
            case State.MatchCheck:
                CheckMatches();
                // ��ġ ĵ��� ����
                if (matchedCandies.Count > 0)
                    UpdateState(State.Pop);
                else
                    UpdateState(State.Idle);
                break;
            case State.Pop:
                OnPopped.Invoke(matchedCandies);
                PopMatches();
                // ��ĭ üũ ����
                UpdateState(State.EmptyCheck);
                break;
            case State.EmptyCheck:
                EmptyCheck();
                UpdateState(State.Spawn);
                break;
            case State.Spawn:
                Spawn();
                UpdateState(State.Fall);
                break;
            case State.Fall:
                Fall();
                UpdateState(State.MatchCheck);
                break;
            case State.Swap:
                // ���� ���ɿ��� üũ ������ �ʿ���

                SwapCandies(swapSourceIdx, swapTargetIdx);
                // ���� �� ���� ī��Ʈ ����
                OnMoved.Invoke();
                // ���� �� ��Ī Ȯ��
                UpdateState(State.MatchCheck);
                break;
            default:
                break;
        }
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
        m_CandyLookup = new Dictionary<CandyType, Candy>();
        foreach (var candy in LevelData.Instance.CandyPrefabs)
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

                var availableCandies = m_CandyLookup.Keys.ToList();

                int leftCandyType = -1;
                int bottomCandyType = -1;
                int rightCandyType = -1;
                int topCandyType = -1;

                //check if there is two gem of the same type of the left
                if (CellContents.TryGetValue(idx + new Vector3Int(-1, 0, 0), out var leftContent) &&
                    leftContent.ContainingCandy != null)
                {
                    leftCandyType = (int)leftContent.ContainingCandy.CandyType;

                    if (CellContents.TryGetValue(idx + new Vector3Int(-2, 0, 0), out var leftLeftContent) &&
                        leftLeftContent.ContainingCandy != null && leftCandyType == (int)leftLeftContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the left, so we can't ue that type anymore
                        availableCandies.Remove((CandyType)leftCandyType);
                    }
                }

                //check if there is two gem of the same type below
                if (CellContents.TryGetValue(idx + new Vector3Int(0, -1, 0), out var bottomContent) &&
                    bottomContent.ContainingCandy != null)
                {
                    bottomCandyType = (int)bottomContent.ContainingCandy.CandyType;

                    if (CellContents.TryGetValue(idx + new Vector3Int(0, -2, 0), out var bottomBottomContent) &&
                        bottomBottomContent.ContainingCandy != null && bottomCandyType == (int)bottomBottomContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the bottom, so we can't ue that type anymore
                        availableCandies.Remove((CandyType)bottomCandyType);
                    }

                    if (leftCandyType != -1 && leftCandyType == bottomCandyType)
                    {
                        //if the left and bottom gem are the same type, we need to check if the bottom left gem is ALSO
                        //of the same type, as placing that type here would create a square, which is a valid match
                        if (CellContents.TryGetValue(idx + new Vector3Int(-1, -1, 0), out var bottomLeftContent) &&
                            bottomLeftContent.ContainingCandy != null && bottomCandyType == leftCandyType)
                        {
                            //we already have a corner of gem on left, bottom left and bottom position, so remove that type
                            availableCandies.Remove((CandyType)leftCandyType);
                        }
                    }
                }

                //as we fill left to right and bottom to top, we could only test left and bottom, but as we can have
                //manually placed gems, we still need to test in the other 2 direction to make sure

                //check right
                if (CellContents.TryGetValue(idx + new Vector3Int(1, 0, 0), out var rightContent) &&
                    rightContent.ContainingCandy != null)
                {
                    rightCandyType = (int)rightContent.ContainingCandy.CandyType;

                    //we have the same type on left and right, so placing that type here would create a 3 line
                    if (rightCandyType != -1 && leftCandyType == rightCandyType)
                    {
                        availableCandies.Remove((CandyType)rightCandyType);
                    }

                    if (CellContents.TryGetValue(idx + new Vector3Int(2, 0, 0), out var rightRightContent) &&
                        rightRightContent.ContainingCandy != null && rightCandyType == (int)rightRightContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the right, so we can't ue that type anymore
                        availableCandies.Remove((CandyType)rightCandyType);
                    }

                    //right and bottom gem are the same, check the bottom right to avoid creating a square
                    if (rightCandyType != -1 && rightCandyType == bottomCandyType)
                    {
                        if (CellContents.TryGetValue(idx + new Vector3Int(1, -1, 0), out var bottomRightContent) &&
                            bottomRightContent.ContainingCandy != null && (int)bottomRightContent.ContainingCandy.CandyType == rightCandyType)
                        {
                            availableCandies.Remove((CandyType)rightCandyType);
                        }
                    }
                }

                //check up
                if (CellContents.TryGetValue(idx + new Vector3Int(0, 1, 0), out var topContent) &&
                    topContent.ContainingCandy != null)
                {
                    topCandyType = (int)topContent.ContainingCandy.CandyType;

                    //we have the same type on top and bottom, so placing that type here would create a 3 line
                    if (topCandyType != -1 && topCandyType == bottomCandyType)
                    {
                        availableCandies.Remove((CandyType)topCandyType);
                    }

                    if (CellContents.TryGetValue(idx + new Vector3Int(0, 1, 0), out var topTopContent) &&
                        topTopContent.ContainingCandy != null && topCandyType == (int)topTopContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the top, so we can't ue that type anymore
                        availableCandies.Remove((CandyType)topCandyType);
                    }

                    //right and top gem are the same, check the top right to avoid creating a square
                    if (topCandyType != -1 && topCandyType == rightCandyType)
                    {
                        if (CellContents.TryGetValue(idx + new Vector3Int(1, 1, 0), out var topRightContent) &&
                            topRightContent.ContainingCandy != null && (int)topRightContent.ContainingCandy.CandyType == topCandyType)
                        {
                            availableCandies.Remove((CandyType)topCandyType);
                        }
                    }

                    //left and top gem are the same, check the top left to avoid creating a square
                    if (topCandyType != -1 && topCandyType == leftCandyType)
                    {
                        if (CellContents.TryGetValue(idx + new Vector3Int(-1, 1, 0), out var topLeftContent) &&
                            topLeftContent.ContainingCandy != null && (int)topLeftContent.ContainingCandy.CandyType == topCandyType)
                        {
                            availableCandies.Remove((CandyType)topCandyType);
                        }
                    }
                }


                var chosenGem = availableCandies[Random.Range(0, availableCandies.Count)];
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
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax - 2; ++y)
            {
                Vector3Int idx = new Vector3Int(x, y, 0);
                Vector3Int idx2 = new Vector3Int(x, y + 1, 0);
                Vector3Int idx3 = new Vector3Int(x, y + 2, 0);

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

        matchedCandies.Distinct().ToList();
    }
    /// <summary>
    /// ��ġ ĵ��� ����
    /// </summary>
    private void PopMatches()
    {
        //Debug.Log($"���� Pop!");
        foreach (Candy candy in matchedCandies)
        {
            CellContents[candy.CurrentIndex].ContainingCandy = null;
            candy.Pop();
        }

        // ��ġ ��� ����Ʈ �ʱ�ȭ
        matchedCandies.Clear();
    }

    private void EmptyCheck()
    {
        // ĵ�� ���� ����
        for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
        {
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
            {
                var emptyCellIndex = new Vector3Int(x, y, 0);
                // �� �� Ȯ��
                if (CellContents[emptyCellIndex].ContainingCandy == null)
                {
                    emptyCells.Add(emptyCellIndex);
                }
            }
        }
    }

    private void Spawn()
    {
        foreach (Vector3Int emptyCell in emptyCells)
        {
            // emptyCell�� ���� ����� ��� SpawnerPostition �˻�
            Vector3Int closestSpawner = GetClosestSpawner(emptyCell);
            // ���ο� ĵ�� ���� ����
            ActivateSpawnerAt(closestSpawner);
        }
        emptyCells.Clear();
    }

    private void Fall()
    {
        for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
        {
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
            {
                var idx = new Vector3Int(x, y, 0);

                if (CellContents[idx].ContainingCandy == null)
                {
                    // �� �� �߰�
                    var candy = FindCandyToFallInColumn(idx);
                    //Debug.Log($"candy.success:{candy.success}, candy Pos:{Grid.GetCellCenterWorld(candy.idx)}");
                    if (candy.success)
                    {
                        // ĵ�� ���� ����
                        MakeCandyFall(idx, candy.idx);
                    }
                    else
                    {
                        // SpawnCell���� ������ ĵ�� ���� ����
                        MakeSpawnedCandyFall(idx);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���� ������ �˻�. ���� ���� �߰ߵǴ� ĵ���� �� �ε����� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>���н� success�� false�� ��ȯ�մϴ�</returns>
    private (bool success, Vector3Int idx) FindCandyToFallInColumn(Vector3Int emptyCellIndex)
    {
        for (int x = emptyCellIndex.x + 1; x <= m_BoundsInt.xMax; ++x)
        {
            if (CellContents[new Vector3Int(x, emptyCellIndex.y)].ContainingCandy != null)
            {
                // ����.
                return (true, new Vector3Int(x, emptyCellIndex.y));
            }
        }
        // ����. 
        return (false, Vector3Int.zero);
    }

    // 3. Candy ����
    private void MakeCandyFall(Vector3Int emptyCellIndex, Vector3Int candyCellIndex)
    {
        Candy movingCandy = CellContents[candyCellIndex].ContainingCandy;

        // ĵ�� ������Ʈ �̵�
        movingCandy.transform.DOMove(Grid.GetCellCenterWorld(emptyCellIndex), 0.5f).SetEase(Ease.OutBounce);

        // ������ ����
        CellContents[emptyCellIndex].ContainingCandy = movingCandy;
        CellContents[candyCellIndex].ContainingCandy = null;

        // Candy ��ü�� ��ġ ���� ������Ʈ
        movingCandy.UpdateIndex(emptyCellIndex);
    }

    private void MakeSpawnedCandyFall(Vector3Int emptyCellIndex)
    {
        // emptyCell�� ���� ����� ��� SpawnerPostition �˻�
        Vector3Int closestSpawner = GetClosestSpawner(emptyCellIndex);

        if (SpawnerContents[closestSpawner].Count <= 0) return;

        // Spawner�� �������� ���� ������ ĵ��
        Candy newCandy = SpawnerContents[closestSpawner].Dequeue();

        // ĵ�� ������Ʈ �̵�
        newCandy.transform.DOMove(Grid.GetCellCenterWorld(emptyCellIndex), 0.5f).SetEase(Ease.OutBounce);

        // ������ ����
        CellContents[emptyCellIndex].ContainingCandy = newCandy;
        newCandy.UpdateIndex(emptyCellIndex);
    }

    private Vector3Int GetClosestSpawner(Vector3Int idx)
    {
        Vector3Int closestSpawner = Vector3Int.zero;
        foreach (Vector3Int spawnerPosition in SpawnerPosition)
        {
            // y��ǥ�� ���� �Ÿ���
            if (idx.y == spawnerPosition.y)
            {
                // ���� x�� ����� �� ����
                if (closestSpawner.Equals(Vector3Int.zero))
                {
                    closestSpawner = spawnerPosition;
                    continue;
                }

                if (closestSpawner.x - idx.x > spawnerPosition.x - idx.x)
                {
                    closestSpawner = spawnerPosition;
                }
            }
        }
        return closestSpawner;
    }

    // 4. ����
    public void StartSwap(Vector3Int sourceIndex, Vector3Int targetIndex)
    {
        swapSourceIdx = sourceIndex;
        swapTargetIdx = targetIndex;

        UpdateState(State.Swap);
    }

    private void SwapCandies(Vector3Int sourceIndex, Vector3Int targetIndex)
    {
        // ĵ�� ������Ʈ ���� ����
        Candy sourceCandy = CellContents[sourceIndex].ContainingCandy;
        Candy targetCandy = CellContents[targetIndex].ContainingCandy;

        if (sourceCandy == null) return;

        if (targetCandy == null)
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

        if (startingCandy != null)
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
            candyPrefab = LevelData.Instance.CandyPrefabs[Random.Range(0, LevelData.Instance.CandyPrefabs.Length)];

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

    // ĵ�� ���� ������ ĵ�� �����մϴ�
    private void ActivateSpawnerAt(Vector3Int closestSpawner)
    {
        //Debug.Log($"closestSpawner:{closestSpawner}, worldPos:{m_Grid.GetCellCenterWorld(closestSpawner)}");
        var candy = Instantiate(LevelData.Instance.CandyPrefabs[Random.Range(0, LevelData.Instance.CandyPrefabs.Length)], m_Grid.GetCellCenterWorld(closestSpawner), Quaternion.identity);
        candy.Init(closestSpawner);

        if (SpawnerContents.ContainsKey(closestSpawner))
            SpawnerContents[closestSpawner].Enqueue(candy);
        else
        {
            Queue<Candy> candies = new Queue<Candy>();
            candies.Enqueue(candy);
            SpawnerContents.Add(closestSpawner, candies);
        }
    }

    private void UpdateState(State newState)
    {
        state = newState;
        RunStateMachine();
    }


    // ������
    private void PrintCellData()
    {
        Debug.Log($"======================================");

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

        Debug.Log($"======================================");
    }
}