using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private LeaderboardEntry _prefab;
    private Dictionary<ulong, LeaderboardEntry> _playerEntries = new();

    public void AddEntry(ulong id)
    {
        // Avoids duplicate entries
        if (_playerEntries.ContainsKey(id)) return;

        var entry = Instantiate(_prefab, transform);
        entry.SetText(id, 0);
        _playerEntries.Add(id, entry);
    }

    public void UpdateScore(ulong id, int score)
    {
        _playerEntries[id].SetText(id, score);
    }
}
