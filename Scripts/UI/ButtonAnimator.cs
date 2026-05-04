using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ButtonAnimator : MonoBehaviour
{
    public Gradient[] Gradients = new Gradient[3];
    public AudioClip[] Sounds = new AudioClip[3];
    public Vector2[] Positions = new Vector2[3];
    public UseTypes UseType = UseTypes.Текст;
    public Vector2[] Scales = new Vector2[3];
    public float[] Speeds = new float[3];
    public bool UseSounds;
    public bool NotMove;
    public enum UseTypes
    {
        Текст,
        Спрайт
    }

    private AudioSource _audioPlayer;
    private RectTransform _transform;
    private Coroutine _currentState;
    private Vector2 _defaultScale;
    private TextMeshProUGUI _text;
    private Vector2 _defaultPos;
    private Image _image;

    private void Awake()
    {

        _transform = GetComponent<RectTransform>();
        _defaultScale = _transform.localScale;
        _defaultPos = _transform.localPosition;
        if (UseType == UseTypes.Текст)
            _text = GetComponent<TextMeshProUGUI>();
        else
            _image = GetComponent<Image>();
        if (UseSounds)
            _audioPlayer = GetComponent<AudioSource>();
    }

    public void Highlighted()
    {
        if (_currentState != null)
            StopCoroutine(_currentState);
        _currentState = StartCoroutine(ChangeState(1));
    }

    public void Pressed()
    {
        if (_currentState != null)
            StopCoroutine(_currentState);
        _currentState = StartCoroutine(ChangeState(2));
    }

    public void MouseExit()
    {

        if (_currentState != null)
            StopCoroutine(_currentState);
        _currentState = StartCoroutine(ChangeState(0));
    }

    private IEnumerator ChangeState(int index)
    {

        Vector2 scale = _defaultScale + Scales[index];
        Vector2 pos = _defaultPos + Positions[index];
        float speed = Speeds[index];
        float gradientValue = 0;
        if (UseSounds && Sounds[index] != null)
            _audioPlayer.PlayOneShot(Sounds[index]);
        while (true)
        {
            Mathf.Clamp(gradientValue += Time.unscaledDeltaTime * speed, 0f, 1);
            if (Vector2.Distance(_transform.localPosition, pos) > 0 && !NotMove)
                _transform.localPosition = Vector2.Lerp(_transform.localPosition, pos, Time.unscaledDeltaTime * speed);
            if (Vector2.Distance(_transform.localScale, scale) > 0)
                _transform.localScale = Vector2.Lerp(_transform.localScale, scale, Time.unscaledDeltaTime * speed);
            ChangeColor(index, gradientValue);

            yield return null;
            if ((Vector2.Distance(_transform.localScale, scale) <= 0 || NotMove) && Vector2.Distance(_transform.position, pos) <= 0 && _text.color == Gradients[index].Evaluate(1))
                break;
        }
        if (index == 2)
            _currentState = StartCoroutine(ChangeState(1));
    }

    private void ChangeColor(int index, float gradientValue)
    {
        if (UseType == UseTypes.Текст)
            _text.color = Gradients[index].Evaluate(gradientValue);
        else
            _image.color = Gradients[index].Evaluate(gradientValue);
    }
}
