using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    private const float FadeTime = 0.5f;

    [SerializeField] private Image _transition;
    [SerializeField] private Transform _inMarker;

    private bool _transitioning;

    private void Awake()
    {
        if (SceneLoadInfo.comeFromAnotherScene)
        {
            GameObject.FindWithTag("Player").transform.position = (Vector2)_inMarker.position;
            StartCoroutine(FadeIn());
            SceneLoadInfo.comeFromAnotherScene = false;
        }
    }

    private IEnumerator FadeIn()
    {
        float time = FadeTime;
        while (time > 0)
        {
            time -= Time.deltaTime;

            Color col = _transition.color;
            col.a = time / FadeTime;
            _transition.color = col;

            yield return null;
        }
    }

    private IEnumerator FadeOut(string endScene)
    {
        float elapsed = 0;
        while (elapsed < FadeTime)
        {
            Color col = _transition.color;
            col.a = elapsed / FadeTime;
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
