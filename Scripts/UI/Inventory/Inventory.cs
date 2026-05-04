using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class SlotKey
    {
        public Slot UsableSlot;
        public KeyCode UseKey;
    }

    [SerializeField] private Slot[] Slots;
    [SerializeField] private SlotKey[] UseSlotsKeys;

    private List<PlayerStatsPreset> _oldStats = new();
    private PlayerController _player;

    private void Awake()
    {
        foreach (Slot slot in Slots)
            slot.ItemChanged += OnItemChanged;
        _player = FindObjectOfType<PlayerController>();
        UpdateOldStats();
    }

    private void Update()
    {
        foreach (SlotKey slot in UseSlotsKeys)
            if (Input.GetKeyDown(slot.UseKey))
                slot.UsableSlot.StorageItem.Use();

    }

    private void OnItemChanged(Item item)
    {
        foreach (PlayerStatsPreset stats in _oldStats)
            _player.RemoveStats(stats);
        foreach (Slot slot in Slots)
            if (slot.StorageItem != null)
                _player.AddStats(slot.StorageItem.Stats);
        UpdateOldStats();
    }

    private void UpdateOldStats()
    {
        _oldStats = new();
        foreach (Slot slot in Slots)
            if (slot.StorageItem != null)
                _oldStats.Add(slot.StorageItem.Stats);
    }
}