using UnityEngine;
using System.Collections;

public class Cell
{
    public int type;            //TYPES: 0 = Empty, 
    public Vector3 position;    //TYPES: 1 = Obstacle I, 2 = Obstacle II, 3 = Obstacle III,
    public int x, y;            //TYPES: 4 = Start, 5 = End         TODO: Remove 4=Start 5=End, make everything 0=Empty
    public int occupant;        //OCCUPANTS: 0 = Empty, 1 = Enemy, 2 = Player

    public int destructible;    //1 = Will be stomped and destroyed when used: 

    public Agent enemy;
    public Cell parent;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public static Cell NoCell()
    {
        Cell cell = new Cell(Vector3.zero, 666, 0, 0);
        return cell;
    }

    public bool IsCell(int _x, int _y)
    {
        if (_x == x && _y == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCell(Cell c)
    {
        if (c.x == x && c.y == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public Cell(Vector3 _position, int _type, int _x, int _y)
    {
        position = _position;
        type = _type;
        x = _x;
        y = _y;
    }

    public int Distance(Cell cell)
    {
        int distX = Mathf.Abs(cell.x - this.x);
        int distY = Mathf.Abs(cell.y - this.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
