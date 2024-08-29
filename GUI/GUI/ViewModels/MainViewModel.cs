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
    [ObservableProperty] private UserControl? _currentView;
    [ObservableProperty] private DisplayMessage _message = new();

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<StartGameMessage>(this, (_, _) =>
        {
            StartGame();
        });
        
        WeakReferenceMessenger.Default.Register<ChangeSettingsMessage>(this, (_, _) =>
        {
            DisplayNewGameView();
        });

        WeakReferenceMessenger.Default.Register<DisplayDialogMessage>(this, (_, m) =>
        {
            DisplayDialog(m.Msg);
        });
        
        DisplayNewGameView();
    }

    private void DisplayNewGameView()
    {
        CurrentView = new NewGameView()
        {
            DataContext = Ioc.Default.GetService<NewGameViewModel>(),
            IsVertical = false
        };
    }

    [RelayCommand]
    private void StartGame()
    {
        try
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentView = new GameView { DataContext = Ioc.Default.GetService<GameViewModel>() };
            });

        }
        catch (Exception)
        {
            Message = new DisplayMessage { Title = "Error", Content = "Unable to create game.\nCheck parameters.", IsVisible = true };
        }
    }

    private void DisplayDialog(DisplayMessage msg)
    {
        Message = msg;
    }
}