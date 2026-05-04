using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image Icon;
    [SerializeField] private GameObject UseKey;
    [SerializeField] private float SelectedDelay;

    [SerializeField] private Item _storageItem;
    public Item StorageItem => _storageItem;

    private Coroutine _checkIsSelected;
    private bool _isInteracts;

    [HideInInspector] public event UnityAction<Item> ItemChanged;

    public static UnityEvent<Item> Selected = new();
    public static UnityEvent<Slot> Interacts = new();
    public static UnityEvent Deselected = new();

    private void Awake()
    {
        ChangeItem(_storageItem);
    }

    private void Update()
    {
        if (_isInteracts && Input.GetMouseButtonDown(0))
            _isInteracts = false;
    }

    public void ChangeItem(Item item)
    {
        _storageItem = item;
        Icon.gameObject.SetActive(_storageItem != null);
        ItemChanged?.Invoke(_storageItem);
        UseKey.SetActive(_storageItem != null && _storageItem.IsUsable);
        if (_storageItem == null) return;
        Icon.sprite = _storageItem.Icon;
    }

    public void OnSelected()
    {
        if (_storageItem == null) return;
        _checkIsSelected = StartCoroutine(CheckIsSelected());
    }

    public void IsInteracts()
    {
        _isInteracts = true;
        Interacts.Invoke(this);
        StopCoroutine(CheckIsSelected());
    }

    private IEnumerator CheckIsSelected()
    {
        yield return new WaitForSeconds(SelectedDelay);
        Selected.Invoke(_storageItem);
    }

    public void OnDeselected()
    {
        if (_isInteracts) return;
        if (_checkIsSelected != null)
        StopCoroutine(_checkIsSelected);
        Deselected.Invoke();
    }
}