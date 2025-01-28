using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class Candy : MonoBehaviour
{
    public int CandyType;

    //public Sprite UISprite; 아이템 효과?
    //When a gem get added to a match, this match get stored here so we can now if this gem is currently in a match and 
    //cannot be used for anything else.
    public Match CurrentMatch = null;

    public Vector3Int CurrentIndex => m_CurrentIndex;
    public int HitPoint => m_HitPoints;
    protected Vector3Int m_CurrentIndex;
    protected int m_HitPoints = 1;



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

    public virtual bool Damage(int damage)
    {
        m_HitPoints -= damage;
        return m_HitPoints > 0;
    }

    public void Destroyed()
    {
        Destroy(gameObject);
    }
}
