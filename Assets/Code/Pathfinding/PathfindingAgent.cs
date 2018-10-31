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

    protected void InitializePathfindingAgent()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    // draws the calculated path
    protected void OnDrawGizmos()
    {
        if (lastPath.Points == null || !lastPath.Successful)
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
    
    protected void RequestPath(Vector3 start, Vector3 end)
    {
        if (awaitingPath)
            return;

        if (lastPath.End == end)
        {
            OnPathReused();
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

    protected virtual void OnPathRecieved(bool success) { }

    protected virtual void OnPathReused() { }
}

[System.Serializable]
public struct PathfindingAgentSettings
{
    [SerializeField] private bool _obstaclesTraversable;
    [SerializeField] private bool _nonObstaclesTraversable;
    [SerializeField] private float _size;

    public bool CanTraverse(Node node)
    {
        if (node.IsObstruction(_size) && _obstaclesTraversable)
            return true;
        if (!node.IsObstruction(_size) && _nonObstaclesTraversable)
            return true;

        return false;
    }
}
