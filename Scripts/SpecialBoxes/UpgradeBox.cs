using UnityEngine;

public class UpgradeBox : SpecialBox
{
    [SerializeField] private UpgradeType Type = UpgradeType.Статы;

    public enum UpgradeType
    {
        Статы,
        Способность
    }

    public static event System.Action<UpgradeType> UpgradeTaken;

    public override void Use()
    {
        UpgradeTaken.Invoke(Type);
    }
}
