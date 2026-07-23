using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private int _damage = 1;
    [SerializeField] private int _speed = 2;
    [SerializeField] private int _lifetime = 3;
    private float _elapsedTime = 0f;
    private void Update()
    {
        if (_elapsedTime < _lifetime)
        {
            _elapsedTime += Time.deltaTime;
            MoveRpc();
        }
        else
        {
            NetworkObject.Despawn();
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
            NetworkObject.Despawn();
        }
    }
}
