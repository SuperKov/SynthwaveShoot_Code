using UnityEngine;

public abstract class Special : MonoBehaviour
{
    [Header("Способность")]
    [SerializeField] private Sprite Icon;

    public static event System.Action<Sprite, float> DelayActivated;

    public void InvokeDelay(float delay)
    {
        DelayActivated.Invoke(Icon, delay);
    }
}
