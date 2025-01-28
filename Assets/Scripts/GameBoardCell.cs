using System;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// ���� ���� �� Ŭ����
/// ���� ���� �����ϴ� ĵ��ѹ� �ؽ�Ʈ�� ����, ��ȯ
/// ���� ���� �����ϴ� ĵ�� ������Ʈ ����, ��ȯ
/// </summary>
public class GameBoardCell
{
    public static readonly Vector3Int[] Neighbours =
    {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

    public Candy ContainingCandy;


    public bool Locked = false;

    public bool CanMatch()
    {
        return ContainingCandy != null;
    }

    public bool CanDelete()
    {
        return !Locked;
    }

    public bool IsEmpty()
    {
        return ContainingCandy == null;
    }
}
