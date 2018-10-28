using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridMap _grid;

    private void Start()
    {
        _grid.CreateGrid();
    }

    /*
    // draws the _grid of nodes
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_grid.gridCenter, new Vector3(_grid.gridSize.x, _grid.gridSize.y, 1));

        if (_grid.grid != null)
        {
            foreach (Node node in _grid.grid)
            {
                Gizmos.color = node.obstruction ? Color.red : Color.blue;
                Gizmos.DrawCube(node.position, Vector3.one * (_grid.nodeRadius * 2 - 0.1f)); //FIXME: doesn't work if _grid.nodeRadius is 0.05 or below (sums to 0 or below)
            }
        }
    }
    */

    // uses the A* pathfinding algorithm
    public List<Node> Pathfind(PathfindingAgentSettings settings, Vector3 startPos, Vector3 endPos)
    {
        Node start = _grid.WorldPositionToNode(startPos);
        Node end = _grid.WorldPositionToNode(endPos);

        if (!settings.CanTraverse(end)) // if you can't traverse the end node, just give up
            return null;    // TODO: possibly more elegant system?

        return CreateTrace(settings, start, end);
    }

    private List<Node> CreateTrace(PathfindingAgentSettings settings, Node start, Node end)
    {
        GenericHeap<Node> openNodes = new GenericHeap<Node>(_grid.maxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        openNodes.Add(start, settings.intelligent);
        Node currentNode = null;

        int iterations = 0; //FIXME: remove this, it's just to stop infite while loops in development

        while (openNodes.Count > 0 && iterations < 1000)
        {
            iterations++; 

            currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            if (currentNode == end)
                break;
            
            foreach (Node node in _grid.GetNeighboringNodes(currentNode))
            {
                if (settings.CanTraverse(node) && !closedNodes.Contains(node))
                {
                    int newMoveCost = currentNode.gCost + _grid.GetDistance(currentNode, node);

                    if (newMoveCost < node.gCost || !openNodes.Contains(node))
                    {
                        node.gCost = newMoveCost;
                        node.hCost = _grid.GetDistance(node, end);
                        node.parent = currentNode;

                        if (!openNodes.Contains(node))
                        {
                            openNodes.Add(node, settings.intelligent);
                        }
                        else
                        {
                            openNodes.UpdateItem(node, settings.intelligent);
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
