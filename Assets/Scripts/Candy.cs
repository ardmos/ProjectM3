using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class Candy : MonoBehaviour
{
    public CandyType CandyType;
    public int CandyScore;

    public Vector3Int CurrentIndex => m_CurrentIndex;
    protected Vector3Int m_CurrentIndex;

    public void Copy(Candy other)
    {
        CandyType = other.CandyType;
        CandyScore = other.CandyScore;
        m_CurrentIndex = other.m_CurrentIndex;
    }

    public virtual void Init(Vector3Int startIdx)
    {
        m_CurrentIndex = startIdx;
    }

    public void UpdateIndex(Vector3Int newIndex)
    {
        m_CurrentIndex = newIndex;
    }

    // Called when swapping a Gem that have its Usable set to true. SwappedGem will contains the other gem it was swiped
    // with or null if that was a use triggered by a double click
    //deleteSelf will be true in most case but is set to false when a bonus item is used. Bonus item just call Use on
    //a "temporary" gem it hold, and that gem should not be deleted
    public virtual void Use(Candy swappedCandy, bool isBonus = false)
    {

    }

    public void Pop()
    {      
        Destroy(gameObject);
    }
}
