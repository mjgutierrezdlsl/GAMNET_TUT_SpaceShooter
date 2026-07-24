using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public GameState CurrentState { get; private set; }
    public event Action GameStart, GameEnd;

    private void Start()
    {
        CurrentState = GameState.PREGAME;
    }

    public void ConnectAsHost()
    {
        print("Connecting as Host");
        NetworkManager.Singleton.StartHost();
        StartGame();
    }

    public void ConnectAsClient()
    {
        print("Connecting as Client");
        NetworkManager.Singleton.StartClient();
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.RUNNING;
        GameStart?.Invoke();
    }

    public void EndGame()
    {
        CurrentState = GameState.POSTGAME;
        GameEnd?.Invoke();
    }
}

public enum GameState
{
    PREGAME,
    RUNNING,
    POSTGAME
}