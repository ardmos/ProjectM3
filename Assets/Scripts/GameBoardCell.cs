using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 보드 셀 클래스
/// 현재 셀에 존재하는 캔디넘버 텍스트값 저장, 반환
/// 현재 셀에 존재하는 캔디 오브젝트 저장, 반환
/// </summary>
public class GameBoardCell : MonoBehaviour
{
    [SerializeField] private Text candyNumberTextUI; // 디버깅용
    [SerializeField] private int candyNumber;
    [SerializeField] private Candy candyObject;

    public GameBoardCell(GameBoardCell other)
    {
        this.candyNumberTextUI = other.candyNumberTextUI;
        this.candyNumber = other.candyNumber;
        this.candyObject = other.candyObject;
    }

    private void Awake()
    {
        candyNumberTextUI = GetComponent<Text>();
    }

    public void SetNewGameBoardCellData(GameBoardCell other)
    {
        this.candyNumber = other.candyNumber;
        this.candyObject = other.candyObject;

        SetCandyNumberText(this.candyNumber); // 디버깅용
    }

    // 캔디 터트리기
    public void PopCandy()
    {
        if(candyObject == null) {
            Debug.Log($"PopCandy().candyObject is null");
        }

        Debug.Log($"사탕 Pop!");
        candyNumber = 0;
        candyObject.SelfDestroy();
        candyObject = null;
    }

    public int GetCandyNumber() {  return candyNumber; }
    public Text GetCandyNumberText() { return candyNumberTextUI; }
    public Candy GetCandyObject() { return candyObject; }
    public RectTransform GetRectTransform() { return GetComponent<RectTransform>(); }
    public void SetCandyNumber(int candyNumber) { this.candyNumber = candyNumber; }
    public void SetCandyNumberText(int candyNumber) { candyNumberTextUI.text = candyNumber.ToString(); } // 디버깅용
    public void SetCandyObject(Candy obj) { candyObject = obj; }
}
