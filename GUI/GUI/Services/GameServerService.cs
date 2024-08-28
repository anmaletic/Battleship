using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Vsite.Oom.Battleship.GUI.Models;

namespace Vsite.Oom.Battleship.GUI.Services;

public interface IGameServerService
{
    bool IsConnected { get; }
    string? ConnectionId { get; }
    event Action<List<User>>? AvailablePlayersReceived;
    Task<bool> Connect();
    Task<bool> Login(string userName);
    Task GetAvailablePlayers();
}

public class GameServerService : IGameServerService
{
    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
    public string? ConnectionId => _hubConnection.ConnectionId;
    public event Action<List<User>>? AvailablePlayersReceived;
    
    private readonly HubConnection _hubConnection;
    
    public GameServerService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7111/gamehub", options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult("YourAccessToken");
                options.Transports = HttpTransportType.WebSockets;
            })
            .WithAutomaticReconnect([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5)])
            .Build();
        
        _hubConnection.On<List<User>>("ReceiveAvailablePlayers", (players) => AvailablePlayersReceived?.Invoke(players));
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
    
    public async Task GetAvailablePlayers()
    {
        await _hubConnection.InvokeAsync("GetAvailablePlayers");
    }
    
}
