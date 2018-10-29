using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridMap
{
    [HideInInspector] public Node[,] Grid; // [HideInInspector] atrribute is unnecessary on these, but I'll add it anyway
    [HideInInspector] public float NodeRadius { get { return 0.5f / _nodesPerUnit; } } 
    [HideInInspector] public int MaxSize { get { return (int) (GridSize.x * GridSize.y); } }

    public Vector3 GridCenter;
    public Vector2 GridSize;

    [SerializeField] private LayerMask _obstructionMask;
    [SerializeField] private int _nodesPerUnit;
    
    public void CreateGrid()
    {
        Grid = new Node[(int) GridSize.x * _nodesPerUnit, (int) GridSize.y * _nodesPerUnit];
        Vector3 bottomLeft = GridCenter - (Vector3.right * GridSize.x / 2) - (Vector3.up * GridSize.y / 2);

        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                Vector3 position = bottomLeft + (Vector3.right * (x + NodeRadius) + Vector3.up * (y + NodeRadius)) / _nodesPerUnit;
                bool obstruction = Physics2D.OverlapCircle(position, NodeRadius, _obstructionMask);

                Grid[x, y] = new Node(obstruction, position, x, y);
            }
        }
    }

    public Node WorldPositionToNode(Vector3 position)
    {
        float percentX = Mathf.Clamp01((position.x + GridSize.x / 2) / GridSize.x);
        float percentY = Mathf.Clamp01((position.y + GridSize.y / 2) / GridSize.y);

        int x = (int) Mathf.Round((GridSize.x - 1) * percentX * _nodesPerUnit);
        int y = (int) Mathf.Round((GridSize.y - 1) * percentY * _nodesPerUnit);

        return Grid[x, y];
    }

    public List<Node> GetNeighboringNodes(Node node)
    {
        List<Node> nodes = new List<Node>();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int X = node.X + x;
                int Y = node.Y + y;

                if (X < 0 || Y < 0 || X >= Grid.GetLength(0) || Y >= Grid.GetLength(1))
                    continue;
                
                nodes.Add(Grid[X, Y]);
            }
        }

        return nodes;
    }

    public int GetDistance(Node a, Node b)
    {
        int y = Mathf.Abs(a.X - b.X);
        int x = Mathf.Abs(a.Y - b.Y);

        if (x > y) 
            return (4 * y) + (10 * x);
        
        return (4 * x) + (10 * y);
    }
}
