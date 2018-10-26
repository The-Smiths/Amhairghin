using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public GridMap grid;

    private void Start()
    {
        grid.CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(grid.gridCenter, new Vector3(grid.gridSize.x, grid.gridSize.y, 1));

        if (grid.grid != null)
        {
            foreach (Node node in grid.grid)
            {
                Gizmos.color = node.obstruction ? Color.blue : Color.red;
                Gizmos.DrawCube(node.position, Vector3.one * (grid.nodeRadius * 2 - 0.1f));
            }
        }
    }
}
