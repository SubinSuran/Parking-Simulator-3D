using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void FadeToScene(string sceneName) { StartCoroutine(FadeOutAndLoadScene(sceneName)); }
    public void FadeTransition(Action onFadeComplete) { StartCoroutine(FadeInOut(onFadeComplete)); }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) { StartCoroutine(FadeIn()); }

    private IEnumerator FadeInOut(Action onFadeComplete)
    {
        yield return FadeOut();
        onFadeComplete?.Invoke();
        yield return FadeIn();
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.blocksRaycasts = true;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            // Use unscaledDeltaTime to work even when the game is paused
            canvasGroup.alpha = timer / fadeDuration;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            // Use unscaledDeltaTime here as well
            canvasGroup.alpha = 1f - (timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }
}