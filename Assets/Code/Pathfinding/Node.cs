using UnityEngine;

public class Node : IGenericHeapItem<Node>
{
    public float DistanceFromObstruction;
    public Vector3 Position;

    public readonly int X;
    public readonly int Y;

    public int gCost; // distance from start node
    public int hCost; // distance from end node
    public int fCost { get { return gCost + hCost; } }

    public Node(float _distanceFromObstruction, Vector3 _position, int _x, int _y)
    {
        DistanceFromObstruction = _distanceFromObstruction;
        Position = _position;
        X = _x;
        Y = _y;
    }

    public bool IsObstruction(float distance)
    {
        return DistanceFromObstruction <= distance;
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
