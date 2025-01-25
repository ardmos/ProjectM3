using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScorePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private ScoreManager scoreManager;

    private void Start()
    {
        GameManager.Instance.OnWin += GameState_OnWin;
        gameObject.SetActive(false);
    }

    private void GameState_OnWin()
    {
        gameObject.SetActive(true);
        scoreText.text = scoreManager.GetCurrentScore().ToString();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
