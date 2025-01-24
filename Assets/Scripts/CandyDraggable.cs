using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CandyDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        CandyDragHandler.Instance.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CandyDragHandler.Instance.OnEndDrag(eventData);
    }
}
