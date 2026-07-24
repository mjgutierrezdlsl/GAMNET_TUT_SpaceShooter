using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : NetworkBehaviour
{
    [SerializeField] private EnemyController _prefab;
    [SerializeField] private float _minSpawnRate = 1f, _maxSpawnRate = 5f;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameManager.Instance.GameStart += SpawnEnemiesRpc;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameManager.Instance.GameStart -= SpawnEnemiesRpc;
    }

    [Rpc(SendTo.Server)]
    private void SpawnEnemiesRpc()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (GameManager.Instance.CurrentState == GameState.RUNNING)
        {
            var interval = Random.Range(_minSpawnRate, _maxSpawnRate);
            yield return new WaitForSeconds(interval);
            var enemy = Instantiate(_prefab, GetRandomEdgePosition(), Quaternion.identity);
            enemy.NetworkObject.Spawn();
        }
    }

    private Vector2 GetRandomEdgePosition()
    {
        var minBounds = _camera.ScreenToWorldPoint(Vector3.zero);
        var maxBounds = _camera.ScreenToWorldPoint(new(Screen.width, Screen.height));
        var randomizer = Random.Range(0f, 1f);

        return randomizer switch
        {
            < 0.25f => new(minBounds.x, Random.Range(minBounds.y, maxBounds.y)),
            >= 0.25f and < 0.50f => new(maxBounds.x, Random.Range(minBounds.y, maxBounds.y)),
            >= 0.50f and < 0.75f => new(Random.Range(minBounds.x, maxBounds.x), minBounds.y),
            >= 0.75f and <= 1.0f => new(Random.Range(minBounds.x, maxBounds.x), maxBounds.y),
            _ => new(Random.Range(minBounds.x, maxBounds.x), maxBounds.y),
        };
    }

}