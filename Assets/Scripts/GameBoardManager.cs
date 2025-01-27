using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[DefaultExecutionOrder(-9999)]
public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager Instance;

    public List<Vector3Int> SpawnerPosition = new();
    public Dictionary<Vector3Int, GameBoardCell> CellContent = new();
    public Candy[] ExistingCandies;
    
    public ScoreManager ScoreManager;

    private List<Vector3Int> m_CellToMatchCheck = new();

    public Grid Grid => m_Grid; // Tilemap의 부모 그리드
    private Grid m_Grid;
    public BoundsInt Bounds => m_BoundsInt;
    private BoundsInt m_BoundsInt;

    private Dictionary<int, Candy> m_CandyLookup;

    private bool m_BoardChanged = false;

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
        //FindAllPossibleMatch();

        //m_BoardWasInit = true;
    }

    private void Update()
    {

        if (m_CellToMatchCheck.Count > 0)
        {
            DoMatchCheck();

            m_BoardChanged = true;
        }
    }

    //generate a gem in every cell, making sure we don't have any match 
    void GenerateBoard()
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

    void DoMatchCheck()
    {
        foreach (var cell in m_CellToMatchCheck)
        {
            //DoCheck(cell);
        }

        m_CellToMatchCheck.Clear();
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
}
