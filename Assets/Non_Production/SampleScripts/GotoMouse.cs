using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoMouse : PathfindingAgent
{
    [SerializeField] private float _speed;

    protected override void Start()
    {
        InitializePathfindingAgent();

        Request();
    }

    protected override void OnPathRecieved(bool success)
    {
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Request();
        }
    }

    private void Request() // helper function to make it easier to request
    {
        // WARNING: FIXME: uncommenting this line can cause an out of memory exception.
        // Problem with path requesting system?

        // RequestPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private IEnumerator FollowPath()
    {
        int targetIndex = 0;
        Vector3 current = lastPath.Points[targetIndex];

        while (true)
        {
            if (transform.position == current)
            {
                targetIndex++;
                if (targetIndex >= lastPath.Points.Length)
                    yield break;

                current = lastPath.Points[targetIndex];
            }

            transform.position = Vector2.MoveTowards(transform.position, current, _speed * Time.deltaTime);
            yield return null;
        }
    }
}
