using System.Collections;
using UnityEngine;

public class MortalDarkness : Enemy
{
    [System.Serializable]
    public struct Phase1Preset
    {
        public int BarrierHealth;
        public float ImmortalTime;
        public float SpawnHandDelay;
        public GameObject PhaseObjects;

        [Header("Барьер")]
        public SpriteRenderer DefendBarrier;
        public Color BarrierDefaulColor;
        public Color BarrierActiveColor;

        [Header("Вид")]
        public float OrthographicSize;
        public Color BGColor;
        public Color HealthColor;
        public Vector2 PlayerOffset;

        [Header("Анимация")]
        public float ShowModelTime;
        public float SwitchBGTime;

        [Header("Звуки")]
        public AudioClip TakeDamageSound;
        public AudioClip BarrierBrokenSound;

        [Header("Эффекты")]
        public CameraShake TakeDamageShake;
        public ParticleSystem TakeDamageEffect;
    }

    [System.Serializable]
    public struct Phase2Preset
    {
        public int SpecialsToStun;
        public float StunTime;
        public float SpecialsDelay;
        public float SpawnHandDelay;

        [Header("Таран")]
        public float SlideSpeed;
        public float WaitBeforeSlide;
        public Vector2 MinPos;
        public Vector2 MaxPos;

        [Header("Эффекты")]
        public Sprite NewModel;
        public ParticleSystem Effect;
        public ParticleSystem SlideEffect;
        public float OrthographicSize;
        public float ChangeOrthographicTime;

        [Header("Конец первой фазы")]
        public float OnMusicTime;
        public float Phase1WaitTime;
        public float Phase1SwitchBGSpeed;
        public float Phase1OrthographicSize;
        public Color Phase1BGColor;

        [Header("Анимация")]
        public Color BGColor;

        [Header("Умер")]
        public AudioClip OnDeadSound;
        public ParticleSystem OnDeadEffect;
        public CameraShake OnDeadShake;
    }

    [Header("Босс")]
    [SerializeField] private MusicPack Music;
    [SerializeField] private float AttackDistance;
    [SerializeField] private float AttackDelay;
    [SerializeField] private Enemy FlyingHandPrefab;
    [SerializeField] private Vector2 HandMinPos = new();
    [SerializeField] private Vector2 HandMaxPos;
    [SerializeField] private Phase1Preset Phase1Stats;
    [SerializeField] private Phase2Preset Phase2Stats;

    private Phases _currentPhase = Phases.Начало;
    private PlayerCamera _playerCamera;
    private BossHealth _bossHealth;
    private GlobalMusic _music;
    private Coroutine _handSpawnCoroutine;
    private Spawner _spawner;
    private int _barrierMaxHealth;
    private int _barrierHealth;
    private int _specialsUsed;
    private bool _canSpawnHand = true;
    private bool _canAttack = true;
    private bool _canUseSpecial;
    private bool _IsImmortal;
    private bool _isStunned;

    private enum Phases
    {
        Начало,
        Фаза_1,
        Фаза_2
    }

    private void Start()
    {
        _playerCamera = FindObjectOfType<PlayerCamera>();
        _bossHealth = FindObjectOfType<BossHealth>();
        _music = FindObjectOfType<GlobalMusic>();
        _spawner = FindObjectOfType<Spawner>();
        Spawn();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.U))
        {
            _music.ChangeMusicPart(Music.GetMusicPart("End"));
        }
#endif
        if (_currentPhase == Phases.Начало)
        {
            Vector2 pos = Player.transform.position;
            transform.position = pos + Phase1Stats.PlayerOffset;
        }
        else if (_currentPhase == Phases.Фаза_1 && _barrierHealth < _barrierMaxHealth / 2 && _canSpawnHand)
            SpawnHand(Phase1Stats.SpawnHandDelay);
        else if (_currentPhase == Phases.Фаза_2)
        {
            if (_isStunned) return;
            if (Vector2.Distance(transform.position, Player.transform.position) <= AttackDistance && _canAttack)
            {
                StartCoroutine(WaitAttackDelay());
                Player.ChangeHealth(-Damage);
            }
            if (_canSpawnHand)
                SpawnHand(Phase2Stats.SpawnHandDelay);
            if (_canUseSpecial)
                StartCoroutine(Slide());
            if (_specialsUsed >= Phase2Stats.SpecialsToStun)
                StartCoroutine(Stun());
        }
    }

    private void Spawn()
    {
        Vector2 pos = Player.transform.position;
        transform.position = pos + Phase1Stats.PlayerOffset;
        _IsImmortal = true;
        _barrierHealth = Phase1Stats.BarrierHealth;
        StartCoroutine(Phase1Started());
    }

    private void SpawnHand(float delay)
    {
        Enemy hand = _spawner.GetEnemy(FlyingHandPrefab);
        hand.Respawn(Spawner.RandomPos(HandMaxPos, HandMinPos, Player.transform.position));
        _handSpawnCoroutine = StartCoroutine(WaitSpawnHandDelay(delay));
    }

    private IEnumerator WaitImmortalTime(float time)
    {
        _IsImmortal = true;
        Phase1Stats.DefendBarrier.color = Phase1Stats.BarrierActiveColor;
        yield return new WaitForSeconds(time);
        Phase1Stats.DefendBarrier.color = Phase1Stats.BarrierDefaulColor;
        _IsImmortal = false;
    }

    private IEnumerator Phase1Started()
    {
        Color color = Model.color;
        color.a = 0;
        Model.color = color;
        _bossHealth.ChangeBarColor(Phase1Stats.HealthColor);
        _music.ChangeVolume(0, 4);
        yield return new WaitWhile(() => _music.GetVolume() > 0);
        _music.ChangeVolume(_music.GetDefaultVolume(), 2);
        _music.ChangeMusicPack(Music);
        _music.ChangeMusicPart(Music.GetMusicPart("Start"));
        _playerCamera.ChangeColorBG(Phase1Stats.BGColor, Phase1Stats.SwitchBGTime);
        while (Model.color.a < 1f)
        {
            yield return null;
            Model.color += new Color(0, 0, 0, Time.deltaTime / Phase1Stats.ShowModelTime);
        }
        _bossHealth.SetTrackedBoss(this);
        _IsImmortal = false;
        _playerCamera.ChangeFollow(transform);
        _playerCamera.ChangeOrthographicSize(Phase1Stats.OrthographicSize, 0.5f);
        Phase1Stats.PhaseObjects.SetActive(true);
        _barrierHealth = Phase1Stats.BarrierHealth;
        _barrierMaxHealth = Phase1Stats.BarrierHealth;
        _currentPhase = Phases.Фаза_1;
        _music.ChangeMusicPart(Music.GetMusicPart("Phase1"));
    }

    private IEnumerator Phase2Started()
    {
        if (_handSpawnCoroutine != null)
            StopCoroutine(_handSpawnCoroutine);
        _music.ChangeVolume(0, 2);
        _canSpawnHand = false;
        _IsImmortal = true;
        Model.color = Color.black;
        AudioPlayer.PlayOneShot(Phase1Stats.BarrierBrokenSound);
        _playerCamera.ChangeOrthographicSize(Phase2Stats.Phase1OrthographicSize, Phase2Stats.ChangeOrthographicTime);
        yield return new WaitForSeconds(4);
        _music.ChangeMusicPart(Music.GetMusicPart("Phase2Start"));
        _music.ChangeVolume(_music.GetDefaultVolume(), Phase2Stats.OnMusicTime);
        _playerCamera.ChangeColorBG(Phase2Stats.Phase1BGColor, Phase2Stats.Phase1SwitchBGSpeed);
        Phase2Stats.Effect.Play();
        yield return new WaitForSeconds(Phase2Stats.Phase1WaitTime);

        _music.ChangeMusicPart(Music.GetMusicPart("Phase2"));
        Model.sprite = Phase2Stats.NewModel;
        Model.color = Color.white;
        _playerCamera.SetDefaultFollow();
        _playerCamera.ChangeOrthographicSize(Phase2Stats.OrthographicSize);
        _playerCamera.ChangeColorBG(Phase2Stats.BGColor, Phase2Stats.Phase1SwitchBGSpeed);
        _bossHealth.SetTrackedBoss(this);
        _IsImmortal = false;
        _canSpawnHand = true;
        _canUseSpecial = true;
    }

    private IEnumerator OnDead()
    {
        _IsImmortal = true;
        _music.ChangeVolume(_music.GetDefaultVolume() / 2, 0.5f);
        _playerCamera.ChangeFollow(transform);
        _music.ChangeMusicPart(Music.GetMusicPart("End"));
        Phase2Stats.OnDeadEffect.Play();
        AudioPlayer.PlayOneShot(Phase2Stats.OnDeadSound);
        _bossHealth.BossDead();
        _playerCamera.Shake(Phase2Stats.OnDeadShake);
        yield return new WaitWhile(() => Phase2Stats.OnDeadEffect.isPlaying);
        _music.ChangeVolume(_music.GetDefaultVolume(), 0.2f);
        _playerCamera.SetDefaultFollow();
        Die();
    }

    private IEnumerator Slide()
    {
        Debug.Log("Sliding");
        _canUseSpecial = false;
        _IsImmortal = false;
        Vector2 playerPos = Player.transform.position;
        Vector2 offset = Spawner.RandomPos(Phase2Stats.MaxPos, Phase2Stats.MinPos, Vector2.zero);
        transform.position = playerPos + offset;
        Model.gameObject.SetActive(true);
        yield return new WaitForSeconds(Phase2Stats.WaitBeforeSlide);
        Phase2Stats.SlideEffect.Play();
        Vector2 targetPos = playerPos - offset;
        Debug.Log(targetPos);
        while (Vector2.Distance(transform.position, targetPos) > 0.02f)
        {
            yield return null;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * Phase2Stats.SlideSpeed);
        }
        _specialsUsed++;
        StartCoroutine(WaitSpecialDelay());
        _IsImmortal = true;
        Model.gameObject.SetActive(false);
    }

    private IEnumerator WaitAttackDelay()
    {
        _canAttack = false;
        yield return new WaitForSeconds(AttackDelay);
        _canAttack = true;
    }

    private IEnumerator WaitSpecialDelay()
    {

        yield return new WaitForSeconds(Phase2Stats.SpecialsDelay);
        _canUseSpecial = true;
    }

    private IEnumerator WaitSpawnHandDelay(float delay)
    {
        _canSpawnHand = false;
        yield return new WaitForSeconds(delay);
        _canSpawnHand = true;
    }

    private IEnumerator Stun()
    {
        _isStunned = true;
        _canUseSpecial = false;
        _IsImmortal = false;
        Model.gameObject.SetActive(true);
        transform.position = Spawner.RandomPos(Phase2Stats.MaxPos, Phase2Stats.MinPos, Player.transform.position);
        Debug.Log("IsStunned");
        yield return new WaitForSeconds(Phase2Stats.StunTime);
        Debug.Log("IsUnstunned");
        _IsImmortal = true;
        Model.gameObject.SetActive(false);
        _specialsUsed = 0;
        _isStunned = false;
        _canUseSpecial = true;
    }

    private void TakeBarrierDamage()
    {
        AudioPlayer.PlayOneShot(Phase1Stats.TakeDamageSound);
        _barrierHealth--;
        TakedDamage.Invoke(_barrierHealth, _barrierMaxHealth);
        Phase1Stats.TakeDamageEffect.Play();
        _playerCamera.Shake(Phase1Stats.TakeDamageShake);
        if (_barrierHealth <= 0)
        {
            _currentPhase = Phases.Фаза_2;
            Phase1Stats.PhaseObjects.SetActive(false);
            _bossHealth.BossDead();
            StartCoroutine(Phase2Started());
            return;
        }
        StartCoroutine(WaitImmortalTime(Phase1Stats.ImmortalTime));
    }

    public override void ChangeHealth(int value)
    {
        if (!isActiveAndEnabled || _IsImmortal) return;
        if (_currentPhase == Phases.Фаза_1)
        {
            TakeBarrierDamage();
            return;
        }
        if (value < 0)
            StartCoroutine(OnTakeDamage());
        IDamagable.ShowDamageTaked(value, transform);
        Health = Mathf.Clamp(Health + value, 0, Health);
        TakedDamage.Invoke(Health, MaxHealth);
        if (Health < 1)
        {
            StopAllCoroutines();
            StartCoroutine(OnDead());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(Phase2Stats.MaxPos.x, Phase2Stats.MaxPos.y, 0));
        Gizmos.DrawWireCube(transform.position, new Vector3(Phase2Stats.MinPos.x, Phase2Stats.MinPos.y, 0));
    }
}