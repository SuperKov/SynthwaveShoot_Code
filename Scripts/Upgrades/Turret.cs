using System.Collections;
using UnityEngine;

public class Turret : Special
{
    [SerializeField] private int Damage;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float AttackRadius;
    [SerializeField] private float RotationOffset;
    [SerializeField] private LayerMask EnemyLayer;

    private Enemy _currentEnemy;
    private bool _canShoot = true;

    private void Start()
    {

        StartCoroutine(CheckEnemy());
    }

    private void Update()
    {
        if (_currentEnemy != null)
        {
            RotateAtEnemy();
            if (CanShoot())
            {
                _currentEnemy.GetComponent<IDamagable>().ChangeHealth(-Damage);
                _canShoot = false;
                Debug.Log("Turret are shooted");
                StartCoroutine(WaitAttackDelay());
            }
        }
        else
            transform.Rotate(new(0, 0, RotationSpeed));
    }

    private bool CanShoot()
    {
        if (Vector2.Distance(transform.position, _currentEnemy.transform.position) > AttackRadius * 1.25f || !_currentEnemy.gameObject.activeSelf)
        {
            _currentEnemy = null;
            return false;
        }
        return _canShoot;
    }

    private void RotateAtEnemy()
    {
        Vector2 difference = transform.position - _currentEnemy.transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float z = Mathf.LerpAngle(transform.eulerAngles.z, rotZ + RotationOffset, RotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + RotationOffset);
    }

    private IEnumerator WaitAttackDelay()
    {
        yield return new WaitForSeconds(AttackDelay);
        _canShoot = true;
    }    

    private IEnumerator CheckEnemy()
    {

        while (true)
        {
            yield return new WaitForSeconds(0.15f);
            if (_currentEnemy != null) continue;
            Collider2D[] targets = new Collider2D[1];
            Physics2D.OverlapCircleNonAlloc(transform.position, AttackRadius, targets, EnemyLayer);
            if (targets[0] != null && targets[0].TryGetComponent(out Enemy enemy))
                _currentEnemy = enemy;
        }
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }
}