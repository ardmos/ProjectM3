using UnityEngine;

public class Candy : MonoBehaviour
{
    public CandyType CandyType;
    public int CandyScore;

    public Vector3Int CurrentIndex => m_CurrentIndex;
    protected Vector3Int m_CurrentIndex;

    /// <summary>
    /// 캔디의 그리드상 위치 정보(Vector3Int)를 초기화합니다
    /// </summary>
    public virtual void Init(Vector3Int startIdx)
    {
        m_CurrentIndex = startIdx;
    }

    /// <summary>
    /// 캔디의 그리드상 위치 정보(Vector3Int)를 업데이트합니다
    /// </summary>
    public void UpdateIndex(Vector3Int newIndex)
    {
        m_CurrentIndex = newIndex;
    }

    /// <summary>
    /// 캔디 오브젝트를 파괴합니다
    /// </summary>
    public void Pop()
    {      
        Destroy(gameObject);
    }
}
