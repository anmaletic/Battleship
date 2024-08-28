using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Vsite.Oom.Battleship.GUI.Messages;
using Vsite.Oom.Battleship.GUI.Models;
using Vsite.Oom.Battleship.GUI.Views;

namespace Vsite.Oom.Battleship.GUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    public bool IsVertical { get; set; }
    [ObservableProperty] private UserControl? _currentView;
    [ObservableProperty] private DisplayMessage _message = new();

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<ChangeSettingsMessage>(this, (r, m) =>
        {
            DispalyNewGameView();
        });
        
        DispalyNewGameView();
    }

    private void DispalyNewGameView()
    {
        CurrentView = new NewGameView()
        {
            DataContext = Ioc.Default.GetService<NewGameViewModel>(),
            StartGameCommand = StartGameCommand,
            IsVertical = false
        };
    }

    [RelayCommand]
    private void StartGame()
    {
        try
        {
            CurrentView = new GameView { DataContext = Ioc.Default.GetService<GameViewModel>() };
        }
        catch (Exception e)
        {
            Message = new DisplayMessage { Title = "Error", Content = "Unable to create game.\nCheck parameters.", IsVisible = true };
        }
    }
}