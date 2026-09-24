using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] PlayerShipController _playerShipPrefab;
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
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadComplete;
        base.OnNetworkSpawn();
        Planet.Instance.Health.OnValueChanged += OnPlanetHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        base.OnNetworkDespawn();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadComplete;
        Planet.Instance.Health.OnValueChanged -= OnPlanetHealthChanged;
    }

    private void OnSceneLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (var client in clientsCompleted)
        {
            if (NetworkManager.ConnectedClients[client].PlayerObject != null) continue;
            var ship = Instantiate(_playerShipPrefab);
            ship.GetComponent<NetworkObject>().SpawnAsPlayerObject(client, true);
        }
        StartGame();
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