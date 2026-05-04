using System.Collections;
using UnityEngine;

public class GlobalMusic : MonoBehaviour
{
    [SerializeField] private AudioSource AudioPlayer;
    [SerializeField] private MusicPack[] DefaultPacks;
    [SerializeField] [Range(0, 1f)] private float DefaultVolume;

    private MusicPack _currentPack;
    private MusicPack.MusicPart _currentPart;
    private Coroutine _changeVolumeCoroutine;

    private void Start()
    {
        ChangeMusicPack(GetRandomPack());
    }

    private void Update()
    {
        if (_currentPack == null || _currentPart == null) return;
        if (AudioPlayer.time >= _currentPart.PartEnd && _currentPart.IsLoop)
            AudioPlayer.time = _currentPart.PartStart;
    }

    private MusicPack GetRandomPack()
    {
        return DefaultPacks[Random.Range(0, DefaultPacks.Length)];
    }

    public float GetDefaultVolume()
    {
        return DefaultVolume;
    }

    public float GetVolume()
    {
        return AudioPlayer.volume;
    }

    public void ChangeVolume(float volume, float time)
    {
        if (_changeVolumeCoroutine != null)
            StopCoroutine(_changeVolumeCoroutine);
        _changeVolumeCoroutine = StartCoroutine(SwitchVolume(volume, time));
    }

    private IEnumerator SwitchVolume(float volume, float time)
    {
        while (AudioPlayer.volume != volume)
        {
            AudioPlayer.volume = Mathf.MoveTowards(AudioPlayer.volume, volume, Time.deltaTime / time);
            yield return null;
        }
    }

    public void ChangeMusicPack(MusicPack pack)
    {
        if (AudioPlayer.volume <= 0)
            ChangeVolume(DefaultVolume, 0.1f);
        _currentPack = pack;
        AudioPlayer.Stop();
        AudioPlayer.clip = pack.Music;
        AudioPlayer.Play();
    }

    public void ChangeMusicPart(MusicPack.MusicPart part)
    {
        _currentPart = part;
        AudioPlayer.time = part.PartStart;
    }
}