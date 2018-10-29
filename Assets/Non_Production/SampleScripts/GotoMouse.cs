using UnityEngine;

public class GotoMouse : PathfindingAgent
{
    [Header("Goto Mouse")]
    [SerializeField] private float _speed;

    protected override void Start()
    {
        InitializePathfindingAgent();
    }

    private void Update()
    {
        RecalculatePath();

        if (lastPath.Successful && lastPath.Points != null && lastPath.Points.Length > 0)
        { 
            transform.position = Vector3.MoveTowards(transform.position, lastPath.Points[0], _speed * Time.deltaTime);
        }
    }

    private void RecalculatePath()
    {
        RequestPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
