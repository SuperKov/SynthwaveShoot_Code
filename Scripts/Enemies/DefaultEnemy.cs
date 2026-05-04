using System.Collections;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    [Header("ַמלבט")]
    public float AttackDistance;
    public float StayAfterAttack;

    private bool _lock;

    private void Update()
    {
        if (!_lock && Vector2.Distance(transform.position, Player.transform.position) <= AttackDistance)
        {
            Player.ChangeHealth(-Damage);
            StartCoroutine(Stay());
        }
    }

    private void FixedUpdate()
    {
        if (!_lock)
        {

            Vector2 difference = Player.transform.position - transform.position;
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + LookOffset);
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, Speed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Stay()
    {
        _lock = true;
        yield return new WaitForSeconds(StayAfterAttack);
        _lock = false;
    }

    private void OnEnable()
    {

        _lock = false;
    }
}