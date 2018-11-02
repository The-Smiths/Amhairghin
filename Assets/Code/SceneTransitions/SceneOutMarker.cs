using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOutMarker : MonoBehaviour
{
    [SerializeField] private string _endScene;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            SceneLoadInfo.comeFromAnotherScene = true;
            GetComponentInParent<SceneTransitionManager>().Transition(_endScene);
        }
    }
}
