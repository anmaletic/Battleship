using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Vsite.Oom.Battleship.GUI.Models;
using Vsite.Oom.Battleship.Model;

namespace Vsite.Oom.Battleship.GUI.Services;

public interface IGameServerService
{
    string? ConnectionId { get; }
    
    Task<bool> Connect();
    Task<bool> Login(string userName);
    Task<bool> Disconnect();
    
    Task ChallengePlayer(User user);
    Task ChallengeResponse(User challenger, string response);
    Task TransferPlayerData(User target, List<ShipDto> data);
    
    Task AttackPlayer(User user, SquareDto targetCell);
    Task AttackResult(User target, HitResult result);
    Task NotifyGameOver(User target, string gameState);
    
    event Action<List<User>>? AvailablePlayersReceived;
    event Action<User>? ChallengeReceived;
    event Action<User, string>? ChallengeResponseReceived;
    event Action<User, List<ShipDto>>? PlayerDataReceived;
    event Action<User, SquareDto>? AttackReceived;
    event Action<User, HitResult>? AttackResultReceived;
    event Action<User, string>? GameOverReceived;
}

public class GameServerService : IGameServerService
{
    private readonly HubConnection _hubConnection;
    public string? ConnectionId => _hubConnection.ConnectionId;
    
    public event Action<List<User>>? AvailablePlayersReceived;
    public event Action<User>? ChallengeReceived;
    public event Action<User, string>? ChallengeResponseReceived;
    public event Action<User, List<ShipDto>>? PlayerDataReceived;
    public event Action<User, SquareDto>? AttackReceived;
    public event Action<User, HitResult>? AttackResultReceived;
    public event Action<User, string>? GameOverReceived;
    
    public GameServerService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://battleship-server.anmal.dev/gamehub", options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult("YourAccessToken");
                options.Transports = HttpTransportType.WebSockets;
            })
            .WithAutomaticReconnect([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5)])
            .Build();
        
        _hubConnection.On<List<User>>("ReceiveAvailablePlayers", (players) => AvailablePlayersReceived?.Invoke(players));
        _hubConnection.On<User>("ReceiveChallenge", source => ChallengeReceived?.Invoke(source));
        _hubConnection.On<User, string>("ReceiveChallengeResponse", (source, response) => ChallengeResponseReceived?.Invoke(source, response));
        _hubConnection.On<User, SquareDto>("ReceiveAttack", (source, target) => AttackReceived?.Invoke(source, target));
        _hubConnection.On<User, HitResult>("ReceiveAttackResult", (source, result) => AttackResultReceived?.Invoke(source, result));
        _hubConnection.On<User, List<ShipDto>>("ReceivePlayerData", (source, data) => PlayerDataReceived?.Invoke(source, data));
        _hubConnection.On<User, string>("ReceiveGameOver", (source, data) => GameOverReceived?.Invoke(source, data));
    }
    
    public async Task<bool> Connect()
    {
        try
        {
            await _hubConnection.StartAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> Disconnect()
    {        
        try
        {
            await _hubConnection.StopAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> Login(string userName)
    {
        try
        {
            await _hubConnection.InvokeAsync("Login", userName);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task TransferPlayerData(User target, List<ShipDto> data)
    {
        await _hubConnection.InvokeAsync("TransferPlayerData", target.ConnectionId, data);
    }
    public async Task GetAvailablePlayers()
    {
        await _hubConnection.InvokeAsync("GetAvailablePlayers");
    }
    
    public async Task ChallengePlayer(User target)
    {
        await _hubConnection.InvokeAsync("ChallengePlayer", target.ConnectionId);
    }
    public async Task ChallengeResponse(User target, string response)
    {
        await _hubConnection.InvokeAsync("ChallengeResponse", target.ConnectionId, response);
    }
    public async Task AttackPlayer(User target, SquareDto targetCell)
    {
        await _hubConnection.InvokeAsync("AttackPlayer", target.ConnectionId, targetCell);
    }
    public async Task AttackResult(User target, HitResult result)
    {
        await _hubConnection.InvokeAsync("AttackResult", target.ConnectionId, result);
    }
    public async Task NotifyGameOver(User target, string gameState)
    {
        await _hubConnection.InvokeAsync("GameOver", target.ConnectionId, gameState);
    }
    
}
