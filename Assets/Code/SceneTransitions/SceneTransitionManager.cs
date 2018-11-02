using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Image _transition;
    [SerializeField] private Transform _inMarker;

    private bool _transitioning;

    private void Start()
    {
        if (SceneLoadInfo.comeFromAnotherScene)
        {
            GameObject.FindWithTag("Player").transform.position = (Vector2)_inMarker.position;
        }
        SceneLoadInfo.comeFromAnotherScene = false; 
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float startTime = 2;
        float time = startTime;

        while (time > 0)
        {
            time -= Time.deltaTime;

            Color col = _transition.color;
            col.a = time / startTime;
            _transition.color = col;

            yield return null;
        }
    }

    private IEnumerator FadeOut(string endScene)
    {
        _transitioning = true;

        float time = 2;
        float elapsed = 0;

        while (elapsed < time)
        {
            Color col = _transition.color;
            col.a = elapsed / time;
            _transition.color = col;

            elapsed += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadSceneAsync(endScene);
    }

    public void Transition(string endScene)
    {
        if (_transitioning)
            return;

        _transitioning = true;
        StartCoroutine(FadeOut(endScene));
    }
}
