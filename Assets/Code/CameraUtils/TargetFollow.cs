using UnityEngine;

public class TargetFollow : MonoBehaviour 
{
    public Transform Target;
    [SerializeField]private float SmoothSpeed = 6.25f;
    [SerializeField]private Vector2 Offset = new Vector2(0,0);

    private const float Z_OFFSET = -10;

    private void Start()
    {
        transform.position = Target.position + new Vector3(Offset.x, Offset.y, Z_OFFSET);
    }

    private void FixedUpdate()
    {
        float interpolant = SmoothSpeed * Time.deltaTime;
        Vector2 newPosition = Vector2.Lerp(transform.position, Target.position, interpolant) + Offset;

        transform.position = new Vector3(newPosition.x, newPosition.y, Z_OFFSET);
    }
}
