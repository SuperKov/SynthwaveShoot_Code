using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] private SpriteRenderer Model;

    public PlayerStatsPreset PlayerStats;

    [Header("Поворот")]
    [SerializeField] private float LookOffset;

    [Header("Жизни")]
    public int Health;
    public bool IsImmortal;
    [SerializeField] private Color TakeDamageColor;
    [SerializeField] private float BlinkTime;
    [SerializeField] private ParticleSystem TakeDamageEffect;

    [Header("Стрельба")]
    [SerializeField] private float MaxShootDistance;
    [SerializeField] private ParticleSystem ShootEffect;

    [Header("Звуки")]
    [SerializeField] private AudioClip ShootSound;
    [SerializeField] private AudioClip TakeDamageSound;
    [SerializeField] private AudioClip DeadSound;

    private List<PlayerStatsPreset> _allStats = new();
    private AudioSource _audioPlayer;
    private Rigidbody2D _rb;
    private Camera _mainCamera;
    private Vector2 _moveInput;
    private Vector2 _moveVelocity;
    private bool _canShoot = true;
    private RaycastHit2D[] _hits = new RaycastHit2D[0];

    public static event UnityAction<int, int> HealthChanged;
    public static event UnityAction PlayerDead;

    private void Start()
    {

        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _audioPlayer = GetComponent<AudioSource>();
        HealthChanged.Invoke(Health, MaxHealth());
    }

    private void Update()
    {
        if (Time.timeScale == 0 || Health < 1) return;
        Shoot();
        Moving();
        LookAtMouse();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveVelocity * Time.fixedDeltaTime);
    }

    public float Speed()
    {
        float speed = PlayerStats.Speed;
        foreach (PlayerStatsPreset stats in _allStats)
            speed += stats.Speed;
        return speed;
    }


    public float AttackSpeed()
    {
        float speed = PlayerStats.AttackSpeed;
        foreach (PlayerStatsPreset stats in _allStats)
            speed += stats.AttackSpeed;
        return speed;
    }

    public int Damage()
    {
        int damage = PlayerStats.Damage;
        foreach (PlayerStatsPreset stats in _allStats)
            damage += stats.Damage;
        return damage;
    }

    public int MaxHealth()
    {
        int health = PlayerStats.MaxHealth;
        foreach (PlayerStatsPreset stats in _allStats)
            health += stats.MaxHealth;
        return health;
    }

    public int MaxShootEnemies()
    {
        int enemies = PlayerStats.MaxShootEnemies;
        foreach (PlayerStatsPreset stats in _allStats)
            enemies += stats.MaxShootEnemies;
        return enemies;
    }

    public void ChangeHealth(int value)
    {
        if (!isActiveAndEnabled || Health < 1 || IsImmortal) return;
        IDamagable.ShowDamageTaked(value, transform);
        if (value < 0)
            StartCoroutine(OnTakeDamage());
        Health = Mathf.Clamp(Health + value, 0, MaxHealth());
        HealthChanged?.Invoke(Health, MaxHealth());
        if (Health < 1)
            Dead();
    }

    public void AddStats(PlayerStatsPreset stats)
    {
        _allStats.Add(stats);
    }

    public void RemoveStats(PlayerStatsPreset stats)
    {
        if (_allStats.Contains(stats))
            _allStats.Remove(stats);
        else
            Debug.Log("Игрок не имеет данных бафов");
    }

    private IEnumerator OnTakeDamage()
    {
        TakeDamageEffect.Play();
        _audioPlayer.PlayOneShot(TakeDamageSound);
        Model.color = TakeDamageColor;
        yield return new WaitForSeconds(BlinkTime);
        Model.color = Color.white;
    }

    private void Shoot()
    {
        if (!Input.GetMouseButton(0) || !_canShoot) return;
        ShootEffect.Play();
        _audioPlayer.PlayOneShot(ShootSound);
        _hits = new RaycastHit2D[MaxShootEnemies()];
        Physics2D.RaycastNonAlloc(transform.position, -transform.up, _hits, MaxShootDistance);
        StartCoroutine(WaitShootDelay());
        if (_hits.Length < 1 || _hits[0].collider == null) return;
        foreach (RaycastHit2D hit in _hits)
            if (hit && hit.collider.TryGetComponent(out IDamagable enemy))
                enemy.ChangeHealth(-Damage());
    }

    private IEnumerator WaitShootDelay()
    {

        _canShoot = false;
        yield return new WaitForSeconds(1 / AttackSpeed());
        _canShoot = true;
    }

    private void Dead()
    {
        _audioPlayer.PlayOneShot(DeadSound);
        Model.gameObject.SetActive(false);
        PlayerDead?.Invoke();
    }

    private void Moving()
    {
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _moveVelocity = _moveInput.normalized * Speed();
    }

    private void LookAtMouse()
    {
        Vector2 difference = _mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + LookOffset);
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawLine(transform.position, MaxShootDistance * -Model.transform.up);
    }
}

[System.Serializable]
public class PlayerStatsPreset
{
    public int MaxHealth;
    
    [Header("Атака")]
    public int Damage;
    public float AttackSpeed;
    [Min(1)] public int MaxShootEnemies;
    
    [Header("Передвижение")]
    public float Speed;
}