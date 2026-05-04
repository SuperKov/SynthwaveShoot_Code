using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] private GameObject InfoPanel;
    [SerializeField] private GameObject InteractivePanel;

    [Header("Инфо. панель")]
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private Image Icon;
    [SerializeField] private Vector2 Offset;

    private Slot _currentSlot;

    private RectTransform _rt;
    private Camera _mainCamera;

    private void Awake()
    {
        Slot.Selected.AddListener(OnSelected);
        Slot.Deselected.AddListener(OnDeselected);
        Slot.Interacts.AddListener(OnInteracts);


        _rt = GetComponent<RectTransform>();
        _mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    public void DeleteItem()
    {
        _currentSlot.ChangeItem(null);
        InteractivePanel.SetActive(false);
    }

    private void OnInteracts(Slot slot)
    {
        InteractivePanel.SetActive(true);
        InfoPanel.SetActive(false);
        gameObject.SetActive(true);
        _currentSlot = slot;
    }

    private void OnSelected(Item item)
    {
        Name.text = item.Name;
        Description.text = item.Description;
        Icon.sprite = item.Icon;
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _rt.position = mousePos + Offset;
        InteractivePanel.SetActive(false);
        InfoPanel.SetActive(true);
        gameObject.SetActive(true);
    }

    private void OnDeselected()
    {
        InfoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Slot.Selected.RemoveListener(OnSelected);
        Slot.Deselected.RemoveListener(OnDeselected);
    }
}