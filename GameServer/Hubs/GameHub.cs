using Microsoft.AspNetCore.SignalR;

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
            Console.WriteLine($"User {user.UserId} disconnected");
        }
        await base.OnDisconnectedAsync(exception);
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
    
    public async Task ChallengePlayer(string userId)
    {
        try
        {
            var user = Users.Find(u => u.UserId == userId);
            if (user?.ConnectionId != null)
            {
                await Clients.Client(user.ConnectionId).SendAsync("ReceiveChallenge", Context.ConnectionId);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task AcceptChallenge(string challengerId)
    {
        try
        {
            var challenger = Users.Find(u => u.ConnectionId == challengerId);
            if (challenger?.ConnectionId != null)
            {
                await Clients.Client(challenger.ConnectionId).SendAsync("ChallengeAccepted", Context.ConnectionId);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task DeclineChallenge(string challengerId)
    {
        try
        {
            var challenger = Users.Find(u => u.ConnectionId == challengerId);
            if (challenger?.ConnectionId != null)
            {
                await Clients.Client(challenger.ConnectionId).SendAsync("ChallengeDeclined", Context.ConnectionId);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task Move(string opponentId, string move)
    {
        try
        {
            var opponent = Users.Find(u => u.UserId == opponentId);
            if (opponent?.ConnectionId != null)
            {
                await Clients.Client(opponent.ConnectionId).SendAsync("ReceiveMove", move);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async Task GameOver(string opponentId, string result)
    {
        try
        {
            var opponent = Users.Find(u => u.UserId == opponentId);
            if (opponent?.ConnectionId != null)
            {
                await Clients.Client(opponent.ConnectionId).SendAsync("ReceiveGameOver", result);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}