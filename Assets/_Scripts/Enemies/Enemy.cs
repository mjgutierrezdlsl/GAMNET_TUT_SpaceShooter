using System;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private int _scoreValue = 1;
    [field: SerializeField] public int MaxHealth { get; private set; } = 1;
    private NetworkVariable<int> _currentHealth = new();

    private Rigidbody2D _rb2D;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer || IsHost)
        {
            _currentHealth.Value = MaxHealth;
        }
        _currentHealth.OnValueChanged += OnHealthChanged;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        _currentHealth.OnValueChanged -= OnHealthChanged;
    }


    private void OnHealthChanged(int previousValue, int newValue)
    {
        if (newValue <= 0)
        {
            if (IsServer || IsHost)
            {
                NetworkObject.Despawn();
            }
        }
    }


    private void FixedUpdate()
    {
        if (!IsServer) return;
        UpdatePositionRpc();
    }

    [Rpc(SendTo.Server)]
    private void UpdatePositionRpc()
    {
        Vector2 direction = (Vector3.zero - transform.position).normalized;
        _rb2D.MovePosition(_rb2D.position + direction * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Planet>(out var planet))
        {
            planet.TakeDamage(_damage);
            _currentHealth.Value -= MaxHealth;
        }
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damage, ulong bulletOwnerId)
    {
        _currentHealth.Value -= damage;
        if (_currentHealth.Value <= 0)
        {
            print($"{this} destroyed by Player {bulletOwnerId}'s bullet");
            PlayerScoreManager.Instance.AddScoreRpc(bulletOwnerId, _scoreValue);
        }
    }


}