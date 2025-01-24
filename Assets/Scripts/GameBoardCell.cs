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
    [SerializeField] private Text candyNumberTextUI;
    [SerializeField] private int candyNumber;
    [SerializeField] private GameObject candyObject;

    private void Awake()
    {
        candyNumberTextUI = GetComponent<Text>();
    }

    // 캔디 터트리기
    public void PopCandy()
    {
        candyNumber = 0;
        Destroy(candyObject);
        candyObject = null;
    }

    public int GetCandyNumber() {  return candyNumber; }
    public Text GetCandyNumberText() { return candyNumberTextUI; }
    public GameObject GetCandyObject() { return candyObject; }
    public RectTransform GetRectTransform() { return GetComponent<RectTransform>(); }
    public void SetCandyNumber(int candyNumber) { this.candyNumber = candyNumber; }
    public void SetCandyNumberText(int candyNumber) { candyNumberTextUI.text = candyNumber.ToString(); }
    public void SetCandyObject(GameObject obj) { candyObject = obj; }
}
