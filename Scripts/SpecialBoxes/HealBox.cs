public class HealBox : SpecialBox
{
    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    public override bool CanBeUsed()
    {
        if (_player.Health >= _player.MaxHealth())
            return false;
        return true;
    }

    public override void Use()
    {
        _player.ChangeHealth(_player.MaxHealth() - _player.Health);
    }
}
