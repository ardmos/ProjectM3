using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CandyDraggable : MonoBehaviour
{
    private float mZCoord;
    private Candy candy;

    private void Awake()
    {
        candy = GetComponent<Candy>();
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        Vector3 touchPos = GetMouseAsWorldPoint();   

        CandyDragHandler.Instance.OnBeginDrag(touchPos, candy);
    }

    private void OnMouseUp()
    {
        //Debug.Log("OnMouseUp()");
        Vector3 touchPos = GetMouseAsWorldPoint();

        CandyDragHandler.Instance.OnEndDrag(touchPos, candy);
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
