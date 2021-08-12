using UnityEditor;
using UnityEngine;

public class Block
{
    [SerializeField] public Position GetPosition;
    public plitka Type { get; private set; }
    public Block(Position position, plitka type)
    {
        GetPosition = position;
        this.Type = type;
    }
    public void MoveTo(Position position)
    {
        GetPosition = position;
    }
    public void MoveTo(int row, int column, int r2)
    {
        MoveTo(new Position(row, column, r2));
    }
    public void MoveBy(int rowOffset, int columntOffset,int row2offset)
    {
        MoveTo(GetPosition.Row + rowOffset, GetPosition.Column + columntOffset,GetPosition.Row2+row2offset);
    }
}