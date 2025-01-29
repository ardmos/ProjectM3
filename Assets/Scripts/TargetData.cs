using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetData 
{
    public CandyType candyType;
    public int count;

    public TargetData(CandyType type, int cnt)
    {
        candyType = type;
        count = cnt;
    }
}
