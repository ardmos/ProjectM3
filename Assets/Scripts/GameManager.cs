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
        // 시작하자마자 터지는것들 스코어 포함 안시키려면 여기서 처리해줘야함
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
                // 게임 정지 

                // 승리 팝업 
                break;
            case GameState.Lose:
                break;
            default:
                Debug.LogError($"유효한 게임 스테이트가 아닙니다.");
                break;
        }
    }
}
