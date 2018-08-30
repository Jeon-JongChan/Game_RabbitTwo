using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damaged : MonoBehaviour {

    public int Life = 1;
    [SerializeField] float _invincibleTime = 0;
    public UnityEvent TakeHitEvent;
    public UnityEvent DieEvent;

    bool _isInvincible = false;
    WaitForSeconds _wsInvincibleTime;

    private void Start()
    {
        _wsInvincibleTime = new WaitForSeconds(_invincibleTime);
    }

    public void TakeHit(int Damage)
    {
        if(!_isInvincible)
        {
            if(_invincibleTime > 0) 
            {
                _isInvincible = true;
                StartCoroutine("Invincible");
            }
            Life -= Damage;
            print("Hit!!");
            TakeHitEvent.Invoke();
            if (Life < 1) Die();
        }
    }

    IEnumerator Invincible()
    {
        yield return _wsInvincibleTime;
        _isInvincible = false;

    }

    void Die()
    {
        if(!gameObject.CompareTag("Player"))
        {
            if (DieEvent != null) DieEvent.Invoke();
        }
    }

}
