using System.Collections.Generic;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Vsite.Oom.Battleship.GUI.Messages;
using Vsite.Oom.Battleship.GUI.Models;
using Vsite.Oom.Battleship.GUI.Services;
using Vsite.Oom.Battleship.Model;

namespace Vsite.Oom.Battleship.GUI.ViewModels;

public partial class GameViewModel : ViewModelBase
{
    private readonly IGameServerService _gameServer;
    [ObservableProperty] private Game _game;
    [ObservableProperty] private bool _isPlayerTurn = true;
    [ObservableProperty] private string _message = "";

    public GameViewModel() : this(null!, null!)
    {
    }
    public GameViewModel(Game game, IGameServerService gameServer)
    {
        _game = game;
        _gameServer = gameServer;
        _gameServer.AttackReceived += AttackReceived;
        _gameServer.AttackResultReceived += AttackResultReceived;
        _gameServer.PlayerDataReceived += PlayerDataReceived;
        _gameServer.GameOverReceived += GameOverReceived;
        
        _game.NewGame();

        var ships = Game.Player.Fleet.Ships;
        List<ShipDto> shipsDto = [];
        
        foreach (var ship in ships)
        {
            var shipDto = new ShipDto();
            foreach (var square in ship.Squares)
            {
                shipDto.Squares.Add(new SquareDto()
                {
                    Column = square.Column,
                    Row = square.Row,
                    SquareState = square.SquareState
                });
            }
            
            shipsDto.Add(shipDto);
        }
        
        _gameServer.TransferPlayerData(Game.OpponentUser, shipsDto);

        IsPlayerTurn = Game.IsPlayerTurn;
    }
    private void GameOverReceived(User source, string gameState)
    {
        Message = gameState;
    }

    private void PlayerDataReceived(User source, List<ShipDto> ships)
    {
        Game.Ships = ships;
    }
    
    private void AttackResultReceived(User source, HitResult result)
    {
        DisplaySquare target = Game.Player.ShotsBoard[_targetCell.X][_targetCell.Y];
        Game.OpponentAttackResult(result, target);
    }
    
    private void AttackReceived(User source, Point targetCell)
    {
        IsPlayerTurn = true;
        DisplaySquare target = Game.Player.FleetBoard[targetCell.X][targetCell.Y];
        
        var result = Game.OpponentPlayerAttack(target);
        _gameServer.AttackResult(Game.OpponentUser, result);
        
        if (Game.IsOpponentWinner())
        {
            Message = "You lost!";
            _gameServer.NotifyGameOver(Game.OpponentUser, "You won!");
        }
    }

    Point _targetCell = new Point();

    [RelayCommand(CanExecute = nameof(CanShoot))]
    private void Shoot(DisplaySquare target)
    {
        if (Game.OpponentType == "AI")
        {
            Game.PlayerAttack(target);

            if (Game.IsGameOver(true))
            {
                Message = "You won!";
                return;
            }

            OpponentShoot();
        }
        else
        {
            if (!IsPlayerTurn) return;
            
            _targetCell = new Point(target.Row, target.Column);
            _gameServer.AttackPlayer(Game.OpponentUser, new Point(target.Row, target.Column));
            IsPlayerTurn = false;
        }
    }
    
    
    private bool CanShoot(DisplaySquare? square)
    {
        return square?.SquareState == SquareState.Intact;
    }

    private void OpponentShoot()
    {
        Game.OpponentAiAttack();

        if (Game.IsGameOver(false))
        {
            Message = "You lost!";
        }
    }


    // Start new game with the same settings
    [RelayCommand]
    private void NewGame()
    {
        Message = "";
        Game.NewGame();
    }

    // Change game settings
    [RelayCommand]
    private void ChangeSettings()
    {
        Message = "";
        WeakReferenceMessenger.Default.Send(new ChangeSettingsMessage());
    }
}