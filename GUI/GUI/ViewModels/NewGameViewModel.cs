using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Vsite.Oom.Battleship.GUI.Messages;
using Vsite.Oom.Battleship.GUI.Models;
using Vsite.Oom.Battleship.GUI.Services;

namespace Vsite.Oom.Battleship.GUI.ViewModels;

public partial class NewGameViewModel : ViewModelBase
{
    private readonly IGameServerService _gameServer;

    [ObservableProperty] private Game _game;
    [ObservableProperty] private bool isAiViewVisible = true;
    [ObservableProperty] private bool isPlayerViewVisible;
    [ObservableProperty] private bool isConnected;
    [ObservableProperty] private ObservableCollection<User> availablePlayers = new();
    [ObservableProperty] private string userName = "";
    [ObservableProperty] private string startButtonText = "Start Game";
    [ObservableProperty] private User? _selectedPlayer;

    public ObservableCollection<DisplayDifficulty> Difficulties { get; } =
    [
        new DisplayDifficulty { Difficulty = GameDifficulty.Easy },
        new DisplayDifficulty { Difficulty = GameDifficulty.Normal },
        new DisplayDifficulty { Difficulty = GameDifficulty.Hard }
    ];

    public NewGameViewModel() : this(null!, null!)
    {
    }
    public NewGameViewModel(Game game, IGameServerService gameServer)
    {
        this._gameServer = gameServer;
        gameServer.AvailablePlayersReceived += AvailablePlayersReceived;
        gameServer.ChallengeReceived += ChallengeReceived;
        gameServer.ChallengeResponseReceived += ChallengeResponseReceived;

        _game = game;

        Difficulties.First(x => x.Difficulty == Game.Difficulty).IsSelected = true;
    }


    private void ChallengeReceived(User source)
    {
        Game.OpponentUser = source;

        var msg = new DisplayMessage()
        {
            Title = "New challenge!",
            Content = $"You are being challenged by {source.UserId}!\nDo you accept?",
            Btn = Btn.YesNo,
            Closed = ChallengeResponse,
            IsVisible = true
        };

        WeakReferenceMessenger.Default.Send(new DisplayDialogMessage(msg));
    }
    private void ChallengeResponse(Btn btn)
    {
        switch (btn)
        {
            case Btn.Yes:
                _gameServer.ChallengeResponse(Game.OpponentUser, "Accept");
                Game.IsPlayerTurn = true;
                
                var msg = new DisplayMessage()
                {
                    Title = "Response!",
                    Content = "Challenge accepted",
                    Btn = Btn.Ok,
                    IsVisible = true
                };
                WeakReferenceMessenger.Default.Send(new DisplayDialogMessage(msg));

                WeakReferenceMessenger.Default.Send(new StartGameMessage());
                WeakReferenceMessenger.Default.UnregisterAll(this);
                break;
            case Btn.No:
                _gameServer.ChallengeResponse(Game.OpponentUser, "Decline");
                break;
        }
    }
    private void ChallengeResponseReceived(User source, string response)
    {
        Game.OpponentUser = source;
        Game.IsPlayerTurn = false;

        if (response == "Accept")
        {
            var msg = new DisplayMessage()
            {
                Title = "Response!",
                Content = "Challenge accepted",
                Btn = Btn.Ok,
                IsVisible = true
            };
            WeakReferenceMessenger.Default.Send(new DisplayDialogMessage(msg));

            WeakReferenceMessenger.Default.Send(new StartGameMessage());
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
        else
        {
            var msg = new DisplayMessage()
            {
                Title = "Response!",
                Content = "Challenge declined",
                Btn = Btn.Ok,
                IsVisible = true
            };
            WeakReferenceMessenger.Default.Send(new DisplayDialogMessage(msg));
        }
    }


    [RelayCommand]
    private void SetDifficulty(GameDifficulty difficulty)
    {
        Game.Difficulty = difficulty;
    }


    // change opponent type to enum
    [RelayCommand]
    private void SetOpponentType(string opponentType)
    {
        Game.OpponentType = opponentType;

        if (opponentType == "AI")
        {
            IsAiViewVisible = true;
            IsPlayerViewVisible = false;
            StartButtonText = "Start Game";
        }
        else
        {
            IsAiViewVisible = false;
            IsPlayerViewVisible = true;
            StartButtonText = "Challenge Player";
        }
    }

    [RelayCommand]
    private async Task Connect(string action)
    {
        switch (action)
        {
            case "Connect":
                try
                {
                    IsConnected = await _gameServer.Connect();
                    if (IsConnected)
                    {
                        var isLoggedIn = await _gameServer.Login(UserName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            case "Disconnect":
                await _gameServer.Disconnect();
                IsConnected = false;
                break;
        }
    }

    // method activated by SignalR
    private void AvailablePlayersReceived(List<User> players)
    {
        var userToRemove = players.FirstOrDefault(x => x.ConnectionId == _gameServer.ConnectionId);
        if (userToRemove != null)
        {
            players.Remove(userToRemove);
        }

        AvailablePlayers = new ObservableCollection<User>(players);
    }

    [RelayCommand]
    private void StartGame()
    {
        if (IsAiViewVisible)
        {
            WeakReferenceMessenger.Default.Send(new StartGameMessage());
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
        else
        {
            if (SelectedPlayer is not null)
            {
                Game.OpponentUser = SelectedPlayer;
                _gameServer.ChallengePlayer(SelectedPlayer);
            }
        }
    }
}