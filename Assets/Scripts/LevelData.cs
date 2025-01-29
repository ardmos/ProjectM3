using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance;

    public int Level;
    [Header("Ŭ���� �̼� ���")]
    public List<TargetData> TargetList = new();
    [Header("�ܰ躰 ��Ÿ ����")]
    public int[] TargetScore = new int[3];
    [Header("������ ���� Ƚ��")]
    public int MoveMaxCount;
    [Header("�̹� ���������� �����ų ĵ�� ���")]
    public Candy[] CandyPrefabs;

    private void Awake()
    {
        Instance = this;
    }
}
