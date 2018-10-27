using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public GridMap grid;

    private List<Node> _gridPath; // FIXME: added for testing purposes

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
                Gizmos.color = node.obstruction ? Color.red : Color.blue;

                if (_gridPath != null && _gridPath.Contains(node))
                {
                    Gizmos.color = Color.black;
                }

                Gizmos.DrawCube(node.position, Vector3.one * (grid.nodeRadius * 2 - 0.1f)); //FIXME: doesn't work if grid.nodeRadius is 0.05 or below (sums to 0 or below)
            }
        }
    }

    // uses the A* pathfinding algorithm
    public void Pathfind(PathfindingAgentSettings settings, Vector3 startPos, Vector3 endPos)
    {
        Node start = grid.WorldPositionToNode(startPos);
        Node end = grid.WorldPositionToNode(endPos);

        _gridPath = CreateTrace(settings, start, end);
    }

    private List<Node> CreateTrace(PathfindingAgentSettings settings, Node start, Node end)
    {
        List<Node> openNodes = new List<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();

        openNodes.Add(start);
        Node currentNode = openNodes[0];

        int iterations = 0; //FIXME: remove this, it's just to stop infite while loops in development

        while (openNodes.Count > 0 && iterations < 1000)
        {
            iterations++; 

            currentNode = openNodes[0];
            for (int i = 0; i < openNodes.Count; i++)
            {
                if (openNodes[i].fCost <= currentNode.fCost && openNodes[i].hCost < currentNode.hCost)
                {
                    currentNode = openNodes[i];
				}
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode == end)
                break;
            
            foreach (Node node in grid.GetNeighboringNodes(currentNode))
            {
                if (!node.obstruction && !closedNodes.Contains(node))
                {
                    int newMoveCost = currentNode.gCost + grid.GetDistance(currentNode, node);

                    if (newMoveCost < node.gCost || !openNodes.Contains(node))
                    {
                        node.gCost = newMoveCost;
                        node.hCost = grid.GetDistance(node, end);
                        node.parent = currentNode;

                        if (!openNodes.Contains(node))
                        {
                            openNodes.Add(node);
                        }
                    }
                }
            }
        }

        return TracePath(start, currentNode);
    }

    private List<Node> TracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();

        Node currentNode = end;

        while (currentNode.parent != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }
}
