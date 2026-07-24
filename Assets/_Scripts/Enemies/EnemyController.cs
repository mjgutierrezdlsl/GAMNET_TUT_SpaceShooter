using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour, IDamageable
{
    [SerializeField] private float _speed;

    private Rigidbody2D _rb2D;

    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (_currentHealth <= 0)
            {
                NetworkObject.Despawn();
            }
        }
    }

    [field: SerializeField] public int MaxHealth { get; private set; } = 1;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CurrentHealth = MaxHealth;
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
        if (collision.CompareTag("Planet"))
        {
            CurrentHealth -= MaxHealth;
        }
        if (collision.TryGetComponent<Bullet>(out var bullet))
        {
            print($"Enemy {NetworkObjectId} shot by Player {bullet.NetworkObject.OwnerClientId}");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        CurrentHealth -= damageAmount;
    }
}
