/// <summary>
/// 스테이지 클리어 목표인 타켓의 정보를 담는 클래스입니다.
/// </summary>
[System.Serializable]
public class TargetData 
{
    public CandyType CandyType;
    public int Count;

    public TargetData(CandyType type, int cnt)
    {
        CandyType = type;
        Count = cnt;
    }
}
