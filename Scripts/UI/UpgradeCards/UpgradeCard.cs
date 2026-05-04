using UnityEngine;
using TMPro;

public abstract class UpgradeCard : MonoBehaviour
{
    [Header("Улучшение")]
    public string Name;
    [Multiline] public string Description;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
   
    [Header("Закрываемые панели")]
    [SerializeField] private GameObject[] ClosingMenu;

    private GameMenu _menu;

    public void Awake()
    {
        NameText.text = Name;
        DescriptionText.text = Description;
        _menu = FindObjectOfType<GameMenu>();
    }

    public virtual void Use() 
    {
        _menu.OffUpgradeMenu();
        foreach (GameObject menu in ClosingMenu)
            menu.SetActive(false); 
    }
}