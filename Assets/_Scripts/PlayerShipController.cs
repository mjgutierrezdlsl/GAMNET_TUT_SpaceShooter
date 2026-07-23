using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipController : NetworkBehaviour
{
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _speed = 2f;

    private float _movementAngle;
    private float MovementAngle
    {
        get => _movementAngle;
        set
        {
            if (!IsOwner) return;
            _movementAngle = value;
            SetPositionAngleRpc(_movementAngle);
        }
    }

    private InputSystem_Actions _input;

    private void Awake()
    {
        _input = new();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        _input.Player.Enable();
        MovementAngle = Random.Range(0f, 360f);
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        _input.Player.Disable();
    }

    private void Update()
    {
        MovementAngle -= _input.Player.Move.ReadValue<float>() * _speed * Time.deltaTime;
    }

    [Rpc(SendTo.Server)]
    private void SetPositionAngleRpc(float angle)
    {
        var position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;
        transform.position = position;
        transform.up = position;
    }
}
