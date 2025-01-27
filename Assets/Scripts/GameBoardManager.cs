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
using Random = UnityEngine.Random;


[DefaultExecutionOrder(-9999)]
public class GameBoardManager : MonoBehaviour
{
    public class PossibleSwap
    {
        public Vector3Int StartPosition;
        public Vector3Int Direction;
    }

    public static GameBoardManager Instance;

    public List<Vector3Int> SpawnerPosition = new();
    public Dictionary<Vector3Int, GameBoardCell> CellContent = new();
    public Candy[] ExistingCandies;
    
    public ScoreManager ScoreManager;

    private List<Vector3Int> m_TickingCells = new();
    private List<Vector3Int> m_NewTickingCells = new();
    private List<Vector3Int> m_CellToMatchCheck = new();
    private List<Match> m_TickingMatch = new();
    private List<PossibleSwap> m_PossibleSwaps = new();
    private int m_PickedSwap;

    public Grid Grid => m_Grid; // Tilemap의 부모 그리드
    private Grid m_Grid;
    public BoundsInt Bounds => m_BoundsInt;
    private BoundsInt m_BoundsInt;

    private Dictionary<int, Candy> m_CandyLookup;
    private Dictionary<Vector3Int, Action> m_CellsCallbacks = new();
    private Dictionary<Vector3Int, Action> m_MatchedCallback = new();

    private bool m_BoardChanged = false;
    private bool m_BoardWasInit = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //candy type과 candy객체를 매핑합니다. 
        m_CandyLookup = new Dictionary<int, Candy>();
        foreach (var candy in ExistingCandies)
        {
            m_CandyLookup.Add(candy.CandyType, candy);
        }

        GenerateBoard();
        FindAllPossibleMatch();

        m_BoardWasInit = true;
    }

    private void Update()
    {
        if (!m_BoardWasInit)
            return;

        if (m_CellToMatchCheck.Count > 0)
        {
            DoMatchCheck();

            m_BoardChanged = true;
        }
    }

    //generate a gem in every cell, making sure we don't have any match 
    private void GenerateBoard()
    {
        m_BoundsInt = new BoundsInt();
        var listOfCells = CellContent.Keys.ToList();

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

        for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
        {
            for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
            {
                var idx = new Vector3Int(x, y, 0);

                if (!CellContent.TryGetValue(idx, out var current) || current.ContainingCandy != null)
                    continue;

                var availableGems = m_CandyLookup.Keys.ToList();

                int leftGemType = -1;
                int bottomGemType = -1;
                int rightGemType = -1;
                int topGemType = -1;

                //check if there is two gem of the same type of the left
                if (CellContent.TryGetValue(idx + new Vector3Int(-1, 0, 0), out var leftContent) &&
                    leftContent.ContainingCandy != null)
                {
                    leftGemType = leftContent.ContainingCandy.CandyType;

                    if (CellContent.TryGetValue(idx + new Vector3Int(-2, 0, 0), out var leftLeftContent) &&
                        leftLeftContent.ContainingCandy != null && leftGemType == leftLeftContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the left, so we can't ue that type anymore
                        availableGems.Remove(leftGemType);
                    }
                }

                //check if there is two gem of the same type below
                if (CellContent.TryGetValue(idx + new Vector3Int(0, -1, 0), out var bottomContent) &&
                    bottomContent.ContainingCandy != null)
                {
                    bottomGemType = bottomContent.ContainingCandy.CandyType;

                    if (CellContent.TryGetValue(idx + new Vector3Int(0, -2, 0), out var bottomBottomContent) &&
                        bottomBottomContent.ContainingCandy != null && bottomGemType == bottomBottomContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the bottom, so we can't ue that type anymore
                        availableGems.Remove(bottomGemType);
                    }

                    if (leftGemType != -1 && leftGemType == bottomGemType)
                    {
                        //if the left and bottom gem are the same type, we need to check if the bottom left gem is ALSO
                        //of the same type, as placing that type here would create a square, which is a valid match
                        if (CellContent.TryGetValue(idx + new Vector3Int(-1, -1, 0), out var bottomLeftContent) &&
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
                if (CellContent.TryGetValue(idx + new Vector3Int(1, 0, 0), out var rightContent) &&
                    rightContent.ContainingCandy != null)
                {
                    rightGemType = rightContent.ContainingCandy.CandyType;

                    //we have the same type on left and right, so placing that type here would create a 3 line
                    if (rightGemType != -1 && leftGemType == rightGemType)
                    {
                        availableGems.Remove(rightGemType);
                    }

                    if (CellContent.TryGetValue(idx + new Vector3Int(2, 0, 0), out var rightRightContent) &&
                        rightRightContent.ContainingCandy != null && rightGemType == rightRightContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the right, so we can't ue that type anymore
                        availableGems.Remove(rightGemType);
                    }

                    //right and bottom gem are the same, check the bottom right to avoid creating a square
                    if (rightGemType != -1 && rightGemType == bottomGemType)
                    {
                        if (CellContent.TryGetValue(idx + new Vector3Int(1, -1, 0), out var bottomRightContent) &&
                            bottomRightContent.ContainingCandy != null && bottomRightContent.ContainingCandy.CandyType == rightGemType)
                        {
                            availableGems.Remove(rightGemType);
                        }
                    }
                }

                //check up
                if (CellContent.TryGetValue(idx + new Vector3Int(0, 1, 0), out var topContent) &&
                    topContent.ContainingCandy != null)
                {
                    topGemType = topContent.ContainingCandy.CandyType;

                    //we have the same type on top and bottom, so placing that type here would create a 3 line
                    if (topGemType != -1 && topGemType == bottomGemType)
                    {
                        availableGems.Remove(topGemType);
                    }

                    if (CellContent.TryGetValue(idx + new Vector3Int(0, 1, 0), out var topTopContent) &&
                        topTopContent.ContainingCandy != null && topGemType == topTopContent.ContainingCandy.CandyType)
                    {
                        //we have two gem of a given type on the top, so we can't ue that type anymore
                        availableGems.Remove(topGemType);
                    }

                    //right and top gem are the same, check the top right to avoid creating a square
                    if (topGemType != -1 && topGemType == rightGemType)
                    {
                        if (CellContent.TryGetValue(idx + new Vector3Int(1, 1, 0), out var topRightContent) &&
                            topRightContent.ContainingCandy != null && topRightContent.ContainingCandy.CandyType == topGemType)
                        {
                            availableGems.Remove(topGemType);
                        }
                    }

                    //left and top gem are the same, check the top left to avoid creating a square
                    if (topGemType != -1 && topGemType == leftGemType)
                    {
                        if (CellContent.TryGetValue(idx + new Vector3Int(-1, 1, 0), out var topLeftContent) &&
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

    private void FindAllPossibleMatch()
    {
        //TODO : instead of going over every gems just do it on moved gems for optimization

        m_PossibleSwaps.Clear();

        //we use a double loop instead of directly querying the cells, so we access them in increasing x then y coordinate
        //this allow to just have to test swapping upward then right, as down and left will have been tested by previous
        //gem already

        for (int y = m_BoundsInt.yMin; y <= m_BoundsInt.yMax; ++y)
        {
            for (int x = m_BoundsInt.xMin; x <= m_BoundsInt.xMax; ++x)
            {
                var idx = new Vector3Int(x, y, 0);
                if (CellContent.TryGetValue(idx, out var cell) && cell.CanBeMoved)
                {
                    var topIdx = idx + Vector3Int.up;
                    var rightIdx = idx + Vector3Int.right;

                    if (CellContent.TryGetValue(topIdx, out var topCell) && topCell.CanBeMoved)
                    {
                        //swap the cell
                        (CellContent[idx].ContainingCandy, CellContent[topIdx].ContainingCandy) = (
                            CellContent[topIdx].ContainingCandy, CellContent[idx].ContainingCandy);

                        if (DoCheck(topIdx, false))
                        {
                            m_PossibleSwaps.Add(new PossibleSwap()
                            {
                                StartPosition = idx,
                                Direction = Vector3Int.up
                            });
                        }

                        if (DoCheck(idx, false))
                        {
                            m_PossibleSwaps.Add(new PossibleSwap()
                            {
                                StartPosition = topIdx,
                                Direction = Vector3Int.down
                            });
                        }

                        //swap back
                        (CellContent[idx].ContainingCandy, CellContent[topIdx].ContainingCandy) = (
                            CellContent[topIdx].ContainingCandy, CellContent[idx].ContainingCandy);
                    }

                    if (CellContent.TryGetValue(rightIdx, out var rightCell) && rightCell.CanBeMoved)
                    {
                        //swap the cell
                        (CellContent[idx].ContainingCandy, CellContent[rightIdx].ContainingCandy) = (
                            CellContent[rightIdx].ContainingCandy, CellContent[idx].ContainingCandy);

                        if (DoCheck(rightIdx, false))
                        {
                            m_PossibleSwaps.Add(new PossibleSwap()
                            {
                                StartPosition = idx,
                                Direction = Vector3Int.right
                            });
                        }

                        if (DoCheck(idx, false))
                        {
                            m_PossibleSwaps.Add(new PossibleSwap()
                            {
                                StartPosition = rightIdx,
                                Direction = Vector3Int.left
                            });
                        }

                        //swap back
                        (CellContent[idx].ContainingCandy, CellContent[rightIdx].ContainingCandy) = (
                            CellContent[rightIdx].ContainingCandy, CellContent[idx].ContainingCandy);
                    }
                }
            }
        }


        m_PickedSwap = Random.Range(0, m_PossibleSwaps.Count);
    }

    /// <summary>
    /// This will return true if a match was found. Setting createMatch to false allow to just check for existing match
    /// which is used by the match finder to check for match possible by swipe 
    /// </summary>
    private bool DoCheck(Vector3Int startCell, bool createMatch = true)
    {
        // in the case we call this with an empty cell. Shouldn't happen, but let's be safe
        if (!CellContent.TryGetValue(startCell, out var centerGem) || centerGem.ContainingCandy == null)
            return false;

        //we ignore that gem if it's already part of another match.
        if (centerGem.ContainingCandy.CurrentMatch != null)
            return false;

        Vector3Int[] offsets = new[]
        {
                Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left
            };

        //First find all the connected gem of the same type
        List<Vector3Int> gemList = new List<Vector3Int>();
        List<Vector3Int> checkedCells = new();

        Queue<Vector3Int> toCheck = new();
        toCheck.Enqueue(startCell);

        while (toCheck.Count > 0)
        {
            var current = toCheck.Dequeue();

            gemList.Add(current);
            checkedCells.Add(current);

            foreach (var dir in offsets)
            {
                var nextCell = current + dir;

                if (checkedCells.Contains(nextCell))
                    continue;

                if (CellContent.TryGetValue(current + dir, out var content)
                    && content.CanMatch()
                    && content.ContainingCandy.CurrentMatch == null
                    && content.ContainingCandy.CandyType == centerGem.ContainingCandy.CandyType)
                {
                    toCheck.Enqueue(nextCell);
                }
            }
        }

        //-- now we build a list of all line of 3+ gems
        List<Vector3Int> lineList = new();

        foreach (var idx in gemList)
        {
            //for each dir (up/down/left/right) if there is no gem in that dir, that mean this could be the start of
            //a matching line, so we check in the opposite direction till we have no more gem
            foreach (var dir in offsets)
            {
                if (!gemList.Contains(idx + dir))
                {
                    var currentList = new List<Vector3Int>() { idx };
                    var next = idx - dir;
                    while (gemList.Contains(next))
                    {
                        currentList.Add(next);
                        next -= dir;
                    }

                    if (currentList.Count >= 3)
                    {
                        lineList = currentList;
                    }
                }
            }
        }

        //no lines match, so there is no match in that.
        if (lineList.Count == 0)
            return false;

        if (createMatch)
        {
            var finalMatch = CreateCustomMatch(startCell);

            foreach (var cell in lineList)
            {
                if (m_MatchedCallback.TryGetValue(cell, out var clbk))
                    clbk.Invoke();

                if (CellContent[cell].CanDelete())
                    finalMatch.AddCandy(CellContent[cell].ContainingCandy);
            }
        }
        return true;
    }

    void DoMatchCheck()
    {
        foreach (var cell in m_CellToMatchCheck)
        {
            DoCheck(cell);
        }

        m_CellToMatchCheck.Clear();
    }

    void MatchTicking()
    {
        for (int i = 0; i < m_TickingMatch.Count; ++i)
        {
            var match = m_TickingMatch[i];

            Debug.Assert(match.MatchingGem.Count == match.MatchingGem.Distinct().Count(),
                "There is duplicate gems in the matching lists");

            const float deletionSpeed = 1.0f / 0.3f;
            match.DeletionTimer += Time.deltaTime * deletionSpeed;

            for (int j = 0; j < match.MatchingGem.Count; j++)
            {
                var gemIdx = match.MatchingGem[j];
                var gem = CellContent[gemIdx].ContainingCandy;

                if (gem == null)
                {
                    match.MatchingGem.RemoveAt(j);
                    j--;
                    continue;
                }

                if (gem.CurrentState == Candy.State.Bouncing)
                {
                    //we stop it bouncing as it is getting destroyed
                    //We check both current and new ticking cells, as it could be the first frame where it started
                    //bouncing so it will be in the new ticking cells NOT in the ticking cell list yet.
                    if (m_TickingCells.Contains(gemIdx)) m_TickingCells.Remove(gemIdx);
                    if (m_NewTickingCells.Contains(gemIdx)) m_NewTickingCells.Remove(gemIdx);

                    gem.transform.position = m_Grid.GetCellCenterWorld(gemIdx);
                    gem.transform.localScale = Vector3.one;
                    gem.StopBouncing();
                }

                //forced deletion doesn't wait for end of timer
                if (match.ForcedDeletion || match.DeletionTimer > 1.0f)
                {
                    Destroy(CellContent[gemIdx].ContainingCandy.gameObject);
                    CellContent[gemIdx].ContainingCandy = null;

                    // 장애물 추가 코드
/*                    if (match.ForcedDeletion && CellContent[gemIdx].Obstacle != null)
                    {
                        CellContent[gemIdx].Obstacle.Clear();
                    }*/

                    //callback are only called when this was a match from swipe and not from bonus or other source 
                    if (!match.ForcedDeletion && m_CellsCallbacks.TryGetValue(gemIdx, out var clbk))
                    {
                        clbk.Invoke();
                    }

                    match.MatchingGem.RemoveAt(j);
                    j--;

                    match.DeletedCount += 1;
                    //we only spawn coins for non bonus match
                    if (match.DeletedCount >= 4 && !match.ForcedDeletion)
                    {
                        GameManager.Instance.ChangeCoins(1);
                        //GameManager.Instance.PoolSystem.PlayInstanceAt(GameManager.Instance.Settings.VisualSettings.CoinVFX, gem.transform.position);
                    }

                    if (match.SpawnedBonus != null && match.OriginPoint == gemIdx)
                    {
                        NewCandyAt(match.OriginPoint, match.SpawnedBonus);
                    }
                    else
                    {
                        m_EmptyCells.Add(gemIdx);
                    }

                    //
                    if (gem.CurrentState != Gem.State.Disappearing)
                    {
                        LevelData.Instance.Matched(gem);

                        foreach (var matchEffectPrefab in gem.MatchEffectPrefabs)
                        {
                            GameManager.Instance.PoolSystem.PlayInstanceAt(matchEffectPrefab, m_Grid.GetCellCenterWorld(gem.CurrentIndex));
                        }

                        gem.gameObject.SetActive(false);

                        gem.Destroyed();
                    }
                }
                else if (gem.CurrentState != Gem.State.Disappearing)
                {
                    LevelData.Instance.Matched(gem);

                    foreach (var matchEffectPrefab in gem.MatchEffectPrefabs)
                    {
                        GameManager.Instance.PoolSystem.PlayInstanceAt(matchEffectPrefab, m_Grid.GetCellCenterWorld(gem.CurrentIndex));
                    }

                    gem.gameObject.SetActive(false);

                    gem.Destroyed();
                }
            }

            if (match.MatchingGem.Count == 0)
            {
                m_TickingMatch.RemoveAt(i);
                i--;
            }
        }
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

        if (!Instance.CellContent.ContainsKey(cellPosition))
            Instance.CellContent.Add(cellPosition, new GameBoardCell());
        
        if(startingCandy != null)
            Instance.NewCandyAt(cellPosition, startingCandy);
    }

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
            candyPrefab = ExistingCandies[Random.Range(0, ExistingCandies.Length)];

        // 매치 이펙트 실행 준비 (Visual Effect 사용 버전)
/*        if (candyPrefab.MatchEffectPrefabs.Length != 0)
        {
            foreach (var matchEffectPrefab in candyPrefab.MatchEffectPrefabs)
            {
                GameManager.Instance.PoolSystem.AddNewInstance(matchEffectPrefab, 16);
            }
        }*/

        // 보드가 초기화된 상태인지 확인 후 진행합니다.  
        if (CellContent[cell].ContainingCandy != null)
        {
            Destroy(CellContent[cell].ContainingCandy.gameObject);
        }

        var candy = Instantiate(candyPrefab, Grid.GetCellCenterWorld(cell), Quaternion.identity);
        CellContent[cell].ContainingCandy = candy;
        candy.Init(cell);

        return candy;
    }

    /// <summary>
    /// 캔디 생성 위치 타일로 캔디 생성 위치를 설정하는 메서드입니다. 
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

    //useful for bonus, this will create a new match and you can add anything you want to it.
    public Match CreateCustomMatch(Vector3Int newCell)
    {
        var newMatch = new Match()
        {
            DeletionTimer = 0.0f,
            MatchingGem = new(),
            OriginPoint = newCell,
            //SpawnedBonus = null
        };

        m_TickingMatch.Add(newMatch);

        return newMatch;
    }
}
