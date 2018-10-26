using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class GridMap
{
    [HideInInspector] public Node[,] grid; // [HideInInspector] attribute is unnecesary as Node isn't serializable, but I'll add it anyway
    [HideInInspector] public float nodeRadius = 0.5f;

    public LayerMask obstructionMask;
    public Vector2 gridSize;
    public Vector3 gridCenter;
    
    public void CreateGrid()
    {
        grid = new Node[(int) gridSize.x, (int) gridSize.y];

        Vector3 bottomLeft = gridCenter - (Vector3.right * gridSize.x / 2) - (Vector3.up * gridSize.y / 2);

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Vector3 position = bottomLeft + Vector3.right * (x + nodeRadius) + Vector3.up * (y + nodeRadius);
                bool obstruction = !Physics2D.OverlapCircle(position, nodeRadius, obstructionMask);

                grid[x, y] = new Node(obstruction, position);
            }
        }
    }

    public Node WorldPositionToNode(Vector3 position)
    {
        float percentX = Mathf.Clamp01((position.x + gridSize.x / 2) / gridSize.x);
        float percentY = Mathf.Clamp01((position.y + gridSize.y / 2) / gridSize.y);

        int x = (int) Mathf.Round((gridSize.x - 1) * percentX);
        int y = (int) Mathf.Round((gridSize.y - 1) * percentY);

        return grid[x, y];
    }
}
