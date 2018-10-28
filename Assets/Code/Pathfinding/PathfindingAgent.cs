using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour
{
    [SerializeField] protected PathfindingAgentSettings pathfindingSettings;

    protected Pathfinder pathfinder;
    protected SerializedPath lastPath;

    protected virtual void Start()
    {
        InitializePathfindingAgent();
    }

    // draws the calculated path
    protected void OnDrawGizmos()
    {
        if (lastPath == null)
            return;
        
        Gizmos.color = Color.white;
        for (int i = 0; i < lastPath.points.Count; i++)
        {
            if (i == 0)
            {
                Gizmos.DrawLine(lastPath.start, lastPath.points[i].position);
            }
            else
            {
                Gizmos.DrawLine(lastPath.points[i - 1].position, lastPath.points[i].position);
            }
        }
    }

    protected void InitializePathfindingAgent()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    // get the direction you need to go to reach the next point of the path
    protected Vector3 GetPathHeading(Vector3 target)
    {
        List<Node> path = GetPath(transform.position, target);

        if (path == null)
            return Vector3.zero;

        return (transform.position - path[0].position).normalized;
    }

    protected List<Node> GetPath(Vector3 start, Vector3 end)
    {
        // check if we can cheat and just return the last path to save some performance
        if (!(lastPath != null && Vector3.Distance(end, lastPath.end) < pathfindingSettings.maxReuseEndDist && Vector3.Distance(start, lastPath.start) < pathfindingSettings.maxReuseStartDist))
        {
            SerializedPath path = new SerializedPath(pathfinder.Pathfind(pathfindingSettings, transform.position, end), start, end);

            if (path.points != null)
            {
                lastPath = path;
            }
        }

        return lastPath.points;
    }
}

public class SerializedPath
{
    public List<Node> points;
    public Vector3 start;
    public Vector3 end;

    public SerializedPath(List<Node> _points, Vector3 _start, Vector3 _end)
    {
        points = _points;
        start = _start;
        end = _end;
    }
}

[System.Serializable]
public struct PathfindingAgentSettings
{
    [SerializeField] private bool _obstaclesTraversable;
    [SerializeField] private bool _nonObstaclesTraversable;

    [Space]

    public float maxReuseStartDist; // the max distance a serialized path's start can be from the wanted path for it to be reused
    public float maxReuseEndDist; // the max distance a serialized path's end can be from the wanted path for it to be reused

    [Space]

    public bool intelligent; // if it always takes the most efficient path

    public bool CanTraverse(Node node)
    {
        if (node.obstruction && _obstaclesTraversable)
            return true;
        if (!node.obstruction && _nonObstaclesTraversable)
            return true;

        return false;
    }
}
