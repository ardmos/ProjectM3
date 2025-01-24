using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ���� �� Ŭ����
/// ���� ���� �����ϴ� ĵ��ѹ� �ؽ�Ʈ�� ����, ��ȯ
/// ���� ���� �����ϴ� ĵ�� ������Ʈ ����, ��ȯ
/// </summary>
public class GameBoardCell : MonoBehaviour
{
    [SerializeField] private Text candyNumberTextUI;
    [SerializeField] private int candyNumber;
    [SerializeField] private GameObject candyObject;

    private void Awake()
    {
        candyNumberTextUI = GetComponent<Text>();
    }

    // ĵ�� ��Ʈ����
    public void PopCandy()
    {
        candyNumber = 0;
        Destroy(candyObject);
        candyObject = null;
    }

    public int GetCandyNumber() {  return candyNumber; }
    public Text GetCandyNumberText() { return candyNumberTextUI; }
    public GameObject GetCandyObject() { return candyObject; }
    public RectTransform GetRectTransform() { return GetComponent<RectTransform>(); }
    public void SetCandyNumber(int candyNumber) { this.candyNumber = candyNumber; }
    public void SetCandyNumberText(int candyNumber) { candyNumberTextUI.text = candyNumber.ToString(); }
    public void SetCandyObject(GameObject obj) { candyObject = obj; }
}
