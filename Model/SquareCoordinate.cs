﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vsite.Oom.Battleship.Model
{
    public struct SquareCoordinate
    {
        public readonly int Row;
        public readonly int Column;

        public SquareCoordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
