using System.Collections;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public bool Busy;
    [SerializeField] private float HideSpeed;
    [SerializeField] private Vector2 Offset;

    private TextMeshProUGUI _text;

    private void Awake()
    {

        _text = GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator ShowText(int value, Transform pos)
    {
        Busy = true;
        _text.text = value.ToString();
        Color color = _text.color;
        color.a = 1;
        _text.color = color;
        while (_text.color.a > 0)
        {
            _text.rectTransform.position = pos.position + new Vector3(Offset.x, Offset.y, 0);
            _text.color -= new Color(0, 0, 0, HideSpeed * Time.deltaTime);
            yield return null;
        }
        _text.text = "";
        Busy = false;
        gameObject.SetActive(false);
    }
}
