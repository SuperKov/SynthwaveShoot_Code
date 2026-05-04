using System.Collections;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    private Camera _camera;
    private CinemachineVirtualCamera _virCamera;
   
    private Transform _player;
    private Color _defaultColorBG;
    private float _defaultOrthoSize;
    private CinemachineBasicMultiChannelPerlin _cameraNoise;
    private Coroutine _switchOrthographicSize; 
    private Coroutine _waitShakeCoroutine;
    private Coroutine _switchColorBG;

    private void Awake()
    {
        _camera = Camera.main;
        _virCamera = GetComponent<CinemachineVirtualCamera>();
        _player = FindObjectOfType<PlayerController>().transform;
        _defaultColorBG = _camera.backgroundColor;
        _defaultOrthoSize = _virCamera.m_Lens.OrthographicSize;
        _cameraNoise = _virCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    public void Shake(CameraShake shake)
    {
        _cameraNoise.m_AmplitudeGain = shake.Force;
        if (_waitShakeCoroutine != null)
            StopCoroutine(_waitShakeCoroutine);
        _waitShakeCoroutine = StartCoroutine(WaitShake(shake.Time));
    }

    public void SetDefault()
    {
        _virCamera.Follow = _player;
        StartCoroutine(SwitchColorBG(_defaultColorBG, 4));
        _virCamera.m_Lens.OrthographicSize = _defaultOrthoSize;
    }

    private IEnumerator WaitShake(float time)
    {
        yield return new WaitForSeconds(time);
        _cameraNoise.m_AmplitudeGain = 0;
    }

    public void ChangeColorBG(Color color, float time = 1f)
    {
        if (_switchColorBG != null)
            StopCoroutine(_switchColorBG);
        _switchColorBG = StartCoroutine(SwitchColorBG(color, time));
    }

    private IEnumerator SwitchColorBG(Color color, float time)
    {
        while (_camera.backgroundColor != color)
        {
            _camera.backgroundColor = Color.Lerp(_camera.backgroundColor, color, Time.deltaTime / time);
            yield return null;
        }
    }

    public void ChangeFollow(Transform target)
    {
        _virCamera.Follow = target;
    }

    public void SetDefaultFollow()
    {
        _virCamera.Follow = _player;
    }

    public void ChangeOrthographicSize(float size, float time = 1f)
    {
        if (_switchOrthographicSize != null)
            StopCoroutine(_switchOrthographicSize);
        StartCoroutine(SwitchOrthographicSize(size, time));
    }

    private IEnumerator SwitchOrthographicSize(float size, float time)
    {
        while (_virCamera.m_Lens.OrthographicSize != size)
        {
            _virCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_virCamera.m_Lens.OrthographicSize, size, Time.deltaTime / time);
            yield return null;
        }
    }

    public void SetDefaultOrthographicSize()
    {
        _virCamera.m_Lens.OrthographicSize = _defaultOrthoSize;
    }
}

[System.Serializable]
public class CameraShake
{
    public float Force;
    public float Time;
}