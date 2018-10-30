using UnityEngine;

public class GotoMouse : PathfindingAgent
{
    [Header("Goto Mouse")]
    [SerializeField] private float _speed;

    private int _currentPoint = 0;

    protected override void Start()
    {
        InitializePathfindingAgent();
    }

    private void Update()
    {
        RecalculatePath();

        if (!lastPath.Successful || lastPath.Points == null || _currentPoint >= lastPath.Points.Length)
            return;

        transform.position = Vector2.MoveTowards(transform.position, lastPath.Points[_currentPoint], _speed * Time.deltaTime);

        if (transform.position == lastPath.Points[_currentPoint])
        {
            _currentPoint++;
        }
    }

    private void RecalculatePath()
    {
        RequestPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    protected override void OnPathRecieved(bool success)
    {
        _currentPoint = 0;
    }
}
