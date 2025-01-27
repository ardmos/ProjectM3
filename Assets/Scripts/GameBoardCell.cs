using System;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 게임 보드 셀 클래스
/// 현재 셀에 존재하는 캔디넘버 텍스트값 저장, 반환
/// 현재 셀에 존재하는 캔디 오브젝트 저장, 반환
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
    public Candy IncomingCandy;


    public bool Locked = false;

    public bool CanFall => (ContainingCandy == null || (ContainingCandy.CanMove && ContainingCandy.CurrentMatch == null)) && !Locked;
    public bool BlockFall => Locked || (ContainingCandy != null && !ContainingCandy.CanMove);
    public bool CanBeMoved => !Locked && ContainingCandy != null && ContainingCandy.CanMove;

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
        return ContainingCandy == null && IncomingCandy == null;
    }
}
