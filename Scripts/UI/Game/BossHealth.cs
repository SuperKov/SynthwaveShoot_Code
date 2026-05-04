using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [Header("ÕÏ áàð")]
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private Image HealthBarImage;
    [SerializeField] private TextMeshProUGUI BossNameText;
    [SerializeField] private Color DefaultBarColor;

    private Enemy _currentBoss;

    private void OnBossTakedDamage(int health, int maxHealth)
    {
        HealthBarImage.fillAmount = (float)health / maxHealth;
    }

    public void SetTrackedBoss(Enemy boss)
    {
        _currentBoss = boss;
        HealthBarImage.fillAmount = 1;
        BossNameText.text = boss.ID;
        HealthBar.SetActive(true);
        _currentBoss.TakedDamage.AddListener(OnBossTakedDamage);
    }

    public void ChangeBarColor(Color color)
    {
        HealthBarImage.color = color;
    }

    public void BossDead()
    {
        _currentBoss.TakedDamage.RemoveListener(OnBossTakedDamage);
        _currentBoss = null;
        HealthBar.SetActive(false);
        HealthBarImage.color = DefaultBarColor;
    }

    private void OnDestroy()
    {
        if (_currentBoss != null)
        _currentBoss.TakedDamage.RemoveListener(OnBossTakedDamage);
    }
}