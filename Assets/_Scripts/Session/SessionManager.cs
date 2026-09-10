using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager :Singleton<SessionManager> 
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

    public const string PlayerNameKey = "playerName";

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task<Dictionary<string, PlayerProperty>> GetPlayerPropertiesAsync()
    {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { PlayerNameKey, playerNameProperty } };
    }

    private async void OnApplicationQuit()
    {
        if (ActiveSession == null) return;
        try
        {
            await DeleteSessionAsync(ActiveSession);
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

    public async void StartSessionAsHost()
    {
        var playerProperties=await  GetPlayerPropertiesAsync();
        
        try
        {
            var options = new SessionOptions { MaxPlayers = 2 ,PlayerProperties = playerProperties}.WithRelayNetwork();
            var session = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Session {session.Id} created! Join code: {session.Code}");
            ActiveSession = session;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void JoinSessionByCode(string code)
    {
        var properties = await GetPlayerPropertiesAsync();
        try
        {
            var options = new JoinSessionOptions{PlayerProperties = properties};
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code,options);
            Debug.Log($"Session {ActiveSession.Code} joined!");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void JoinSessionById(string id)
    {
        var properties = await GetPlayerPropertiesAsync();
        try
        {
            var options = new JoinSessionOptions{PlayerProperties = properties};
            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(id,options);
            Debug.Log($"Session {ActiveSession.Id} joined!");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
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