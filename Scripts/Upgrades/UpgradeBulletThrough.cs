using UnityEngine;

public class UpgradeBulletThrough : UpgradeCard
{
    [SerializeField] private int GiveUnits;

    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    public override void Use()
    {
        base.Use();
        _player.PlayerStats.MaxShootEnemies += GiveUnits;
    }
}
