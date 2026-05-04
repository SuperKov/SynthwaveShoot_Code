using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour
{
    [SerializeField] private Image Panel;
    [SerializeField] private ParticleSystem LoadingEffect;
    [SerializeField] private float WaitAfterEffects;
    public static bool SwitchSceneStarted;

    private static AsyncOperation _currentOperation;

    public static event System.Action LoadingStarted;

    private void Awake()
    {
        PlayerPrefs.SetString("CurrentLevel", SceneManager.GetActiveScene().name);
        LoadingStarted += WaitLoading;
        SwitchSceneStarted = false;
    }

    public static void LoadScene(string sceneName)
    {

        if (SwitchSceneStarted) return;
        _currentOperation = SceneManager.LoadSceneAsync(sceneName);
        _currentOperation.allowSceneActivation = false;
        LoadingStarted.Invoke();
        SwitchSceneStarted = true;
    }

    private void WaitLoading()
    {
        StartCoroutine(WaitLoading_());
    }

    private IEnumerator WaitLoading_()
    {
        Panel.color = new(0, 0, 0, 0);
        Panel.gameObject.SetActive(true);
        LoadingEffect.Play();
        while (!Mathf.Approximately(Panel.color.a, 1f))
        {
            Panel.color += new Color(0, 0, 0, Time.unscaledDeltaTime);
            Panel.color = new(0, 0, 0, Mathf.Clamp(Panel.color.a, 0, 1));
            yield return null;
        }
        yield return new WaitWhile(() => LoadingEffect.isPlaying);
        yield return new WaitForSeconds(WaitAfterEffects);
        _currentOperation.allowSceneActivation = true;
    }

    private void OnDestroy()
    {
        LoadingStarted -= WaitLoading;
    }
}