using UnityEngine;

/// <summary>
/// 게임 보드 셀 클래스
/// 현재 셀에 존재하는 캔디넘버 텍스트값 저장, 반환
/// 현재 셀에 존재하는 캔디 오브젝트 저장, 반환
/// </summary>
public class GameBoardCell:MonoBehaviour
{
    [SerializeField] private int candyNumber;
    [SerializeField] private Candy candyObject;

    public GameBoardCell(GameBoardCell other)
    {
        this.candyNumber = other.candyNumber;
        this.candyObject = other.candyObject;
    }

    public void SetNewGameBoardCellData(GameBoardCell other)
    {
        this.candyNumber = other.candyNumber;
        this.candyObject = other.candyObject;
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
    public Candy GetCandyObject() { return candyObject; }
    public RectTransform GetRectTransform() { return GetComponent<RectTransform>(); }
    public void SetCandyNumber(int candyNumber) { this.candyNumber = candyNumber; }
    public void SetCandyObject(Candy obj) { candyObject = obj; }
}
