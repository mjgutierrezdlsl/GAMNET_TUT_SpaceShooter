using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour, IDamageable
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private int _damage = 1;
    [field: SerializeField] public int MaxHealth { get; private set; } = 1;
    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            if (!IsServer) return;
            _currentHealth = value;
            if (_currentHealth <= 0)
            {
                NetworkObject.Despawn();
            }
        }
    }

    private Rigidbody2D _rb2D;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }
    public override void OnNetworkSpawn()
    {
        _currentHealth = MaxHealth;
        base.OnNetworkSpawn();
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
        if (collision.TryGetComponent<IDamageable>(out var damageable))
        {
            TakeDamage(MaxHealth);
            damageable.TakeDamage(_damage);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        CurrentHealth -= damageAmount;
    }
}