using System.Collections.Generic;
using UnityEngine;

public class EffectsDelayPanel : MonoBehaviour
{
    [SerializeField] private Transform ContentPanel;
    [SerializeField] private EffectDelay EffectPrefab;

    private List<EffectDelay> _allEffects = new();

    private void Awake()
    {
        Special.DelayActivated += OnDelayActivated;
    }

    private EffectDelay GetEffectIcon()
    {
        foreach (EffectDelay effect in _allEffects)
            if (!effect.IsBusy)
                return effect;
        EffectDelay newEffect = Instantiate(EffectPrefab, ContentPanel);
        _allEffects.Add(newEffect);
        return newEffect;
    }

    private void OnDelayActivated(Sprite icon, float delay)
    {
        StartCoroutine(GetEffectIcon().StartDelay(icon, delay));
    }

    private void OnDestroy()
    {
        Special.DelayActivated += OnDelayActivated;
    }
}