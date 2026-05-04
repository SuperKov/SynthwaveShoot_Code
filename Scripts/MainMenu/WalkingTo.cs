using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingTo : MonoBehaviour
{
    [SerializeField] private Transform[] Positions;
    [SerializeField] private float Speed;
    [SerializeField] private float WaitBeforeStart;
    [SerializeField] private bool IsLoop;

    private bool _started;
    private int _currentPosition;
    private Vector2 _startPos;

    private IEnumerator Start()
    {
        _startPos = transform.position;
        LookAt();
        yield return new WaitForSeconds(WaitBeforeStart);
        _started = true;
    }

    private void Update()
    {
        if (!_started) return;
        transform.position = Vector2.MoveTowards(transform.position, Positions[_currentPosition].position, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, Positions[_currentPosition].position) <= 0.01f)
        {
            _currentPosition++;
            if (_currentPosition >= Positions.Length)
            {
                transform.position = _startPos;
                _currentPosition = 0;
                if (!IsLoop)
                    enabled = false;
            }
            LookAt();
        }
    }

    private void LookAt()
    {
        Vector2 difference = Positions[_currentPosition].transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);
    }
}