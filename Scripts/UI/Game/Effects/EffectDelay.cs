using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EffectDelay : MonoBehaviour
{
    [SerializeField] private Image EffectImage;
    [SerializeField] private Image EffectDelayImage;
    [SerializeField] private float HideSpeed;
    public bool IsBusy;

    public IEnumerator StartDelay(Sprite icon, float time)
    {
        IsBusy = true;
        EffectDelayImage.fillAmount = 0;
        EffectImage.sprite = icon;
        EffectDelayImage.sprite = icon;
        UpdateColor();
        gameObject.SetActive(true);
        while (EffectDelayImage.fillAmount < 1f)
        {
            yield return null;
            EffectDelayImage.fillAmount += Time.deltaTime / time;
        }
        StartCoroutine(OffIcons());
    }

    private void UpdateColor()
    {
        Color color = EffectDelayImage.color;
        color.a = 1f;
        EffectDelayImage.color = color;
    }

    private IEnumerator OffIcons()
    {
        while (EffectDelayImage.color.a > 0)
        {

            yield return null;
            EffectDelayImage.color -= new Color(0, 0, 0, HideSpeed * Time.deltaTime);
        }
        IsBusy = false;
        gameObject.SetActive(false);
    }
}