using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour 
{
    public Transform Target;
    [SerializeField]private float SmoothSpeed = 6.25f;
    [SerializeField]private Vector3 Offset = new Vector3(0,0,-10);

    private void FixedUpdate()
    {
        float interpolant = SmoothSpeed * Time.deltaTime;
        Vector3 newPosition = Vector3.Lerp(transform.position, Target.position, interpolant);

        transform.position = newPosition + Offset;
    }
}
