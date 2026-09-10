using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLobbyDialog : MonoBehaviour
{
    [SerializeField] private LobbyPlayerView _prefab;
    [SerializeField] private Transform _root;
    [SerializeField] private TextMeshProUGUI _roomCode;

    private Dictionary<string, LobbyPlayerView> _players = new Dictionary<string, LobbyPlayerView>();

    private void OnEnable()
    {
        SessionManager.Instance.ActiveSession.Changed += GenerateViews;
        _roomCode.text = $"Room Code: {SessionManager.Instance.ActiveSession.Code}";
    }

    private void OnDisable()
    {
        SessionManager.Instance.ActiveSession.Changed -= GenerateViews;
    }


    private void GenerateViews()
    {
        foreach (var player in SessionManager.Instance.ActiveSession.Players)
        {
            if (_players.ContainsKey(player.Id)) continue;
            var view = Instantiate(_prefab, _root);
            if (player.Properties.TryGetValue(SessionManager.PlayerNameKey, out var property)) view.Initialize(property.Value);

            _players.Add(player.Id, view);
        }
    }
}

