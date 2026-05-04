using UnityEngine;

public class UpgradeStatsCard : UpgradeCard
{
    [SerializeField] private _type Type;
    [SerializeField] private int GivePercent;
    [SerializeField] private int ForcedGive;

    private enum  _type
    {
        Урон,
        Здоровье,
        Скорость_атаки
    }
    private PlayerController _player;
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    private int UpgradedValue(int value, float modifier)
    {
        if ((int)(value * modifier) == value)
            return Mathf.RoundToInt(value + ForcedGive);
        return Mathf.RoundToInt(value * modifier);

    }
    
    public override void Use()
    {
        base.Use();
        float modifier = 1 + ((float)GivePercent / 100);
        switch (Type)
        {
            case _type.Урон:
                _player.PlayerStats.Damage = UpgradedValue(_player.PlayerStats.Damage, modifier);
                break;
            case _type.Здоровье:
                _player.PlayerStats.MaxHealth = UpgradedValue(_player.PlayerStats.MaxHealth, modifier);
                break;
            case _type.Скорость_атаки:
                _player.PlayerStats.AttackSpeed *= modifier;
                break;
        }
    }

}