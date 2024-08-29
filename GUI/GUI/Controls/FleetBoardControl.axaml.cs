using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Vsite.Oom.Battleship.GUI.Models;

namespace Vsite.Oom.Battleship.GUI.Controls;

public partial class FleetBoardControl : UserControl
{
    public FleetBoardControl()
    {
        InitializeComponent();
    }
    
    public static readonly StyledProperty<ObservableCollection<ObservableCollection<DisplaySquare>>> SourceProperty = 
        AvaloniaProperty.Register<FleetBoardControl, ObservableCollection<ObservableCollection<DisplaySquare>>>(
            nameof(Source));
    public ObservableCollection<ObservableCollection<DisplaySquare>> Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly StyledProperty<double> CellSizeProperty = AvaloniaProperty.Register<FleetBoardControl, double>(
        nameof(CellSize), defaultValue: 30);
    public double CellSize
    {
        get => GetValue(CellSizeProperty);
        set => SetValue(CellSizeProperty, value);
    }

    public static readonly StyledProperty<IImmutableSolidColorBrush> ItemsBackgroundProperty = AvaloniaProperty.Register<FleetBoardControl, IImmutableSolidColorBrush>(nameof(ItemsBackground));
    public IImmutableSolidColorBrush ItemsBackground
    {
        get => GetValue(ItemsBackgroundProperty);
        set => SetValue(ItemsBackgroundProperty, value);
    }
}