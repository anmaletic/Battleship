using Avalonia;
using Avalonia.Controls;
using Vsite.Oom.Battleship.GUI.Models;

namespace Vsite.Oom.Battleship.GUI.Controls;

public partial class MessageControl : UserControl
{
    public MessageControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<DisplayMessage> MessageProperty = AvaloniaProperty.Register<MessageControl, DisplayMessage>(
        nameof(Message));

    public DisplayMessage Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public void CloseCommand(Btn btn)
    {
        Message.Closed(btn);
        Message.IsVisible = false;
    }
}