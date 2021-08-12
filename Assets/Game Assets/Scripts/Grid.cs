using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class handle all the logic about deleting rows when a user
// got a full row with blocks. As well the property Grid.grid
// stores on a matrix the state of the game if in given position (x,y)
// is there a block or not.
public class Grid : MonoBehaviour {

    // The grid itself
    public static int w = 10;
    public static int h = 20;
    // grid storing the Transform element
    public static Transform[,] grid = new Transform[w, h];
    public static Transform[] gems2 = new Transform[4];
    // convert a real vector to discret coordinates using Mathf.Round
    public static Vector3 roundVector2(Vector3 v) {
        return new Vector3 (Mathf.Round (v.x), Mathf.Round (v.y));
    }

    // check if some vector is inside the limits of game (borders left, right and down)
    public static bool insideBorder(Vector3 pos) {
        return ((int)pos.x >= 0 &&
                (int)pos.x < w &&
                (int)pos.y >= 0);
    }
    public static bool insideBorder2(Vector3 pos)
    {
        return (int)pos.x >= 0 &&
               (int)pos.x < w &&
               (int)pos.z >= 0 &&
               (int)pos.z < w &&
               (int)pos.y >= 0;
    }
    public static int count = 0;
    // destroy the row at y line
    public static void deleteRow(int y)
    {
        for (int x = 0; x < w; x++) {
            if (grid[x, y] != null)
            {
                ScoreManager.score += (h - y) * 10;
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }


    // Whenever a row was deleted, the above rows should fall towards the bottom by one unit. 
    // The following function will take care of that:
    public static void decreaseRow(int y) {
        for (int x = 0; x < w; x++) {
            if (grid[x, y] != null) {
                grid[x, y - 1] = grid[x, y];
                grid[x,y] = null;
                grid[x, y-1].position += new Vector3(0, -1, 0);
            }
        }
    }

    // whenever a row is deleted, all the above rows should be descreased by 1
    public static void decreaseRowAbove(int y) {
        for (int i = y; i < h; i++) {
            decreaseRow(i);
        }
    }
    public static bool isRowFull2(int x1,int y)
    {
        if (grid[x1, y] == null)
        {
            return false;
        }
        return true;
    }
    // check if a row is full and, then, can be deleted (score +1)
    public static bool isRowFull(int y){
        for (int x = 0; x < w; x++) {
            if (grid[x, y] == null) {
                return false;
            }
        }
        return true;

    }

    public static void deleteFullRows2() 
    {

    }
    public static void deleteFullRows() {
        for (int y = 0; y < h; y++) {
            count = 0;
            if (isRowFull2(4, y))
            {
                gems2[count] = grid[4, y];
                count++; print(count);
                if (count >= 2)
                {
                    deleteRow(y);
                    decreaseRowAbove(y + 1);
                }
                //deleteRow(y); 
                //decreaseRowAbove(y + 1);
            }
            if (isRowFull2(5,y))
            {
                gems2[count] = grid[5, y];
                count++; print(count);
                if (count >= 2)
                {
                    deleteRow(y);
                    decreaseRowAbove(y + 1);
                }
                //deleteRow(y);
                //decreaseRowAbove(y + 1);
            }
            if (isRowFull2(6, y))
            {
                gems2[count] = grid[6, y];
                count++; print(count);
                if (count >= 2)
                {
                    deleteRow(y);
                    decreaseRowAbove(y + 1);
                }
                //deleteRow(y);
                //decreaseRowAbove(y + 1);
            }
            //if (isRowFull(y)) {
            //    
            //    // add new points to score when a row is deleted
            //    ScoreManager.score += (h - y) * 10;
            //    --y;
            //    // NOTE: --y decreases y by one whenever a row was deleted.
            //    // it's to make sure that the next step of the for loop continues
            //    // at the correct index (which must be decreased by one, because we just deleted a row).
            //}else print("isnotfull");
        }
        for (int y = 0; y < h; y++)
        {
             }
    }


//	// Use this for initialization
//	void Start () {
//		
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//	}
}
