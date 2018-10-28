using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridMap _grid;

    private Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
    private PathRequest _currentRequest;
    private bool _processingPath = false;

    private void Awake()
    {
        _grid.CreateGrid();
    }

    #region Requesting Paths

    public void RequestPath(Vector3 start, Vector3 end, PathfindingAgentSettings settings, Action<PathResponse> callback)
    {
        PathRequest request = new PathRequest(start, end, settings, callback);
        _requestQueue.Enqueue(request);

        ProcessNextRequest();
    }

    private void ProcessNextRequest()
    {
        if (_processingPath || _requestQueue.Count == 0)
            return;
        
        _currentRequest = _requestQueue.Dequeue();
        _processingPath = true;

        StartCoroutine(FindPath(_currentRequest.Start, _currentRequest.End, _currentRequest.Settings));
    }

    private void OnPathFound(bool successful, Vector3[] points)
    {
        PathResponse response = new PathResponse(_currentRequest.Start, _currentRequest.End, successful, points);
        _currentRequest.Callback(response);

        _processingPath = false;
        ProcessNextRequest();
    }

    #endregion

    #region Pathfinding

    private IEnumerator FindPath(Vector3 startPosition, Vector3 endPosition, PathfindingAgentSettings settings)
    {
        bool success = true;
        Vector3[] path = null;

        Node start = _grid.WorldPositionToNode(startPosition);
        Node end = _grid.WorldPositionToNode(endPosition);

        if (!settings.CanTraverse(start) || !settings.CanTraverse(end))
            success = false;
            
        GenericHeap<Node> openNodes = new GenericHeap<Node>(_grid.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();

        openNodes.Add(start, settings.Intelligent);
        Node currentNode = null;

        while (openNodes.Count > 0 && success == true)
        {
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
                        node.Parent = currentNode;

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

        yield return null;

        if (success)
        {
            path = RetracePath(start, currentNode);
        }

        OnPathFound(success, path);
    }

    private Vector3[] RetracePath(Node start, Node end)
    {
        List<Vector3> points = new List<Vector3>();
        Node currentNode = end;

        while (currentNode.Parent != null)
        {
            points.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        points.Reverse();
        return points.ToArray();
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