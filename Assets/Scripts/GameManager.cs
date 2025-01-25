using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Play,
        Win,
        Lose
    }

    public static GameManager Instance;

    public event Action OnReady;
    public event Action OnPlay;
    public event Action OnWin;
    public event Action OnLose;

    [SerializeField] private GameState gameState = GameState.Ready;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // �������ڸ��� �����°͵� ���ھ� ���� �Ƚ�Ű���� ���⼭ ó���������
        SetGameState(GameState.Play);
    }

    public void SetGameState(GameState newState)
    {
        gameState = newState;

        RunStateMachine();
    }

    public GameState GetGameState() => gameState;

    private void RunStateMachine()
    {
        switch (gameState)
        {
            case GameState.Ready:
                break;
            case GameState.Play:
                break;
            case GameState.Win:
                OnWin.Invoke();
                // ���� ���� 

                // �¸� �˾� 
                break;
            case GameState.Lose:
                break;
            default:
                Debug.LogError($"��ȿ�� ���� ������Ʈ�� �ƴմϴ�.");
                break;
        }
    }
}
