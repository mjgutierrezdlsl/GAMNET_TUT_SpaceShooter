using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] private PlayerShipController _shipPrefab;
    private Dictionary<ulong, PlayerShipController> _playerShips = new();
    public GameState CurrentState { get; private set; }
    public UnityEvent GameStart, GameEnd;

    private void Start()
    {
        CurrentState = GameState.PREGAME;
    }
    public void StartGame()
    {
        print("Starting Game...");
        CurrentState = GameState.RUNNING;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnShip(clientId);
        }
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
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        StartGame();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        base.OnNetworkDespawn();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        Planet.Instance.Health.OnValueChanged -= OnPlanetHealthChanged;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnShip(clientId);
    }

    private void SpawnShip(ulong clientId)
    {
        var ship = Instantiate(_shipPrefab);
        ship.NetworkObject.SpawnWithOwnership(clientId);
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