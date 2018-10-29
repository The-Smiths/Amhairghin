using System.Collections;
using UnityEngine;

public class GotoMouse : PathfindingAgent
{
    [SerializeField] private float _speed;
    [SerializeField] private float _reachDistance;

    protected override void Start()
    {
        InitializePathfindingAgent();

        Request();
    }

    protected override void OnPathRecieved(bool success)
    {
        StopCoroutine(FollowPath());

        if (success && lastPath.Points != null)
            StartCoroutine(FollowPath());

        //Request();
    }

    private void Request()
    {
        RequestPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private IEnumerator FollowPath()
    {
        int target = 0;

        while (true)
        {
            float distance = Vector3.Distance(transform.position, lastPath.Points[target]);
            transform.position = Vector3.MoveTowards(transform.position, lastPath.Points[target], _speed * Time.deltaTime);

            if (distance <= _reachDistance)
            {
                target++;

                if (target >= lastPath.Points.Length)
                {
                    Request();
                    yield break;
                }
            }

            yield return null;
        }
    }
}
