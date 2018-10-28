using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoMouse : PathfindingAgent
{
    [SerializeField] private float _speed;

    private void Update()
    {
        Vector3 heading = GetPathHeading(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
