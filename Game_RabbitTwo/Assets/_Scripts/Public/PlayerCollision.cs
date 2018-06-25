using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

    // Inspector 변수들
    public string groundTag = "Untagged";
    public string obstacleTag = "Untagged";

    //Components
    PlayerJump2D pj;
    Rabbit rabbit;
    //필요 변수들
    bool stay = true;
    //이벤트 변수
    public delegate void JumpDelgate();
    public static event JumpDelgate InitJumpEvent;
    public static event JumpDelgate JumpAnimationEvent;
    public static event JumpDelgate JumpLandingEvent;

    private void Awake()
    {
        pj = GameObject.Find("Rabbit").GetComponent<PlayerJump2D>();
        rabbit = GameObject.Find("Rabbit").GetComponent<Rabbit>();
        InitJumpEvent += pj.JumpStateReset;
        JumpAnimationEvent += rabbit.JumpAnimation;
        JumpLandingEvent += rabbit.LandingAnimation;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == groundTag || col.gameObject.tag == obstacleTag)
        {
            print("jumpCollisionGround - PlayerCollision");
            JumpLandingEvent();
            InitJumpEvent();
            stay = true;
        }
    }

}
