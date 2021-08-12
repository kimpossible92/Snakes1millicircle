using System.Collections;
using UnityEngine;

public class plitka : MonoBehaviour
{

    public int x1, y, x2;
    public Block GetBlock;
    public GroupView view;
    public void MoveTo(Position position)
    {
        GetPosition = position;
    }
    public void MoveTo(int row, int column)
    {
        MoveTo(new Position(row, column,x2));
    }

    public void MoveBy(int rowOffset, int columntOffset)
    {
        MoveTo(GetPosition.Row + rowOffset, GetPosition.Column + columntOffset);
    }
    public Position GetPosition;
    public void SetPosition(int Xx1, int YY, int Xx2)
    {
        GetPosition = new Position(YY, Xx1, Xx2);
    }
    public void updatepos()
    {
        //if (y == 0)
        //{
            if (x1 == 1 && x2 == 1) { transform.localPosition = new Vector3(2, 0, 1f); }
            if (x1 == 1 && x2 == 0) { transform.localPosition = new Vector3(2, 0, 0f); }
            if (x1 == 0 && x2 == 1) { transform.localPosition = new Vector3(0, 0, 1f); }
            if (x1 == 0 && x2 == 0) { transform.localPosition = new Vector3(0, 0, 0f); }
        //}
        //if (y == 1)
        //{
        //    if (x1 == 1 && x2 == 1) { transform.localPosition = new Vector3(2, 1, 1f); }
        //    if (x1 == 1 && x2 == 0) { transform.localPosition = new Vector3(2, 1, 0f); }
        //    if (x1 == 0 && x2 == 1) { transform.localPosition = new Vector3(0, 1, 1f); }
        //    if (x1 == 0 && x2 == 0) { transform.localPosition = new Vector3(0, 1, 0f); }
        //}
    }
    // Use this for initialization
    void Start()
    {
        //GetPosition = new Position(x1, y, x2);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
