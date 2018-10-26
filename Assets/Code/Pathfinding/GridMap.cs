using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private LayerMask _obstructionMask;
    [SerializeField] private Vector2 _gridSize;
    
    private float _nodeRadius = 0.5f;
    private Node[,] _grid;

    private void Start()
    {
        CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridSize.x, _gridSize.y, 1));

        if (_grid != null)
        {
            foreach (Node node in _grid)
            {
                Gizmos.color = node.obstruction ? Color.blue : Color.red;
                Gizmos.DrawCube(node.position, Vector3.one * (_nodeRadius * 2 - 0.1f));
            }
        }
    }

    private void CreateGrid()
    {
        _grid = new Node[(int) _gridSize.x, (int) _gridSize.y];

        Vector3 bottomLeft = transform.position - (Vector3.right * _gridSize.x / 2) - (Vector3.up * _gridSize.y / 2);

        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                Vector3 position = bottomLeft + Vector3.right * (x + _nodeRadius) + Vector3.up * (y + _nodeRadius);
                bool obstruction = !Physics2D.OverlapCircle(position, _nodeRadius, _obstructionMask);

                _grid[x, y] = new Node(obstruction, position);
            }
        }
    }
}
