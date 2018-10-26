using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool obstruction;
    public Vector3 position;

    public Node(bool _obstruction, Vector3 _position)
    {
        obstruction = _obstruction;
        position = _position;
    }
}
