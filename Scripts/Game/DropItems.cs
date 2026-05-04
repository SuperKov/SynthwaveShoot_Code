using UnityEngine;

public class DropItems : MonoBehaviour
{
    [SerializeField] private int PointsToDrop;
    [SerializeField] private int Points;
    [SerializeField] private float WavePointsModifier;
    [SerializeField] private float LevelPointsModifier;

    [Header("Настройка аптечек")]
    [SerializeField] [Range(0, 100)] private int ForcedHealDrop;
    [SerializeField] private GameObject HealPrefab;

    [Header("Улучшение")]
    [SerializeField] private GameObject UpgradePrefab;
    [SerializeField] private int BoxesToSpecial;
    [SerializeField] private GameObject SpecialPrefab;

    private PlayerController _player;
    private int _boxesDropped;
    private Spawner _spawner;

    private void Awake()
    {

        _player = FindObjectOfType<PlayerController>();
        _spawner = FindObjectOfType<Spawner>();
        Enemy.EnemyDead.AddListener(OnEnemyDead);
    }

    private void DropItem(Vector3 pos)
    {
        float healthPercent = (float)_player.Health / _player.MaxHealth() * 100;
        if (healthPercent <= ForcedHealDrop)
        {
            Instantiate(HealPrefab, pos, Quaternion.identity);
            return;
        }
        if (_boxesDropped < BoxesToSpecial)
        {
            Instantiate(UpgradePrefab, pos, Quaternion.identity);
            _boxesDropped++;
        }
        else
        {
            Instantiate(SpecialPrefab, pos, Quaternion.identity);
            _boxesDropped -= BoxesToSpecial;
        }
    }

    private void OnEnemyDead(Enemy enemy)
    {

        Points += enemy.GivePoints;
        if (Points < PointsToDrop) return;
        int reqPoints;
        if (_spawner.CurrentLevelModifier == 1f)
            reqPoints = Mathf.RoundToInt(PointsToDrop + (_spawner.CurrentWave * WavePointsModifier));
        else
            reqPoints = Mathf.RoundToInt(PointsToDrop + (_spawner.CurrentWave * WavePointsModifier) + (_spawner.CurrentLevelModifier * LevelPointsModifier));
        if (Points >= reqPoints)
        {
            Points -= reqPoints;
            DropItem(enemy.transform.position);
        }
    }

    private void OnDestroy()
    {

        Enemy.EnemyDead.RemoveListener(OnEnemyDead);
    }
}