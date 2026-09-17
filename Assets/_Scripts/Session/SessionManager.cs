using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;

public class SessionManager : Singleton<SessionManager>
{
    private ISession _activeSession;
    public ISession ActiveSession
    {
        get => _activeSession;
        set
        {
            _activeSession = value;
            Debug.Log($"ActiveSession: {_activeSession}");
        }
    }

    private string _playerName;

    public const string KEY_PLAYER_READY = "playerReady";

    [SerializeField] private UnityEvent _onConnectSession, _onJoinSession, _onJoinSessionFailed;
    [SerializeField] private UnityEvent<string> _onPlayerNameGet, _onPlayerNameUpdate;

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            _onPlayerNameGet?.Invoke(_playerName);
            Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task DeleteSessionAsync(ISession session)
    {
        if (!session.IsHost)
        {
            Debug.LogWarning("Only the host can delete the session.");
            return;
        }

        try
        {
            await session.AsHost().DeleteAsync();
            Debug.Log("Session deleted successfully.");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            ActiveSession = null;
        }
    }

    public async Task UpdatePlayerName(string playerName)
    {
        try
        {
            _playerName = await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            _onPlayerNameUpdate?.Invoke(_playerName);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task DeleteAccount()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void StartSessionAsHost()
    {
        _onConnectSession?.Invoke();

        try
        {
            var options = new SessionOptions { MaxPlayers = 2 }.WithRelayNetwork().WithPlayerName();
            var session = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Session {session.Id} created! Join code: {session.Code} | Network State: {session.Network.State:G}");
            ActiveSession = session;
            _onJoinSession?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            _onJoinSessionFailed?.Invoke();
        }
    }

    public async void JoinSessionByCode(string code)
    {
        _onConnectSession?.Invoke();
        try
        {
            var options = new JoinSessionOptions().WithPlayerName();
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code, options);
            _onJoinSession?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            _onJoinSessionFailed?.Invoke();
        }
    }

    public async void JoinSessionById(string id)
    {
        _onConnectSession?.Invoke();
        try
        {
            var options = new JoinSessionOptions().WithPlayerName();
            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(id, options);
            Debug.Log($"Session {ActiveSession.Id} joined!");
            _onJoinSession?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            _onJoinSessionFailed?.Invoke();
        }
    }

    public async void KickPlayer(string playerId)
    {
        try
        {
            if (!ActiveSession.IsHost) return;
            await ActiveSession.AsHost().RemovePlayerAsync(playerId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task LeaveSession()
    {
        if (ActiveSession == null) return;
        try
        {
            await ActiveSession.LeaveAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            ActiveSession = null;
        }
    }
}