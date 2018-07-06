using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

public class LifeInteraction : ObjectMovement2D,IDamageable
{
    [Tooltip("이것은 해당 오브젝트의 체력입니다. \n-1일경우 무적입니다.")]
    public int health = -1;

    //상속 받은 자식만 변수 사용이 가능하다. static 선언이 없으면 상속받은 객체마다 다른 메모리 공간을 가지게 된다.
    protected bool dead = false;

    public void TakeHit(int Damage)
    {
        if (health > 0)
        {
            health -= Damage;
        }
        else if (health == 0)
        {
            Die();
        }
        else if (health < 0)
        {
            Debug.Log("LifeInteraction.cs - it's invincibility");
        }
    }
    void Die()
    {
        Debug.Log("LifeInteraction.cs - 뒤집니다.");
        dead = true;
    }
}
