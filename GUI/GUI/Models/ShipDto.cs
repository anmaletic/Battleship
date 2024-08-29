using System.Collections.Generic;
using Vsite.Oom.Battleship.Model;

namespace Vsite.Oom.Battleship.GUI.Models;

public class SquareDto
{
    public int Row { get; set; }
    public int Column { get; set; }
    public SquareState SquareState { get; set; }
}

public class ShipDto
{
    public List<SquareDto> Squares { get; set; } = [];
}