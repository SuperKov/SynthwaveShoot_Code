using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Vector2 Offset;
    [SerializeField] private DamageText[] DamageTexts;

    [Header("ÕÏ Áàð")]
    [SerializeField] private int MaxPlayerHealth;
    [SerializeField] private int MinPlayerHealth;
    [SerializeField] private float HealthSizeModifier;
    [SerializeField] private Image Health;
    [SerializeField] private Transform HealthBar;

    private Transform _player;

    private void Awake()
    {
        IDamagable.DamageTaked += ShowDamageText;
        PlayerController.HealthChanged += OnHealthChanged;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        Vector3 convPos = Offset;
        Vector3 pos = _player.position + convPos;
        transform.position = pos;
    }

    public void OnHealthChanged(int health, int maxHealth)
    {
        Vector2 size = HealthBar.localScale;
        size.y = Mathf.Clamp(maxHealth, MinPlayerHealth, MaxPlayerHealth) / HealthSizeModifier;
        HealthBar.localScale = size;
        Health.fillAmount = (float)health / maxHealth;
    }

    public void ShowDamageText(int value, Transform pos)
    {
        foreach (DamageText text in DamageTexts)
            if (!text.Busy)
            {
                text.gameObject.SetActive(true);
                StartCoroutine(text.ShowText(value, pos));
                return;
            }
    }

    private void OnDestroy()
    {
        IDamagable.DamageTaked -= ShowDamageText;
        PlayerController.HealthChanged -= OnHealthChanged;
    }
}