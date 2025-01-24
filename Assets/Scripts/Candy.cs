using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }
}
