using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridMap
{
    [HideInInspector] public Node[,] grid; // [HideInInspector] attribute is unnecesary as Node isn't serializable, but I'll add it anyway
    [HideInInspector] public float nodeRadius { get { return 0.5f / nodesPerUnit; } } 
    [HideInInspector] public int maxSize { get { return (int) (gridSize.x * gridSize.y); } }

    public Vector2 gridSize;
    public Vector3 gridCenter;

    [Space]

    public LayerMask obstructionMask;
    public int nodesPerUnit;
    
    public void CreateGrid()
    {
        grid = new Node[(int) gridSize.x * nodesPerUnit, (int) gridSize.y * nodesPerUnit];

        Vector3 bottomLeft = gridCenter - (Vector3.right * gridSize.x / 2) - (Vector3.up * gridSize.y / 2);

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Vector3 position = bottomLeft + (Vector3.right * (x + nodeRadius) + Vector3.up * (y + nodeRadius)) / nodesPerUnit;
                bool obstruction = Physics2D.OverlapCircle(position, nodeRadius, obstructionMask);

                grid[x, y] = new Node(obstruction, position, x, y);
            }
        }
    }

    public Node WorldPositionToNode(Vector3 position)
    {
        float percentX = Mathf.Clamp01((position.x + gridSize.x / 2) / gridSize.x);
        float percentY = Mathf.Clamp01((position.y + gridSize.y / 2) / gridSize.y);

        int x = (int) Mathf.Round((gridSize.x - 1) * percentX * nodesPerUnit);
        int y = (int) Mathf.Round((gridSize.y - 1) * percentY * nodesPerUnit);

        return grid[x, y];
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

                int X = node.x + x;
                int Y = node.y + y;

                if (X < 0 || Y < 0 || X >= grid.GetLength(0) || Y >= grid.GetLength(1))
                    continue;
                
                nodes.Add(grid[X, Y]);
            }
        }

        return nodes;
    }

    public int GetDistance(Node a, Node b)
    {
        int y = Mathf.Abs(a.x - b.x);
        int x = Mathf.Abs(a.y - b.y);

        if (x > y) 
            return (4 * y) + (10 * x);
        
        return (4 * x) + (10 * y);
    }
}
