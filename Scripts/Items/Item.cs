using UnityEngine;

 [CreateAssetMenu(fileName = "New_Item", menuName = "Create Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string _Name;
    public string Name => _Name;

    [SerializeField] [Multiline] private string _Description;
    public string Description => _Description;

    [SerializeField] private Sprite _Icon;
    public Sprite Icon => _Icon;

    [SerializeField] private bool _IsUsable;
    public bool IsUsable => _IsUsable;

    [SerializeField] private PlayerStatsPreset _Stats;
    public PlayerStatsPreset Stats => _Stats;

    public virtual void Use() { }
}