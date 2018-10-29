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

    // draw the path
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

        awaitingPath = true;
        pathfinder.RequestPath(start, end, pathfindingSettings, RecievePath);
    }

    protected void RecievePath(PathResponse response)
    {
        awaitingPath = false;
        lastPath = response;

        OnPathRecieved(response.Successful);
    }

    protected virtual void OnPathRecieved(bool success) { }
}

[System.Serializable]
public struct PathfindingAgentSettings
{
    [SerializeField] private bool _obstaclesTraversable;
    [SerializeField] private bool _nonObstaclesTraversable;

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
