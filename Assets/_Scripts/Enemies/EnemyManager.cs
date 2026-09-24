using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    [SerializeField] private Enemy _prefab;
    [SerializeField] private float _minSpawnInterval = 0.2f, _maxSpawnInterval = 1f;
    private List<Enemy> _enemies = new();

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameManager.Instance.GameStart.AddListener(SpawnEnemyRpc);
        GameManager.Instance.StateChanged += OnGameStateChanged;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameManager.Instance.GameStart.RemoveListener(SpawnEnemyRpc);
        GameManager.Instance.StateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.POSTGAME)
        {
            StopAllCoroutines();
            ClearEnemiesRpc();
        }
    }


    [Rpc(SendTo.Server)]
    public void SpawnEnemyRpc()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    [Rpc(SendTo.Server)]
    public void ClearEnemiesRpc()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy == null) continue;
            enemy.GetComponent<NetworkObject>().Despawn();
        }
        _enemies.Clear();
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (GameManager.Instance.CurrentState == GameState.RUNNING)
        {
            var interval = Random.Range(_minSpawnInterval, _maxSpawnInterval);
            yield return new WaitForSeconds(interval);
            var enemy = Instantiate(_prefab, GetRandomEdgePosition(), Quaternion.identity);
            enemy.NetworkObject.Spawn();
            _enemies.Add(enemy);
        }
    }

    private Vector2 GetRandomEdgePosition()
    {
        var maxBounds = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        var minBounds = _camera.ScreenToWorldPoint(Vector3.zero);
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