using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour
{
    [SerializeField] protected PathfindingAgentSettings pathfindingSettings;

    protected Pathfinder pathfinder;
    protected PathResponse lastPath;
    protected bool awaitingPath;

    protected virtual void Start()
    {
        InitializePathfindingAgent();
    }

    protected void OnDrawGizmos()
    {
        if (!lastPath.Successful)
            return;
        
        Gizmos.color = Color.white;
        for (int i = 0; i < lastPath.Points.Length; i++)
        {
            if (i == 0)
            {
                Gizmos.DrawLine(lastPath.Start, lastPath.Points[i]);
            }
            else
            {
                Gizmos.DrawLine(lastPath.Points[i - 1], lastPath.Points[i]);
            }
        }
    }

    protected void InitializePathfindingAgent()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    protected void RequestPath(Vector3 start, Vector3 end)
    {
        if (awaitingPath)
            return;

        if (CanReuseLastPath(start, end))
        {
            StartCoroutine(ReuseLastPath());
            return;
        }

        awaitingPath = true;

        pathfinder.RequestPath(start, end, pathfindingSettings, RecievePath);
    }

    protected void RecievePath(PathResponse response)
    {
        awaitingPath = false;
        lastPath = response;

        OnPathRecieved(response.Successful);
    }

    protected bool CanReuseLastPath(Vector3 start, Vector3 end)
    {
        if (!lastPath.Successful)
            return false;

        return Vector3.Distance(start, lastPath.Start) < pathfindingSettings.MaxReuseStartDist && Vector3.Distance(end, lastPath.End) < pathfindingSettings.MaxReuseEndDist;
    }

    protected IEnumerator ReuseLastPath()
    {
        yield return null; // prevents a stack overflow exception
        OnPathRecieved(true);
    }

    protected virtual void OnPathRecieved(bool success) {}
}

[System.Serializable]
public struct PathfindingAgentSettings
{
    [SerializeField] private bool _obstaclesTraversable;
    [SerializeField] private bool _nonObstaclesTraversable;

    [Space]

    public float MaxReuseStartDist;
    public float MaxReuseEndDist;

    [Space]

    public bool Intelligent;

    public bool CanTraverse(Node node)
    {
        if (node.Obstruction && _obstaclesTraversable)
            return true;
        if (!node.Obstruction && _nonObstaclesTraversable)
            return true;

        return false;
    }
}
