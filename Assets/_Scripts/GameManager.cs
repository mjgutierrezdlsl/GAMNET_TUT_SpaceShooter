using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public GameState CurrentState { get; private set; }
    public event Action GameStart, GameEnd;
    [SerializeField] private Planet _planet;

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
    [Rpc(SendTo.Everyone)]
    public void EndGameRpc()
    {
        print("Ending Game...");
        CurrentState = GameState.POSTGAME;
        GameEnd?.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        base.OnNetworkSpawn();
        Planet.Instance.Health.OnValueChanged += OnPlanetHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        base.OnNetworkDespawn();
        Planet.Instance.Health.OnValueChanged -= OnPlanetHealthChanged;
    }

    private void OnPlanetHealthChanged(int previousValue, int newValue)
    {
        print($"Planet: {Planet.Instance.CurrentHealth}/{Planet.Instance.MaxHealth}");
        if (newValue <= 0)
        {
            EndGameRpc();
        }
    }

}

public enum GameState
{
    PREGAME,
    RUNNING,
    POSTGAME,
}