using System.Collections;
using UnityEngine;

public class TeleportUpgrade : Special
{
    [SerializeField] private float Delay;
    [SerializeField] private ParticleSystem TelepotEffect;

    private Transform _player;
    private Camera _mainCamera;
    private bool _canTP = true;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Z) || !_canTP) return;
        Teleport();
    }

    private void Teleport()
    {
        TelepotEffect.Play();
        Vector3 pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = _player.position.z;
        _player.position = pos;
        InvokeDelay(Delay);
        StartCoroutine(WaitTeleportDelay());
    }

    private IEnumerator WaitTeleportDelay()
    {

        _canTP = false;
        yield return new WaitForSeconds(Delay);
        _canTP = true;
    }
}
