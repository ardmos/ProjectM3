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
        // �������ڸ��� �����°͵� ���ھ� ���� �Ƚ�Ű���� ���⼭ ó���������
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
