using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public GridMap Grid;

    [Header("Pathfinding")]
    [SerializeField] private int maxIterations;

    [Header("Multithreading")]
    [SerializeField] private int maxJobs;

    private Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
    private int _currentJobs = 0;

    private void Awake()
    {
        Grid.LoadGrid();
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Grid.GridCenter, Grid.GridSize);
    }

    #endregion

    #region Requesting Paths
    
    public void RequestPath(Vector3 start, Vector3 end, PathfindingAgentSettings settings, Action<PathResponse> callback)
    {
        PathRequest request = new PathRequest(start, end, settings, callback);
        _requestQueue.Enqueue(request);
        
        ProcessNextRequest();
    }

    private void ProcessNextRequest()
    {
        if (_currentJobs >= maxJobs || _requestQueue.Count == 0)
            return;

        _currentJobs++;

        //StartCoroutine(FindPath(_requestQueue.Dequeue()));

        Thread thread = new Thread(() => FindPath(_requestQueue.Dequeue()));
        thread.Start();
    }

    private void OnPathFound(PathRequest request, PathResponse response)
    {
        request.Callback(response);

        _currentJobs--;
        ProcessNextRequest();
    }

    #endregion

    #region Pathfinding

    private void FindPath(PathRequest request)
    {
        PathfindingAgentSettings settings = request.Settings;

        bool finished = false;
        Vector3[] path = null;

        Node start = Grid.WorldPositionToNode(request.Start);
        Node end = Grid.WorldPositionToNode(request.End);

        if (!settings.CanTraverse(end))
        {
            finished = true;
            foreach (Node node in Grid.GetNeighboringNodes(end))
            {
                if (settings.CanTraverse(node))
                {
                    end = node;
                    finished = false;
                    break;
                }
            }
        }

        if (!settings.CanTraverse(end))
        {
            finished = true;
            foreach (Node node in Grid.GetNeighboringNodes(start))
            {
                if (settings.CanTraverse(node))
                {
                    start = node;
                    finished = false;
                    break;
                }
            }
        }

        GenericHeap<Node> openNodes = new GenericHeap<Node>(Grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        List<NodeParentChild> parentChild = new List<NodeParentChild>();

        openNodes.Add(start);
        Node currentNode = null;

        int iterations = 0;

        while (openNodes.Count > 0 && !finished && iterations < maxIterations)
        {
            iterations++;

            currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            if (currentNode == end)
                break;
            
            foreach (Node node in Grid.GetNeighboringNodes(currentNode))
            {
                if (settings.CanTraverse(node) && !closedNodes.Contains(node))
                {
                    int newMoveCost = currentNode.gCost + Grid.GetDistance(currentNode, node);

                    if (newMoveCost < node.gCost || !openNodes.Contains(node))
                    {
                        node.gCost = newMoveCost;
                        node.hCost = Grid.GetDistance(node, end);

                        parentChild.Add(new NodeParentChild(currentNode, node));

                        if (!openNodes.Contains(node))
                        {
                            openNodes.Add(node);
                        }
                        else
                        {
                            openNodes.UpdateItem(node);
                        }
                    }
                }
            }
        }

        //yield return new WaitForEndOfFrame();
        
        bool success = currentNode == end;

        if (success)
            path = RetracePath(start, currentNode, parentChild);

        //OnPathFound(request, new PathResponse(request.Start, request.End, success, path));
        MainThreadDispatcher.Add(() => OnPathFound(request, new PathResponse(request.Start, request.End, success, path))); // execute on the main thread
    }

    private Vector3[] RetracePath(Node start, Node end, List<NodeParentChild> parentChild)
    {
        List<Vector3> points = new List<Vector3>();

        Node currentNode = end;
        Node currentParent = GetParent(currentNode, parentChild);

        while (currentParent != null)
        {
            points.Add(currentNode.Position);

            currentNode = currentParent;
            currentParent = GetParent(currentNode, parentChild);
        }

        points.Reverse();
        return points.ToArray();
    }

    private Node GetParent(Node node, List<NodeParentChild> parentChild)
    {
        foreach (NodeParentChild pc in parentChild)
        {
            if (pc.Child == node)
                return pc.Parent;
        }

        return null;
    }
    
    #endregion
}

public struct PathRequest
{
    public Vector3 Start;
    public Vector3 End;
    public PathfindingAgentSettings Settings;
    public Action<PathResponse> Callback;

    public PathRequest(Vector3 _start, Vector3 _end, PathfindingAgentSettings _settings, Action<PathResponse> _callback)
    {
        Start = _start;
        End = _end;
        Settings = _settings;
        Callback = _callback;
    }
}

public struct PathResponse
{
    public Vector3 Start;
    public Vector3 End;
    public bool Successful;
    public Vector3[] Points;

    public PathResponse(Vector3 _start, Vector3 _end, bool _successful, Vector3[] _points)
    {
        Start = _start;
        End = _end;
        Successful = _successful;
        Points = _points;
    }
}

public struct NodeParentChild
{
    public Node Parent;
    public Node Child;

    public NodeParentChild(Node _parent, Node _child)
    {
        Parent = _parent;
        Child = _child;
    }
}