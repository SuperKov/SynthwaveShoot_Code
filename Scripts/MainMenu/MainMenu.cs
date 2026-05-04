using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource GameMusic;
    [SerializeField] private float SwitchMusicSpeed;

    private Coroutine _currentSwitchMusicPitch;

    public void SwitchMusicPitch(float to)
    {
        if (_currentSwitchMusicPitch != null)
            StopCoroutine(_currentSwitchMusicPitch);
        _currentSwitchMusicPitch = StartCoroutine(SwitchMusicPitch_(to));
    }

    private IEnumerator SwitchMusicPitch_(float to)
    {
        while (!Mathf.Approximately(GameMusic.pitch, to))
        {
            GameMusic.pitch = Mathf.Lerp(GameMusic.pitch, to, Time.unscaledDeltaTime * SwitchMusicSpeed);
            yield return null;
        }
        GameMusic.pitch = to;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
