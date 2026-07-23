using System;
using Unity.Netcode;

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
        print("Connecting as Host...");
        NetworkManager.Singleton.StartHost();
        StartGame();
    }
    public void ConnectAsClient()
    {
        print("Connecting as Client...");
        NetworkManager.Singleton.StartClient();
        StartGame();
    }
    public void StartGame()
    {
        print("Starting Game...");
        CurrentState = GameState.RUNNING;
        GameStart?.Invoke();
    }
    public void EndGame()
    {
        print("Ending Game...");
        CurrentState = GameState.POSTGAME;
        GameEnd?.Invoke();
    }
}

public enum GameState
{
    PREGAME,
    RUNNING,
    POSTGAME,
}