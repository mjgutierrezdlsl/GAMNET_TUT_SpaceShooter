using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyDialog : MonoBehaviour
{
    [SerializeField] private LobbyPlayerView _prefab;
    [SerializeField] private Transform _root;
    [SerializeField] private TextMeshProUGUI _roomCode;
    [SerializeField] private Button _readyButton;
    private bool _isReady;

    public bool IsReady
    {
        get => _isReady;
        set
        {
            _isReady = value;
            _readyButton.interactable = !_isReady;
        }
    }
    private Dictionary<string, LobbyPlayerView> _playerViews = new Dictionary<string, LobbyPlayerView>();

    private void OnEnable()
    {
        SessionManager.Instance.ActiveSession.Changed += GenerateViews;
        SessionManager.Instance.ActiveSession.PlayerHasLeft += OnPlayerLeft;
        SessionManager.Instance.ActiveSession.PlayerPropertiesChanged += OnPlayerPropertiesChanged;
        _roomCode.text = $"Room Code: {SessionManager.Instance.ActiveSession.Code}";
    }

    private void OnDisable()
    {
        SessionManager.Instance.ActiveSession.Changed -= GenerateViews;
        SessionManager.Instance.ActiveSession.PlayerHasLeft -= OnPlayerLeft;
        SessionManager.Instance.ActiveSession.PlayerPropertiesChanged -= OnPlayerPropertiesChanged;
    }

    private void OnPlayerPropertiesChanged()
    {
        foreach (var player in SessionManager.Instance.ActiveSession.Players)
        {
            var view = _playerViews[player.Id];
            if (player.Properties.TryGetValue(SessionManager.KEY_PLAYER_READY, out var property))
            {
                view.NameLabel.color = property.Value == "true" ? Color.green : Color.white;
            }
        }
    }

    private void OnPlayerLeft(string playerId)
    {
        Destroy(_playerViews[playerId].gameObject);
        _playerViews.Remove(playerId);
    }

    private void GenerateViews()
    {
        foreach (var player in SessionManager.Instance.ActiveSession.Players)
        {
            if (_playerViews.ContainsKey(player.Id)) continue;
            var view = Instantiate(_prefab, _root);
            view.Initialize(player.GetPlayerName());

            _playerViews.Add(player.Id, view);
        }
    }

    public async void ToggleReadyState()
    {
        IsReady = !IsReady;
        try
        {
            var properties = new Dictionary<string, PlayerProperty>
            {
                {
                    SessionManager.KEY_PLAYER_READY,
                    new PlayerProperty(
                        IsReady ? "true" : "false",
                        VisibilityPropertyOptions.Member
                    )
                }
            };
            SessionManager.Instance.ActiveSession.CurrentPlayer.SetProperties(properties);
            await SessionManager.Instance.ActiveSession.SaveCurrentPlayerDataAsync();
            print("Player Properties Updated");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void LeaveRoom()
    {
        await SessionManager.Instance.LeaveSession();
    }
}

