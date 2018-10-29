using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridMap _grid;

    [Header("Pathfinding")]
    [SerializeField] private int maxIterations;

    [Header("Multithreading")]
    [SerializeField] private int maxJobs;

    private Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
    private int _currentJobs = 0;

    private void Awake()
    {
        _grid.CreateGrid();
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(_grid.GridCenter, _grid.GridSize);
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

        Thread thread = new Thread(() => FindPath(_requestQueue.Dequeue()));
        thread.Start();
    }

    private void OnPathFound(PathRequest request, bool successful, Vector3[] points)
    {
        PathResponse response = new PathResponse(request.Start, request.End, successful, points);
        request.Callback(response);

        _currentJobs--;
        ProcessNextRequest();
    }

    #endregion

    #region Pathfinding

    private void FindPath(PathRequest request)
    {
        PathfindingAgentSettings settings = request.Settings;
        
        bool success = true;
        Vector3[] path = null;

        Node start = _grid.WorldPositionToNode(request.Start);
        Node end = _grid.WorldPositionToNode(request.End);

        if (!settings.CanTraverse(end))
            success = false;
            
        GenericHeap<Node> openNodes = new GenericHeap<Node>(_grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        List<NodeParentChild> parentChild = new List<NodeParentChild>();

        openNodes.Add(start, settings.Intelligent);
        Node currentNode = null;

        int iterations = 0;

        while (openNodes.Count > 0 && success == true && iterations < maxIterations)
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

                        parentChild.Add(new NodeParentChild(currentNode, node));

                        if (!openNodes.Contains(node))
                        {
                            openNodes.Add(node, settings.Intelligent);
                        }
                        else
                        {
                            openNodes.UpdateItem(node, settings.Intelligent);
                        }
                    }
                }
            }
        }

        if (success)
            path = RetracePath(start, currentNode, parentChild);

        MainThreadDispatcher.Add(() => OnPathFound(request, success, path)); // execute on the main thread
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