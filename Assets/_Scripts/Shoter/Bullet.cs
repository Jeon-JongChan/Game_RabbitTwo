using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : ObjectInteraction
{
    /* make bullet, Immediately input value */
    public Rigidbody2D rd2d;

    /* if shoot, Immediately input value */
    public Vector2 direction;
    Vector2 initPos;
    List<string> tags;
    float extinctionTime;
    float speed;

    bool returnTrigger = false;

    public void GetRigidbodyComponent()
    {
        rd2d = GetComponent<Rigidbody2D>();
    }


    public void InitBaseProperty(Vector2 bulletStartingPoint, float speed, List<string> tags , float extinctionTime = 0, bool returnTrigger = true)
    {
        initPos = bulletStartingPoint;
        transform.position = initPos;
        this.extinctionTime = extinctionTime;
        this.speed = speed;
        this.tags = tags;
        this.returnTrigger = returnTrigger;
    }

    public void Shoot(Vector2 dir)
    {
        direction = dir;
        gameObject.SetActive(true);
        StartCoroutine(BulletShot2D(rd2d, direction, extinctionTime, speed));
    }
    public void Shoot(Vector2 bulletStartingPoint, Vector2 dir)
    {
        transform.position = bulletStartingPoint;
        direction = dir;
        gameObject.SetActive(true);
        StartCoroutine(BulletShot2D(rd2d, direction, extinctionTime, speed));
    }
    public void Shoot()
    {
        gameObject.SetActive(true);
        StartCoroutine(BulletShot2D(rd2d,-direction, extinctionTime, speed));
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
    public void SetReturnBullet()
    {
        returnTrigger = true;
    }
    private void OnDisable()
    {
        if (!returnTrigger) ExitBullet();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        foreach (var v in tags)
        {
            if (col.CompareTag(v))
            {
                LifeInteraction interaction = col.gameObject.GetComponent<LifeInteraction>();
                if (interaction != null) interaction.TakeHit(1);
                ExitBullet();
                break;
            }
        }
    }
}
