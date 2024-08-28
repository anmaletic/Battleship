using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Vsite.Oom.Battleship.GUI.Models;
using Vsite.Oom.Battleship.GUI.Services;
using Vsite.Oom.Battleship.GUI.ViewModels;
using Vsite.Oom.Battleship.GUI.Views;
using Vsite.Oom.Battleship.Model;

namespace Vsite.Oom.Battleship.GUI;

public partial class App : Application
{
    public App()
    {
        ConfigureServices();
    }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Ioc.Default.GetService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Ioc.Default.GetService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void ConfigureServices()
    {
        Ioc.Default.ConfigureServices(new ServiceCollection()
            .AddTransient<MainViewModel>()
            .AddTransient<GameViewModel>()
            .AddTransient<NewGameViewModel>()
            .AddScoped<SquareEliminator>()
            .AddScoped<IGameServerService, GameServerService>()
            .AddSingleton<Game>()
            .BuildServiceProvider());
    }
}