using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pathfinder))]
public class PathfinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Pathfinder script = (Pathfinder) target;
        if (GUILayout.Button("Generate Grid"))
        {
            script.grid.CreateGrid();
        }
    }
}
