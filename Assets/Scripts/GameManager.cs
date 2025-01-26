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

    [SerializeField] private State gameState = State.Ready;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // �������ڸ��� �����°͵� ���ھ� ���� �Ƚ�Ű���� ���⼭ ó���������
        SetGameState(State.Play);
    }

    public void SetGameState(State newState)
    {
        gameState = newState;

        RunStateMachine();
    }

    public State GetGameState() => gameState;

    private void RunStateMachine()
    {
        switch (gameState)
        {
            case State.Ready:
                break;
            case State.Play:
                break;
            case State.Win:
                OnWin.Invoke();
                // ���� ���� 

                // �¸� �˾� 
                break;
            case State.Lose:
                OnLose.Invoke(); // Lose�� ���� ���� �̱���
                break;
            default:
                Debug.LogError($"��ȿ�� ���� ������Ʈ�� �ƴմϴ�.");
                break;
        }
    }
}
