using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour, IDamagable
{
    public string ID;
    public int Health;
    public int Damage;
    public float Speed;
    public float LookOffset;
    public int GivePoints;
    public SpriteRenderer Model;
    [SerializeField] private Color TakeDamageColor;
    [SerializeField] private float BlinkTime;
    [SerializeField] private ParticleSystem DieEffect;

    [Header("Звуки")]
    [SerializeField] private AudioClip DeadSound;

    [HideInInspector] public PlayerController Player;
    [HideInInspector] public AudioSource AudioPlayer;
    [HideInInspector] public int MaxHealth;

    [HideInInspector] public UnityEvent<int, int> TakedDamage = new();
    
    public static UnityEvent<Enemy> EnemyDead = new();

    private Collider2D _collider;

    private void Awake()
    {
        Player = FindObjectOfType<PlayerController>();
        AudioPlayer = GetComponent<AudioSource>();
        _collider = GetComponent<Collider2D>();
        MaxHealth = Health;
    }

    public virtual void ChangeHealth(int value)
    {
        if (!isActiveAndEnabled) return;
        if (value < 0)
            StartCoroutine(OnTakeDamage());
        IDamagable.ShowDamageTaked(value, transform);
        Health = Mathf.Clamp(Health + value, 0, Health);
        if (Health < 1)
            Die();
    }

    public void Respawn()
    {
        enabled = true;
        Model.color = Color.white;
        _collider.enabled = true;
        Model.gameObject.SetActive(true);
        Health = MaxHealth;
        gameObject.SetActive(true);
    }
    public void Respawn(Vector2 pos)
    {
        Respawn();
        transform.position = pos;
    }

    public IEnumerator OnTakeDamage()
    {
        Model.color = TakeDamageColor;
        yield return new WaitForSeconds(BlinkTime);
        Model.color = Color.white;
    }


    public virtual void Die()
    {
        StopAllCoroutines();
        _collider.enabled = false;
        AudioPlayer.PlayOneShot(DeadSound);
        DieEffect.Play();
        Model.gameObject.SetActive(false);
        StartCoroutine(WaitDieEffect());
        enabled = false;
    }

    private IEnumerator WaitDieEffect()
    {

        EnemyDead.Invoke(this);
        yield return new WaitWhile(() => !DieEffect.isStopped);
        gameObject.SetActive(false);
    }
}