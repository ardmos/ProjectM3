using UnityEngine;

/// <summary>
/// ���� ���� �� Ŭ����
/// ���� ���� �����ϴ� ĵ��ѹ� �ؽ�Ʈ�� ����, ��ȯ
/// ���� ���� �����ϴ� ĵ�� ������Ʈ ����, ��ȯ
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

    // ĵ�� ��Ʈ����
    public void PopCandy()
    {
        if(candyObject == null) {
            Debug.Log($"PopCandy().candyObject is null");
        }

        Debug.Log($"���� Pop!");
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
