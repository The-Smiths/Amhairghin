using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool obstruction;
    public Vector3 position;

    public int x;
    public int y;

    public int gCost; // distance from start node
    public int hCost; // distance from end node
    public int fCost { get { return gCost + hCost; } }

    public Node parent;

    public Node(bool _obstruction, Vector3 _position, int _x, int _y)
    {
        obstruction = _obstruction;
        position = _position;
        x = _x;
        y = _y;
    }
}
