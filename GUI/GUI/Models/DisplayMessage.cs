using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vsite.Oom.Battleship.GUI.Models;

public enum Btn
{
    Ok,
    Yes,
    No,
    YesNo
}

public class DisplayMessage : INotifyPropertyChanged
{
    private bool isVisible;
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public Btn Btn { get; set; } = Btn.Ok;

    public Action<Btn> Closed { get; set; }  = _ => { };

    public bool IsVisible
    {
        get => isVisible;
        set
        {
            if (value == isVisible) return;
            isVisible = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}