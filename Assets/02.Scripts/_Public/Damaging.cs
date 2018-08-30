using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damaging : MonoBehaviour {
    public int damage = 1;
    [SerializeField] string[] damageTargetTag;
    [SerializeField] UnityEvent DamageEvent;
    private void OnCollisionEnter(Collision col)
    {
        if(damageTargetTag.Length > 0)
        {
            foreach(var v in damageTargetTag)
            {
                if (col.gameObject.CompareTag(v))
                {
                    col.gameObject.SendMessage("TakeHit", damage, SendMessageOptions.DontRequireReceiver);
                    print("Damaging : " + col.gameObject.name);
                    DamageEvent.Invoke();
                }
            }
        }
        else col.gameObject.SendMessage("TakeHit", damage, SendMessageOptions.DontRequireReceiver);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (damageTargetTag.Length > 0)
        {
            foreach (var v in damageTargetTag)
            {
                if (col.gameObject.CompareTag(v))
                {
                    col.gameObject.SendMessage("TakeHit", damage, SendMessageOptions.DontRequireReceiver);
                    print("Damaging : " + col.gameObject.name);
                    DamageEvent.Invoke();
                }
            }
        }
        else col.gameObject.SendMessage("TakeHit", damage, SendMessageOptions.DontRequireReceiver);
    }
}
