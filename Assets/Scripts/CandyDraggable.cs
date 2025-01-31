using UnityEngine;

/// <summary>
/// 캔디가 드래그될 수 있도록 만들어주는 컴포넌트입니다. 캔디 오브젝트에 추가되어야 합니다. 
/// 드래그 관련 입력만 처리하고, 그로 인한 결과 처리는 CandyDragHandler에게 맡깁니다.
/// </summary>
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
