using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Candy : MonoBehaviour
{
    //public int candyNumber;
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
}
