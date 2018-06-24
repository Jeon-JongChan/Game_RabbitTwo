using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump2D : ObjectControl
{
    [Range(0, 10)]
    public int jumpLevel = 1;
    [Range(0, 10)]
    public float jumpHeight = 1f;
    
    //Components
    Rigidbody2D playerRb;

    //필요한 변수
    int jumpState = 0;
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
                Jump(playerRb, jumpHeight);
                print("점프 : " + jumpState);
                jumpState++;
            }

        }
    }
    /// <summary>
    /// 해당 함수를 플레이어가 호출시 점프횟수가 초기화 된다. 다중점프 구현에 필수
    /// </summary>
    public void JumpStateReset()
    {
        print("점프 초기화 : " + jumpState);
        /* 반발력을 없애준다.*/
        Vector2 initVelocity = new Vector2(playerRb.velocity.x, 0);
        playerRb.velocity = initVelocity;
        jumpState = 0;
    }
}
