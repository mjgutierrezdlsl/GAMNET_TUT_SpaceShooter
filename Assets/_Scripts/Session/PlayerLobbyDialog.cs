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
    [SerializeField] private Button _readyButton, _startButton;

    private bool _isReady;
    public bool IsReady
    {
        get => _isReady;
        set
        {
            _isReady = value;
            _readyButton.GetComponentInChildren<TextMeshProUGUI>().text = !_isReady ? "Ready" : "Unready";
        }
    }

    private int readyCount;
    public int ReadyCount
    {
        get => readyCount;
        set
        {
            readyCount = value;
            UpdateReadyCount();
        }
    }

    private async void UpdateReadyCount()
    {
        if (!_isHost) return;
        try
        {
            var hostSession = SessionManager.Instance.ActiveSession.AsHost();
            var properties = new Dictionary<string, SessionProperty>
            {
                {SessionManager.KEY_PLAYER_READY,new SessionProperty(readyCount.ToString(),VisibilityPropertyOptions.Private)}
            };
            hostSession.SetProperties(properties);
            await hostSession.SavePropertiesAsync();
            Debug.Log("Session Properties Saved");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private bool _isHost;

    private Dictionary<string, LobbyPlayerView> _playerViews = new Dictionary<string, LobbyPlayerView>();

    private void OnEnable()
    {
        SessionManager.Instance.ActiveSession.Changed += GenerateViews;
        SessionManager.Instance.ActiveSession.PlayerHasLeft += OnPlayerLeft;
        SessionManager.Instance.ActiveSession.PlayerPropertiesChanged += OnPlayerPropertiesChanged;
        _roomCode.text = $"Room Code: {SessionManager.Instance.ActiveSession.Code}";
        _isHost = SessionManager.Instance.ActiveSession.IsHost;
        if (_isHost)
        {
            SessionManager.Instance.ActiveSession.SessionPropertiesChanged += OnSessionPropertiesChanged;
        }
    }

    private void OnDisable()
    {
        SessionManager.Instance.ActiveSession.Changed -= GenerateViews;
        SessionManager.Instance.ActiveSession.PlayerHasLeft -= OnPlayerLeft;
        SessionManager.Instance.ActiveSession.PlayerPropertiesChanged -= OnPlayerPropertiesChanged;
        if (_isHost)
        {
            SessionManager.Instance.ActiveSession.SessionPropertiesChanged -= OnSessionPropertiesChanged;
        }
    }

    private void OnSessionPropertiesChanged()
    {
        if (!_isHost) return;
        SessionManager.Instance.ActiveSession.Properties.TryGetValue(SessionManager.KEY_PLAYER_READY, out var property);
        var count = Int32.Parse(property.Value);
        Debug.Log($"Ready Count: {count}");
        _startButton.gameObject.SetActive(count == SessionManager.Instance.ActiveSession.PlayerCount);
    }

    private void OnPlayerPropertiesChanged()
    {
        foreach (var player in SessionManager.Instance.ActiveSession.Players)
        {
            var view = _playerViews[player.Id];
            if (player.Properties.TryGetValue(SessionManager.KEY_PLAYER_READY, out var property))
            {
                if (property.Value == "true")
                {

                    view.NameLabel.color = Color.green;
                    ReadyCount++;
                }
                else
                {
                    view.NameLabel.color = Color.white;
                    ReadyCount--;
                }
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

