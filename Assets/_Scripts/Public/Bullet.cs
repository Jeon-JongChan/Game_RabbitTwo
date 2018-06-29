using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : ObjectInteraction
{
    /* make bullet, Immediately input value */
    Rigidbody2D rigidbody;

    /* if shoot, Immediately input value */
    Vector2 direction;
    Vector2 initPos;
    float extinctionTime;
    float speed;
    int selectedRayType = 0;
    float rayScale = 1f;
    int layerMask = 0;
    string targetTag = null;

    public void GetRigidbodyComponent()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void InitBaseProperty(Vector2 bulletStartingPoint, float speed, int layerMask = 0, float extinctionTime = 0, float rayScale = 1f)
    {
        initPos = bulletStartingPoint;
        transform.position = initPos;
        this.extinctionTime = extinctionTime;
        this.speed = speed;
        this.layerMask = layerMask;
        this.rayScale = rayScale;
    }

    public void Shoot(Vector2 dir)
    {
        gameObject.SetActive(true);
        StartCoroutine(BulletShot2D(rigidbody, dir, extinctionTime, speed, layerMask, rayScale, selectedRayType));
    }

    /// <summary>
    /// Shoter.cs에서 종료 명령이 오면 발사 위치로 복귀한 후 화면에 보일경우 꺼 줍니다.
    /// </summary>
    public void ExitBullet()
    {
        StopAllCoroutines();
        transform.position = initPos;
        if(gameObject.activeSelf == true )gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ExitBullet();
    }
}
