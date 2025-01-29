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
