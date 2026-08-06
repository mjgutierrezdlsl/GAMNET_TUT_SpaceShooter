using System;
using Unity.Netcode;
using UnityEngine;

public class Planet : NetworkSingleton<Planet>, IDamageable
{
    public NetworkVariable<int> Health { get; } = new();
    public int CurrentHealth
    {
        get => Health.Value;
        private set
        {
            Health.Value = value;
        }
    }

    [field: SerializeField] public int MaxHealth { get; private set; } = 10;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        base.OnNetworkSpawn();
        GameManager.Instance.GameStart += OnGameStart;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        base.OnNetworkDespawn();
        GameManager.Instance.GameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        TakeDamageRpc(damageAmount);
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damageAmount)
    {
        CurrentHealth -= damageAmount;
    }
}