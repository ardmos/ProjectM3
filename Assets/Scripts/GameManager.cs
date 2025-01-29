using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
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

    [SerializeField] private State state = State.Ready;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 시작하자마자 터지는것들 스코어 포함 안시키려면 여기서 처리해줘야함
        UpdateState(State.Play);
    }

    public void UpdateState(State newState)
    {
        state = newState;

        RunStateMachine();
    }

    public State GetGameState() => state;

    private void RunStateMachine()
    {
        switch (state)
        {
            case State.Ready:
                break;
            case State.Play:
                break;
            case State.Win:
                OnWin.Invoke();
                // 게임 정지 

                // 승리 팝업 
                break;
            case State.Lose:
                OnLose.Invoke(); // Lose시 동작 아직 미구현
                break;
            default:
                Debug.LogError($"유효한 게임 스테이트가 아닙니다.");
                break;
        }
    }
}
