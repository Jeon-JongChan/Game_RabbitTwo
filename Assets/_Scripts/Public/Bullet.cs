using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : ObjectInteraction
{
    /* make bullet, Immediately input value */
    public Rigidbody2D rigidbody;

    /* if shoot, Immediately input value */
    Vector2 direction;
    float extinctionTime;
    float speed;
    int selectedRayType = 0;
    float rayScale = 1f;
    int layerMask = 1;

    void ShootCircle()
    {

    }

    void ShootTarget()
    {

    }
    void ExitBullet()
    {
        StopAllCoroutines();
    }
}
