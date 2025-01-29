using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScorePanelController : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public GameObject[] StarObjects = new GameObject[3];
    public TextMeshProUGUI ScoreText;
    public ScoreManager ScoreManager;
    public Button CloseButton; // �������� ���� ������ �̵�
    public Button RetryButton; // �� �������� �����
    public Button NextButton; // ���� ��������

    private void Start()
    {
        GameManager.Instance.OnWin += GameState_OnWin;
        
        InitPopup();
    }

    private void InitPopup()
    {
        TitleText.text = $"Level{LevelData.Instance.Level}";
        ScoreText.text = "0";
        gameObject.SetActive(false);
    }

    private void GameState_OnWin()
    {
        gameObject.SetActive(true);
        ScoreText.text = ScoreManager.GetCurrentScore().ToString();
        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(i < ScoreManager.StarScore);
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
