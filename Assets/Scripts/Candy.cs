using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Candy : MonoBehaviour
{
    public Vector3Int CurrentIndex => m_CurrentIndex;
    protected Vector3Int m_CurrentIndex;

    public int CandyType;

    private int score = 10;

    public void SelfDestroy(Action<bool> callback)
    {
        Destroy(gameObject);

        callback(true);
    }

    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }

    public int GetScore() {  return score; }


    public virtual void Init(Vector3Int startIdx)
    {
        m_CurrentIndex = startIdx;
    }
}
