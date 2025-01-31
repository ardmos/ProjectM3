using UnityEngine;

/// <summary>
/// ĵ�� �巡�׵� �� �ֵ��� ������ִ� ������Ʈ�Դϴ�. ĵ�� ������Ʈ�� �߰��Ǿ�� �մϴ�. 
/// �巡�� ���� �Է¸� ó���ϰ�, �׷� ���� ��� ó���� CandyDragHandler���� �ñ�ϴ�.
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
