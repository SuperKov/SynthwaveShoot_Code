using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnemy : Enemy
{
    private float _lookOffset;

    private void Start()
    {
        _lookOffset = LookOffset;
    }

    private void FixedUpdate()
    {
        Vector2 difference = Player.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        if (Player.transform.position.x > transform.position.x)
        {
            _lookOffset = LookOffset;
            transform.localScale = new(1, transform.localScale.y, 1);
        }
        else
        {
            _lookOffset = LookOffset * 3;
            transform.localScale = new(-1, transform.localScale.y, 1);
        }
        transform.rotation = Quaternion.Euler(0, 0, rotZ + _lookOffset);
        transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, Speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActiveAndEnabled)
        {
            Player.ChangeHealth(-Damage);
            Debug.Log("attacked");
            Die();
        }
    }
}