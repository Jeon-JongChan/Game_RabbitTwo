using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerJump2D))]
public class Player : LifeInteraction
{
    //public variable - inspector
    public float speed = 2;
    public float BarrierdelayTime = 5f;
    public GameObject barrier;
    [SerializeField] GameObject particle;
    [SerializeField] float particleLifeTime = 0.3f;
    
    //Components
    Rigidbody2D playerRb;
    /*  오른쪽 스탠딩 0   왼쪽 스탠딩 1
     *  오른쪽 워킹 2    왼쪽 워킹 3
     *  오른쪽 점프 4    왼쪽 점프5
     */
    Animator playerMoveAnimator;
    BoxCollider2D box2d;
    SpriteRenderer playerSr;
    //need virable
    float x = 0, y = 0;
    float initSpeed;
    int aniState = 0;
    float limitZangle = 0.2f;
    bool jump = false;
    bool limitState = false;  
    bool barrierState = false;
    public bool BarrierState{
        get{return barrierState;}
        set{barrierState = value;}
    }
    public bool Dead{
        get{return dead;}
    }
    bool barrierAniTrigger = true;

	// Use this for initialization
	void Start () {
        playerRb = GetComponent<Rigidbody2D>();
        playerMoveAnimator = GetComponent<Animator>();
        playerSr = GetComponent<SpriteRenderer>();
        box2d = GetComponent<BoxCollider2D>();
        initSpeed = speed;
	}
	
	// Update is called once per frame
	void Update () {
        if (!dead)
        {
            x = Input.GetAxisRaw("Horizontal");

            AnimationControl();
            Move(playerRb, new Vector2(x, y), speed);

            if (Mathf.Abs(playerRb.velocity.y) > 1f) JumpAnimation();
            if (barrierState && barrierAniTrigger)
            {
                //print("켜진다. 배리어");
                barrier.SetActive(barrierAniTrigger);
                barrierAniTrigger = false;
                StartCoroutine(BarrierExit(BarrierdelayTime));
            }
        }
    }
    public void PlayerDie()
    {
        DisableObject2D(playerSr,box2d);
        if(particle != null) particle.SetActive(true);
    }
    IEnumerator BarrierExit(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        barrier.SetActive(barrierAniTrigger);
        barrierState = false;
        barrierAniTrigger = true;
    }
    // 사용된 클래스 변수 : x, anistate, jump
    void AnimationControl()
    {
        if (x == 0 && playerRb.velocity.x == 0 && !jump)
        {
            aniState = aniState % 2;
        }
        else if (x != 0 && !jump)
        {
            if (x > 0) aniState = 2;
            else if (x < 0) aniState = 3;
        }
        /* 점프 도중 방향 전환할 때*/
        if (jump && x > 0) aniState = 4;
        else if (jump && x < 0) aniState = 5;

        if (playerMoveAnimator != null) playerMoveAnimator.SetInteger("move", aniState);
    }
    /// <summary>
    /// 플레이어 하단에 위치한 충돌 오브젝트에서 실행되는 함수(PlayerCollision). 충돌감지를 벗어날 경우 실행된다.
    /// </summary>
    public void JumpAnimation()
    {
        if (aniState % 2 == 0)
        {
            aniState = 4;
        }
        else if (aniState % 2 == 1)
        {
            aniState = 5;
        }
        jump = true;
    }
    public void LandingAnimation()
    {
        if (aniState % 2 == 0)
        {
            aniState = 0;
        }
        else if (aniState % 2 == 1)
        {
            aniState = 1;
        }
        jump = false;
    }
    /// <summary>
    /// 속도를 제한하는 LimitVelocity 코루틴을 종료시킵니다.
    /// </summary>
    public void ClearLimitState()
    {
        limitState = false;
    }

    /// <summary>
    /// 일정 시간동안 플레이어의 속도를 제한합니다. limitTime 이 0이면 특정 bool 값이 변하기 전까지 지속합니다.
    /// </summary>
    public IEnumerator LimitVelocity(float limitTime, float limit)
    {
        float start = Time.time;
        float end = start;

        Vector2 tempVector;
        limitState = true;
        speed = limit;

        while(limitState)
        {
            yield return new WaitForFixedUpdate();
            end += Time.fixedDeltaTime;

            if (Mathf.Abs(playerRb.velocity.x) > limit || Mathf.Abs(playerRb.velocity.y) > limit)
            {
                tempVector.x = Mathf.Clamp(playerRb.velocity.x, -limit, limit);
                tempVector.y = Mathf.Clamp(playerRb.velocity.y, -20 , limit);
                playerRb.velocity = tempVector;
            }

            if(limitTime == 0) continue;
            else if(end - start > limitTime) break;

        }

        speed = initSpeed;
        yield return new WaitForFixedUpdate();
        //Debug.Log("Player.cs - " + limitTime + " 디버프 종료" + start + " " + end);
    }
}
