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
using Random = UnityEngine.Random;


[DefaultExecutionOrder(-9999)]
public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager Instance;

    public Grid Grid; // Tilemap의 부모 그리드
    public Dictionary<Vector3Int, GameBoardCell> CellContent = new();
    public Candy[] ExistingCandies;


    private void Awake()
    {
        Instance = this;
    }






    /// <summary>
    ///  Candy Placer Tile로부터 배치 셀을 생성하도록 호출되는 메서드입니다.
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <param name="startingCandy"></param>
    public static void RegisterCell(Vector3Int cellPosition, Candy startingCandy = null)
    {
        //Not super happy with that, but Startup is called before all Awake....
        if (Instance == null)
        {
            Instance = GameObject.Find("GameBoardManager").GetComponent<GameBoardManager>();
            Instance.GetReference();
        }

        if (!Instance.CellContent.ContainsKey(cellPosition))
            Instance.CellContent.Add(cellPosition, new GameBoardCell());
        
        Instance.NewCandyAt(cellPosition, startingCandy);
    }

    private void GetReference()
    {
        Grid = GameObject.Find("Grid").GetComponent<Grid>();
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
}
