using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObject : ObjectInteraction {


    public Vector2 direction = Vector2.zero;
    [Tooltip("다른 총알과 충돌할지 안할지 결정합니다.")]
    public bool otherBallCollisionTrigger = true;
    [Tooltip("플레이어와 충돌시 사라지게 합니다.")]
    public bool disapperCollisionPlayer = true;
    List<string> tags;
    float extinctionTime;
    float speed;

    /* needs components */
    Rigidbody2D rd2d;
    SpriteRenderer sr;
    CircleCollider2D circle;

    /* needs variable */


    public void GetBulletComponent()
    {
        rd2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circle = GetComponent<CircleCollider2D>();
    }

	public void InitBaseProperty(Vector2 bulletStartingPoint, float speed, List<string> tags , float extinctionTime = 0)
    {
        SaveState(false, false, bulletStartingPoint);
        transform.position = initPos;
        this.extinctionTime = extinctionTime;
        this.speed = speed;
        this.tags = tags;
    }
	// Use this for initialization
	private void Awake() {
		
	} 

    public void Shoot(Vector2 bulletStartingPoint, Vector2 dir)
    {
        state = true;
        if (sr != null) sr.enabled = true;
        if (circle != null) circle.enabled = true;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        transform.position = bulletStartingPoint;
        direction = dir;
        StartCoroutine(BounceShot2D(rd2d, extinctionTime, speed));
    }

    /// <summary>
    /// 발사체를 발사시키는 함수. 외부에서 충돌에 의해 direction이 변경된다.
    /// </summary>
    /// <param name="selectedRayType"> If it is 0, it is OverlapCircle. 1 is RayCast</param>
    public IEnumerator BounceShot2D(Rigidbody2D rigidbody2D, float extinctionTime, float speed = 1f, bool sleepState = false)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;
        sleepState = false;
        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if (extinctionTime > 0)
        {
            /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
            while (currentTime - startedTime < extinctionTime && state)
            {
                //print("ObjectInteraction - 남은시간 " + (currentTime - startedTime));
                    //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red);
                MovePos(rigidbody2D, direction, speed, sleepState);
                yield return new WaitForFixedUpdate();
                currentTime = Time.time;
            }
        }
        /* 0일 경우 오브젝트와 충돌 할 때까지 이동한다. */
        else if (extinctionTime == 0)
        {
            //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red, 5);
            while(state)
            {
                MovePos(rigidbody2D, direction, speed);

                yield return new WaitForFixedUpdate();
            }
        }
        // 오브젝트와 충돌하고 나면 비활성화 한다.
        //Debug.Log("ObjectInteraction - 총알의 움직임이 종료되었습니다.");
        rigidbody2D.gameObject.SetActive(false);
    }
    public void ExitBullet()
    {
        if (sr != null) sr.enabled = false;
        if (circle != null) circle.enabled = false;
    }
    private void OnDisable()
    {
        ExitBullet();
    }
    public override bool LoadState(bool init = false)
    {
        if (sr != null) sr.enabled = true;
        if (circle != null) circle.enabled = true;
        return base.LoadState();
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(!otherBallCollisionTrigger)
        {
            if(!col.gameObject.CompareTag("BOUNCEBALL")) direction = GetReflectAngleVector2D(col, direction);
        }
        else direction = GetReflectAngleVector2D(col, direction);

        if(tags.Count > 0)
        {
            foreach(var v in tags)
            {
                if(col.gameObject.CompareTag(v))
                {
                    LifeInteraction interaction = col.gameObject.GetComponent<LifeInteraction>();
                    if(interaction != null)interaction.TakeHit(1);
                    
                    if(disapperCollisionPlayer)
                    {
                        state = false;
                    }
                    else{
                        if(!col.gameObject.CompareTag("Player")) state = false;
                    }
                    break;
                }
            }
        }
    }

}

