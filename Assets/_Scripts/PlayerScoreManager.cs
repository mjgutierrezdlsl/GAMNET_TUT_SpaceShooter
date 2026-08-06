using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScoreManager : NetworkSingleton<PlayerScoreManager>
{
    private Dictionary<ulong, NetworkVariable<int>> _playerScores = new();

    [Rpc(SendTo.Server)]
    public void AddPlayerRpc(ulong id)
    {
        print($"Trying to add Player {id}");
        var score = new NetworkVariable<int>();
        score.Initialize(this);
        if (_playerScores.TryAdd(id, score))
        {
            print($"Player {id} added to PlayerScoreManager");
        }
        else
        {
            Debug.LogError($"Failed to add Player {id} to PlayerScoreManager");
        }
    }

    [Rpc(SendTo.Server)]
    public void RemovePlayerRpc(ulong id)
    {
        if (_playerScores.Remove(id))
        {
            print($"Player {id} removed from PlayerScoreManager");
        }
        else
        {
            Debug.LogError($"Failed to remove Player {id} to PlayerScoreManager");

        }
    }

    public void AddScoreRpc(ulong id, int scoreAmount)
    {
        if (_playerScores.TryGetValue(id, out var score))
        {
            score.Value += scoreAmount;
            print($"Player {id} Score: {score.Value}");
        }
        else
        {
            Debug.LogError($"Player {id} not found. {scoreAmount} not added.");
        }
    }
}