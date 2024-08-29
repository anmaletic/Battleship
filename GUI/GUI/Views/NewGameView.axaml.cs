using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Vsite.Oom.Battleship.GUI.Views;

public partial class NewGameView : UserControl
{
    public Thickness ControlPadding { get; set; } = new Thickness(40);
    public double LargeFont { get; set; } = 36;
    public double MediumFont { get; set; } = 26;
    public double SmallFont { get; set; } = 16;
    
    public double ButtonWidth { get; set; } = 150;

    public NewGameView()
    {
        if (OperatingSystem.IsAndroid() || IsVertical)
        {
            ControlPadding = new Thickness(10);
            ButtonWidth = 100;
            
            LargeFont = 24;
            MediumFont = 18;
            SmallFont = 12;
        }
        
        InitializeComponent();
    }
    
    public static readonly StyledProperty<bool> IsVerticalProperty = AvaloniaProperty.Register<NewGameView, bool>(
        nameof(IsVertical));

    public bool IsVertical
    {
        get => GetValue(IsVerticalProperty);
        set => SetValue(IsVerticalProperty, value);
    }
}