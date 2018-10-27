using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour
{
    [SerializeField] protected PathfindingAgentSettings pathfindingSettings;

    protected Pathfinder pathfinder;

    protected virtual void Start()
    {
        InitializePathfindingAgent();
    }

    protected void InitializePathfindingAgent()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    protected Vector3 GetPathHeading(Vector3 target)
    {
        pathfinder.Pathfind(pathfindingSettings, transform.position, target);

        return Vector3.zero;
    }
}

[System.Serializable]
public struct PathfindingAgentSettings
{
    [SerializeField] private bool _canMoveThroughObstacles;
    [SerializeField] private bool _canMoveThroughNonObstacles;

    public bool CanTraverse(Node node)
    {
        if (node.obstruction && _canMoveThroughObstacles)
            return true;
        if (!node.obstruction && _canMoveThroughNonObstacles)
            return true;

        return false;
    }
}
