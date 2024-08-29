using Vsite.Oom.Battleship.GUI.Models;

namespace Vsite.Oom.Battleship.GUI.Messages;

public class DisplayDialogMessage
{
    public  DisplayMessage Msg { get; set; }
    public DisplayDialogMessage(DisplayMessage msg)
    {
        Msg = msg;
    }
}