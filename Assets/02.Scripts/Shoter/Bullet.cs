using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : ObjectInteraction
{
    /* make bullet, Immediately input value */

    /* if shoot, Immediately input value */
    public Vector2 direction;
    List<string> tags;
    float extinctionTime;
    float speed;

    /* needs components */
    Rigidbody2D rd2d;
    SpriteRenderer sr;
    CircleCollider2D circle;

    /* needs variable */
    bool returnTrigger = false;

    public void GetBulletComponent()
    {
        rd2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circle = GetComponent<CircleCollider2D>();
    }


    public void InitBaseProperty(Vector2 bulletStartingPoint, float speed, List<string> tags , float extinctionTime = 0, bool returnTrigger = true)
    {
        SaveState(true, false, bulletStartingPoint);
        transform.position = initPos;
        this.extinctionTime = extinctionTime;
        this.speed = speed;
        this.tags = tags;
        this.returnTrigger = returnTrigger;
    }

    /// <summary>
    /// 회전 발사에 사용됩니다.
    /// </summary>
    public void Shoot(Vector2 bulletStartingPoint, Vector2 dir)
    {
        if (sr != null) sr.enabled = true;
        if (circle != null) circle.enabled = true;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        transform.position = bulletStartingPoint;
        direction = dir;
        StartCoroutine(BulletShot2D(rd2d, direction, extinctionTime, speed));
    }
    /// <summary>
    /// 리터닝 발사에 사용됩니다. 위치를 복원 시키지 않습니다.
    /// </summary>
    public void Shoot()
    {
        if (sr != null) sr.enabled = true;
        if (circle != null) circle.enabled = true;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        StartCoroutine(BulletShot2D(rd2d,-direction, extinctionTime, speed));
    }

    /// <summary>
    /// Shoter.cs에서 종료 명령이 오면 발사 위치로 복귀한 후 화면에 보일경우 꺼 줍니다.
    /// </summary>
    public void ExitBullet()
    {
        if (sr != null) sr.enabled = false;
        if (circle != null) circle.enabled = false;
    }
    public override bool LoadState()
    {
        if (sr != null) sr.enabled = true;
        if (circle != null) circle.enabled = true;
        return base.LoadState();
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
                IDamageable interaction = col.gameObject.GetComponent<IDamageable>();
                if (interaction != null) interaction.TakeHit(1);
                ExitBullet();
                break;
            }
        }
    }
}
