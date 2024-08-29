using Avalonia.Controls;

namespace Vsite.Oom.Battleship.GUI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Control_OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is not ViewModels.MainViewModel viewModel)
            return;
    }
}