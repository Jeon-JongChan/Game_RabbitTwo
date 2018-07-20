using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump2D : ObjectMovement2D
{
    [Range(0, 10)]
    public int jumpLevel = 1;
    [Range(0, 50)]
    public float jumpHeight = 1f;
    
    //Components
    Rigidbody2D playerRb;

    //필요한 변수
    int initJumpLevel = 3;
    int jumpState = 0;
    //float gravity = 9.8f;
    bool jump = false;

    // Use this for initialization
    void Start () {
        playerRb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpState < jumpLevel)
            {
                Jump(playerRb, jumpHeight,ForceMode2D.Impulse);
                //print("PlayerJump2D - 점프 : " + jumpState);
                jumpState++;
                jump = true;
            }

        }
    }
    /// <summary>
    /// 해당 함수를 플레이어가 호출시 점프횟수가 초기화 된다. 다중점프 구현에 필수
    /// </summary>
    public void JumpStateReset(bool resetJumpPower)
    {
        //print("PlayerJump2D - 점프 초기화 : " + jumpState);
        /* 반발력을 없애준다.*/
        if(resetJumpPower) 
        {
            Vector2 initVelocity = new Vector2(playerRb.velocity.x, 0);
            playerRb.velocity = initVelocity;
        }
        jumpState = 0;
        jump = false;
    }

    public void JumpLevelPlus(int plusValue)
    {
        jumpLevel += plusValue;
    }
    public void JumpLevelMinus(int plusValue)
    {
        jumpLevel -= plusValue;
    }
    public void SetJumpLevel(int level)
    {
        jumpLevel = level;
    }
    public int GetJumpInitLevel()
    {
        return initJumpLevel;
    }
}
