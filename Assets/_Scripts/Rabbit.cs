using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : ObjectInteraction
{
    //public variable - inspector
    public int speed = 2;

    //Components
    Rigidbody2D playerRb;
    /*  오른쪽 스탠딩 0   왼쪽 스탠딩 1
     *  오른쪽 워킹 2    왼쪽 워킹 3
     *  오른쪽 점프 4    왼쪽 점프5
     */
    Animator playerAnimator;

    //need virable
    float x = 0, y = 0;
    int aniState = 0;
    bool jump = false;

	// Use this for initialization
	void Start () {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        x = Input.GetAxisRaw("Horizontal");

        if( x == 0 && playerRb.velocity.x == 0 && !jump)
        {
            aniState = aniState % 2;
        }
        else if(x != 0 && !jump)
        {
            if (x > 0) aniState = 2;
            else if(x < 0) aniState = 3;
        }
        /* 점프 도중 방향 전환할 때*/
        if (jump && x > 0) aniState = 4;
        else if (jump && x < 0) aniState = 5;

        playerAnimator.SetInteger("move", aniState);
        Move(playerRb, new Vector2(x, y), speed);
        
        if(Mathf.Abs(playerRb.velocity.y) > 1f) JumpAnimation();

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
    public void JumpBugFix()
    {
        Debug.Log("PlayerJump2D - 점프버그수정");
        Vector2 initVelocity = new Vector2(playerRb.velocity.x, 5);
        playerRb.velocity = initVelocity;
        jump = false;
        aniState = 0;
    }
}
