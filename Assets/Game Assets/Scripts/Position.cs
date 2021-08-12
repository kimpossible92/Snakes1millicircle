using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Position
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Row2 { get; private set; }

    public Position(int row, int column,int r2)
    {
        Row = row;
        Row2 = r2;
        Column = column;
    }
}
