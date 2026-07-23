using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Rigidbody2D _rb2D;
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
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
            print(collision);
            NetworkObject.Despawn();
        }
    }
}