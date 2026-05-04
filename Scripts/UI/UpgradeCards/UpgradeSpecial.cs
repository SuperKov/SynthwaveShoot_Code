using UnityEngine;
using UnityEngine.Events;

public class UpgradeSpecial : UpgradeCard
{
    [Header("Способность")]
    [SerializeField] private UnityEvent Special = new();

    public override void Use()
    {
        base.Use();
        Special.Invoke();
    }
}
