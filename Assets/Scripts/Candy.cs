using UnityEngine;

public class Candy : MonoBehaviour
{
    public CandyType CandyType;
    public int CandyScore;

    public Vector3Int CurrentIndex => m_CurrentIndex;
    protected Vector3Int m_CurrentIndex;

    /// <summary>
    /// ĵ���� �׸���� ��ġ ����(Vector3Int)�� �ʱ�ȭ�մϴ�
    /// </summary>
    public virtual void Init(Vector3Int startIdx)
    {
        m_CurrentIndex = startIdx;
    }

    /// <summary>
    /// ĵ���� �׸���� ��ġ ����(Vector3Int)�� ������Ʈ�մϴ�
    /// </summary>
    public void UpdateIndex(Vector3Int newIndex)
    {
        m_CurrentIndex = newIndex;
    }

    /// <summary>
    /// ĵ�� ������Ʈ�� �ı��մϴ�
    /// </summary>
    public void Pop()
    {      
        Destroy(gameObject);
    }
}
