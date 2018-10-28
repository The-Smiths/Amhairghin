using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IGenericHeapItem<Node>
{
    public bool Obstruction;
    public Vector3 Position;

    public int X;
    public int Y;

    public int gCost; // distance from start node
    public int hCost; // distance from end node
    public int fCost { get { return gCost + hCost; } }

    public Node Parent;

    public Node(bool _obstruction, Vector3 _position, int _x, int _y)
    {
        Obstruction = _obstruction;
        Position = _position;
        X = _x;
        Y = _y;
    }

    #region IGenericHeapItem

    public int Index { get; set; }

    public int CompareTo(Node other)
    {
        int result = fCost.CompareTo(other.fCost);

        if (result == 0)
        {
            result = hCost.CompareTo(other.hCost);
        }

        return -result;
    }

    #endregion
}
