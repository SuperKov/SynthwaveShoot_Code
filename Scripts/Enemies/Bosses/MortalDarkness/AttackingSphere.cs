using System.Collections;
using UnityEngine;

public class AttackingSphere : MonoBehaviour
{
    [SerializeField] private int Damage;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float RotateSpeed;
    [SerializeField] private Transform Center;
    [SerializeField] private Vector3 RotateAxis;

    private bool _canAttack = true;

    private void Update()
    {
        transform.RotateAround(Center.position, RotateAxis, RotateSpeed * Time.deltaTime);
    }

    private IEnumerator WaitAttackDelay()
    {
        _canAttack = false;
        yield return new WaitForSeconds(AttackDelay);
        _canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player") && _canAttack)
        {
            other.transform.GetComponent<IDamagable>().ChangeHealth(-Damage);
            StartCoroutine(WaitAttackDelay());
        }
    }
}