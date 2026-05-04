using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable] public class EnemyType
    {
        public Enemy EnemyPrefab;
        public int Amount;
    }

    [System.Serializable] public class Wave
    {
        public EnemyType[] EnemyWave;
        public bool StartNext = true;
    }

    [SerializeField] private List<Wave> Waves;
    [SerializeField] private float NextLevelModifier;
    public int CurrentWave;
    public float CurrentLevelModifier = 1;

    [Header("—лучайный спавн")]
    [SerializeField] private Vector2 MinDistance;
    [SerializeField] private Vector2 MaxDistance;

    [Header("∆дать перед спавном")]
    [SerializeField] private float MaxSpawnDelay;
    [SerializeField] private float MinSpawnDelay;
    [SerializeField] private float AverageSpawnDelay;
    [SerializeField] private int MaxEnemiesAmount;

    private int _currentEnemies;
    private int _spawnedEnemies;
    private List<Enemy> _allCachedEnemies = new();
    private Transform _player;
    private float _timeAfterLastEnemyDead;
    private float _spawnDelay;
    private bool _waveStarted;
    private Coroutine _currentSpawn;

    private void Awake()
    {

        _spawnDelay = AverageSpawnDelay;
        Enemy.EnemyDead.AddListener(OnEnemyDead);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnWave();
    }

    private void Update()
    {

        _timeAfterLastEnemyDead += Time.deltaTime;
    }

    public Enemy GetEnemy(Enemy type)
    {
        foreach (Enemy enemy in _allCachedEnemies)
            if (!enemy.gameObject.activeSelf && enemy.ID == type.ID)
                return enemy;
        Enemy newEnemy = Instantiate(type, RandomPos(MaxDistance, MinDistance, _player.position), Quaternion.identity);
        _allCachedEnemies.Add(newEnemy);
        return newEnemy;
    }

    public static Vector2 RandomPos(Vector2 maxDistacne, Vector2 minDistance, Vector2 point)
    {
        float distX = maxDistacne.x / 2;
        float distY = maxDistacne.y / 2;
        Vector2 pos = new()
        {
            x = Random.Range(point.x - distX, point.x + distX),
            y = Random.Range(point.y - distY, point.y + distY)
        };
        if (Mathf.Abs(pos.x) < minDistance.x / 2 && Mathf.Abs(pos.y) < minDistance.y /2)
            return RandomPos(maxDistacne, minDistance, point);
        return pos;
    }

    private void SpawnWave()
    {
        if (Waves.Count < 1)
            return;
        _currentSpawn = StartCoroutine(SpawnEnemies());
    }

    private void GenerateEnemiesList()
    {
        _currentEnemies = 0;
        foreach (EnemyType enemyType in Waves[CurrentWave].EnemyWave)
        {
            int amount = Mathf.RoundToInt(enemyType.Amount * CurrentLevelModifier);
            _currentEnemies += amount;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        if (_currentSpawn != null)
            StopCoroutine(_currentSpawn);
        GenerateEnemiesList();
        yield return new WaitForSeconds(2);
        _timeAfterLastEnemyDead -= 2;
        Debug.Log("Wave start Spawning");
        _waveStarted = true;
        foreach (EnemyType enemyType in Waves[CurrentWave].EnemyWave)
        {
            int amount = Mathf.RoundToInt(enemyType.Amount * CurrentLevelModifier);
            for (int i = 0; i < amount; i++)
            {
                yield return new WaitWhile(() => _spawnedEnemies >= MaxEnemiesAmount);
                _spawnedEnemies++;
                GetEnemy(enemyType.EnemyPrefab).Respawn(RandomPos(MaxDistance, MinDistance, _player.position));
                yield return new WaitForSeconds(_spawnDelay);
            }
        }
        Debug.Log("Wave Spawned. End");
    }

    private void OnEnemyDead(Enemy enemy)
    {
        if (!_waveStarted) return;
        _currentEnemies--;
        _spawnedEnemies--;
        CalculateSpawnDelay();
        if (_currentEnemies > 0) return;
        Debug.Log("WaveCleared");
        _waveStarted = false;
        Wave currentWave = Waves[CurrentWave];
        if (CurrentWave >= Waves.Count - 1)
        {
            CurrentLevelModifier += NextLevelModifier;
            CurrentWave = 0;
        }
        else
            CurrentWave++;
        if (!currentWave.StartNext) return;
        SpawnWave();
    }

    private void CalculateSpawnDelay()
    {

        switch (_timeAfterLastEnemyDead)
        {
            case > 14.5f:
                _spawnDelay *= 1.5f;
                break;
            case > 8.6f:
                _spawnDelay *= 1.2f;
                break;
            case < 0.34f:
                _spawnDelay /= 1.14f;
                break;
            case < 0.88f:
                _spawnDelay /= 1.08f;
                break;
            case < 1.9f:
                _spawnDelay = Mathf.Clamp(_spawnDelay /= 1.12f, AverageSpawnDelay, MaxSpawnDelay);
                break;
        }
        _timeAfterLastEnemyDead = 0;
        _spawnDelay = Mathf.Clamp(_spawnDelay, MinSpawnDelay, MaxSpawnDelay);
    }

    private void OnDestroy()
    {

        Enemy.EnemyDead.RemoveListener(OnEnemyDead);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(MaxDistance.x, MaxDistance.y, 0));
        Gizmos.DrawWireCube(transform.position, new Vector3(MinDistance.x, MinDistance.y, 0));
    }
}