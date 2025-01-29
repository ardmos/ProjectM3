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

    // 싱글톤 인스턴스
    public static GameBoardManager Instance;

    public const int MIN_MATCH = 3; // 최소 매치 개수

    public List<Vector3Int> SpawnerPosition = new();
    public Dictionary<Vector3Int, Queue<Candy>> SpawnerContents = new();
    // 보드의 각 셀 정보를 저장하는 딕셔너리
    public Dictionary<Vector3Int, GameBoardCell> CellContents = new();
    // 캔디 프리팹들
    //public Candy[] CandyPrefabs;

    public ScoreManager ScoreManager;

    public Grid Grid => m_Grid; // Tilemap의 부모 그리드
    private Grid m_Grid;
    public BoundsInt Bounds => m_BoundsInt;
    // 보드의 경계를 나타내는 변수
    private BoundsInt m_BoundsInt;

    // 젬 타입과 실제 Gem 객체를 매핑하는 딕셔너리
    private Dictionary<CandyType, Candy> m_CandyLookup;

    public State state = State.Init;

    // 스왑 관련 변수
    private Vector3Int swapSourceIdx = new();
    private Vector3Int swapTargetIdx = new();

    // 매치 체크 결과 변수
    private List<Candy> matchedCandies = new List<Candy>();

    // 빈칸 검색 결과 변수
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

    // 보드 초기화 메서드
    public void InitializeBoard()
    {
        // 보드 경계 설정
        SetBoardBounds();
        // 캔디 룩업 테이블 초기화
        InitializeCandyLookup();
        // 보드 생성
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
                // 기본 상태
                //Debug.Log($"Idle State:{state}");
                break;
            case State.MatchCheck:
                CheckMatches();
                // 매치 캔디들 제거
                if (matchedCandies.Count > 0)
                    UpdateState(State.Pop);
                else
                    UpdateState(State.Idle);
                break;
            case State.Pop:
                OnPopped.Invoke(matchedCandies);
                PopMatches();
                // 빈칸 체크 시작
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
                // 스왑 가능여부 체크 로직이 필요함

                SwapCandies(swapSourceIdx, swapTargetIdx);
                // 스왑 후 무브 카운트 증가
                OnMoved.Invoke();
                // 스왑 후 매칭 확인
                UpdateState(State.MatchCheck);
                break;
            default:
                break;
        }
    }

    // 보드 경계 설정
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

    // 캔디 검색용 테이블 초기화
    private void InitializeCandyLookup()
    {
        //candy type과 candy prefab 객체를 매핑합니다. 
        m_CandyLookup = new Dictionary<CandyType, Candy>();
        foreach (var candy in LevelData.Instance.CandyPrefabs)
        {
            m_CandyLookup.Add(candy.CandyType, candy);
        }
    }

    // 0. 생성
    // 최초 보드 생성 메서드. 겹치는 캔디가 없도록 합니다
    private void GenerateBoard()
    {
        // 보드의 각 셀을 순회하며 젬 생성
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

    // 1. 매치
    /// <summary>
    /// 1. 매치 확인 & 제거
    /// </summary>
    private void CheckMatches()
    {
        // 수평 검사
        for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
        {
            for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax - 2; ++x)
            {
                Vector3Int idx = new Vector3Int(x, y, 0);
                Vector3Int idx2 = new Vector3Int(x + 1, y, 0);
                Vector3Int idx3 = new Vector3Int(x + 2, y, 0);

                //Debug.Log($"{Grid.GetCellCenterWorld(idx)}의 캔디를 기준으로 수평 확인");

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

        // 수직 검사
        for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
        {
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax - 2; ++y)
            {
                Vector3Int idx = new Vector3Int(x, y, 0);
                Vector3Int idx2 = new Vector3Int(x, y + 1, 0);
                Vector3Int idx3 = new Vector3Int(x, y + 2, 0);

                //Debug.Log($"{Grid.GetCellCenterWorld(idx)}의 캔디를 기준으로 수직 확인");

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
    /// 매치 캔디들 제거
    /// </summary>
    private void PopMatches()
    {
        //Debug.Log($"사탕 Pop!");
        foreach (Candy candy in matchedCandies)
        {
            CellContents[candy.CurrentIndex].ContainingCandy = null;
            candy.Pop();
        }

        // 매치 결과 리스트 초기화
        matchedCandies.Clear();
    }

    private void EmptyCheck()
    {
        // 캔디 낙하 로직
        for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
        {
            for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
            {
                var emptyCellIndex = new Vector3Int(x, y, 0);
                // 빈 곳 확인
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
            // emptyCell의 가장 가까운 상단 SpawnerPostition 검색
            Vector3Int closestSpawner = GetClosestSpawner(emptyCell);
            // 새로운 캔디 스폰 지시
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
                    // 빈 셀 발견
                    var candy = FindCandyToFallInColumn(idx);
                    //Debug.Log($"candy.success:{candy.success}, candy Pos:{Grid.GetCellCenterWorld(candy.idx)}");
                    if (candy.success)
                    {
                        // 캔디 낙하 시작
                        MakeCandyFall(idx, candy.idx);
                    }
                    else
                    {
                        // SpawnCell에서 생성된 캔디 낙하 시작
                        MakeSpawnedCandyFall(idx);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 윗쪽 셀들을 검색. 가장 먼저 발견되는 캔디의 셀 인덱스를 반환합니다.
    /// </summary>
    /// <returns>실패시 success를 false로 반환합니다</returns>
    private (bool success, Vector3Int idx) FindCandyToFallInColumn(Vector3Int emptyCellIndex)
    {
        for (int x = emptyCellIndex.x + 1; x <= m_BoundsInt.xMax; ++x)
        {
            if (CellContents[new Vector3Int(x, emptyCellIndex.y)].ContainingCandy != null)
            {
                // 성공.
                return (true, new Vector3Int(x, emptyCellIndex.y));
            }
        }
        // 실패. 
        return (false, Vector3Int.zero);
    }

    // 3. Candy 낙하
    private void MakeCandyFall(Vector3Int emptyCellIndex, Vector3Int candyCellIndex)
    {
        Candy movingCandy = CellContents[candyCellIndex].ContainingCandy;

        // 캔디 오브젝트 이동
        movingCandy.transform.DOMove(Grid.GetCellCenterWorld(emptyCellIndex), 0.5f).SetEase(Ease.OutBounce);

        // 데이터 갱신
        CellContents[emptyCellIndex].ContainingCandy = movingCandy;
        CellContents[candyCellIndex].ContainingCandy = null;

        // Candy 객체의 위치 정보 업데이트
        movingCandy.UpdateIndex(emptyCellIndex);
    }

    private void MakeSpawnedCandyFall(Vector3Int emptyCellIndex)
    {
        // emptyCell의 가장 가까운 상단 SpawnerPostition 검색
        Vector3Int closestSpawner = GetClosestSpawner(emptyCellIndex);

        if (SpawnerContents[closestSpawner].Count <= 0) return;

        // Spawner가 보유중인 새로 생성된 캔디
        Candy newCandy = SpawnerContents[closestSpawner].Dequeue();

        // 캔디 오브젝트 이동
        newCandy.transform.DOMove(Grid.GetCellCenterWorld(emptyCellIndex), 0.5f).SetEase(Ease.OutBounce);

        // 데이터 갱신
        CellContents[emptyCellIndex].ContainingCandy = newCandy;
        newCandy.UpdateIndex(emptyCellIndex);
    }

    private Vector3Int GetClosestSpawner(Vector3Int idx)
    {
        Vector3Int closestSpawner = Vector3Int.zero;
        foreach (Vector3Int spawnerPosition in SpawnerPosition)
        {
            // y좌표로 먼저 거르고
            if (idx.y == spawnerPosition.y)
            {
                // 가장 x가 가까운 곳 선택
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

    // 4. 스왑
    public void StartSwap(Vector3Int sourceIndex, Vector3Int targetIndex)
    {
        swapSourceIdx = sourceIndex;
        swapTargetIdx = targetIndex;

        UpdateState(State.Swap);
    }

    private void SwapCandies(Vector3Int sourceIndex, Vector3Int targetIndex)
    {
        // 캔디 오브젝트 참조 저장
        Candy sourceCandy = CellContents[sourceIndex].ContainingCandy;
        Candy targetCandy = CellContents[targetIndex].ContainingCandy;

        if (sourceCandy == null) return;

        if (targetCandy == null)
        {
            // 위치 스왑 애니메이션
            sourceCandy.transform.DOMove(Grid.GetCellCenterWorld(targetIndex), 0.5f).SetEase(Ease.OutBounce);
            sourceCandy.UpdateIndex(targetIndex);
        }
        else
        {
            // 위치 스왑 애니메이션
            sourceCandy.transform.DOMove(Grid.GetCellCenterWorld(targetIndex), 0.5f).SetEase(Ease.OutBounce);
            targetCandy.transform.DOMove(Grid.GetCellCenterWorld(sourceIndex), 0.5f).SetEase(Ease.OutBounce);

            // Candy 객체의 내부 데이터 갱신 (필요한 경우)
            sourceCandy.UpdateIndex(targetIndex);
            targetCandy.UpdateIndex(sourceIndex);

        }

        // CellContents 데이터 갱신
        CellContents[sourceIndex].ContainingCandy = targetCandy;
        CellContents[targetIndex].ContainingCandy = sourceCandy;
    }


    /// <summary>
    ///  Candy Placer Tile로부터 배치 셀을 생성하도록 호출되는 메서드입니다.
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

    // 그리드 참조
    private void GetReference()
    {
        m_Grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    /// <summary>
    /// 새로운 캔디를 배치하는 메서드 입니다.
    /// candyPrefab이 null이면 랜덤 캔디를 배치합니다.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="candyPrefab"></param>
    /// <returns></returns>
    private Candy NewCandyAt(Vector3Int cell, Candy candyPrefab)
    {
        if (candyPrefab == null)
            candyPrefab = LevelData.Instance.CandyPrefabs[Random.Range(0, LevelData.Instance.CandyPrefabs.Length)];

        // 보드가 초기화된 상태인지 확인 후 진행합니다.  
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
    /// 캔디 생성 위치 타일로 캔디 생성 셀을 설정하는 메서드입니다. 
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

    // 캔디 생성 셀에서 캔디를 생성합니다
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


    // 디버깅용
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