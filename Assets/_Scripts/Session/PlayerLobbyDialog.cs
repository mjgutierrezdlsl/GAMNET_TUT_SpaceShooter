using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyDialog : MonoBehaviour
{
    [SerializeField] private LobbyPlayerView _prefab;
    [SerializeField] private Transform _root;
    
    private Dictionary<string, LobbyPlayerView> _players = new Dictionary<string, LobbyPlayerView>();

    private void OnEnable()
    {
        SessionManager.Instance.ActiveSession.PlayerJoined += GenerateViews;
    }

    private void OnDisable()
    {
        SessionManager.Instance.ActiveSession.PlayerJoined -= GenerateViews;
    }

    private void GenerateViews(string playerId)
    {
        foreach (var player in  SessionManager.Instance.ActiveSession.Players)
        {
            if (_players.ContainsKey(player.Id)) continue;
            var view = Instantiate(_prefab, _root);
            if (player.Properties.TryGetValue(SessionManager.PlayerNameKey, out var property)) view.Initialize(property.Value);
            
            _players.Add(player.Id, view);
        }
    }
}

