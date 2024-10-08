﻿using Microsoft.AspNetCore.SignalR;
using Vsite.Oom.Battleship.GUI.Models;
using Vsite.Oom.Battleship.Model;

namespace GameServer.Hubs;

public class GameHub : Hub
{
    private class User
    {
        public string? UserId { get; set; }
        public string? ConnectionId { get; set; }
    }
    
    private static readonly List<User> Users = [];
    
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"<> User {Context.ConnectionId} connected");
        
        return base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Users.Find(u => u.ConnectionId == Context.ConnectionId);
        if (user != null)
        {
            Users.Remove(user);
            Console.WriteLine($"<> User {user.ConnectionId} - {user.UserId} disconnected");
        }
        await base.OnDisconnectedAsync(exception);
        
        GetAvailablePlayers();
    }
    
    public void Login(string userId)
    {
        try
        {
            Users.RemoveAll(u => u.UserId == userId);
            
            Users.Add(new User()
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId
            });
            
            GetAvailablePlayers();
            
            Console.WriteLine($"<> Logged in user: {userId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public void GetAvailablePlayers()
    {
        try
        {
            Clients.All.SendAsync("ReceiveAvailablePlayers", Users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task ChallengePlayer(string targetId)
    {
        try
        {
            var target = Users.Find(u => u.ConnectionId == targetId);
            var source = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            if (target?.ConnectionId != null)
            {
                await Clients.Client(target.ConnectionId).SendAsync("ReceiveChallenge", source);
            }
            
            Console.WriteLine($"<> {source!.UserId} challenged {target!.UserId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task ChallengeResponse(string targetId, string response)
    {
        try
        {
            var target = Users.Find(u => u.ConnectionId == targetId);
            var source = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            if (target?.ConnectionId != null)
            {
                await Clients.Client(target.ConnectionId).SendAsync("ReceiveChallengeResponse", source, response);
            }
            
            Console.WriteLine($"<> {source!.UserId} responded to {target!.UserId} challenge: {response}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task TransferPlayerData(string targetId, List<ShipDto> data)
    {
        try
        {
            var target = Users.Find(u => u.ConnectionId == targetId);
            var source = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            if (target?.ConnectionId != null)
            {
                await Clients.Client(target.ConnectionId).SendAsync("ReceivePlayerData", source, data);
            }
            
            Console.WriteLine($"<> {source!.UserId} sent player data to {target!.UserId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task AttackPlayer(string targetId, SquareDto targetCell)
    {
        try
        {
            var target = Users.Find(u => u.ConnectionId == targetId);
            var source = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            if (target?.ConnectionId != null)
            {
                await Clients.Client(target.ConnectionId).SendAsync("ReceiveAttack", source, targetCell);
            }
            
            Console.WriteLine($"{source!.UserId} attacked {target!.UserId} {targetCell.Row}, {targetCell.Column}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task AttackResult(string targetId, HitResult result)
    {
        var target = Users.Find(u => u.ConnectionId == targetId);
        var source = Users.Find(u => u.ConnectionId == Context.ConnectionId);
        if (target?.ConnectionId != null)
        {
            await Clients.Client(target.ConnectionId).SendAsync("ReceiveAttackResult", source, result);
        }
        
        Console.WriteLine($"{target!.UserId} attacked {source!.UserId} result: {result}");
    }
    
    public async Task GameOver(string targetId, string result)
    {
        try
        {
            var target = Users.Find(u => u.ConnectionId == targetId);
            var source = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            if (target?.ConnectionId != null)
            {
                await Clients.Client(target.ConnectionId).SendAsync("ReceiveGameOver", source, result);
            }
            
            Console.WriteLine($"<> Game over! {target!.UserId} won against {source!.UserId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}