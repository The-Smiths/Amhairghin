using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour 
{
    public Transform Target;
    public float SmoothSpeed = 6.25f;
    public Vector3 Offset = new Vector3(0,0,-10);

    private void FixedUpdate()
    {
        var alpha = SmoothSpeed * Time.deltaTime;
        var newPosition = Vector3.Lerp(transform.position, Target.position, alpha);

        transform.position = newPosition + Offset;
    }
}
