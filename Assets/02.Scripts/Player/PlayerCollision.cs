using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

    // Inspector 변수들
    public GameObject player;
    public string[] collisionTag;

    //Components
    PlayerJump2D pj;

    //필요 변수들
    bool stay = true;

    //이벤트 변수
    public delegate void JumpDelgate();
    public static event JumpDelgate InitJumpEvent;
    public static event JumpDelgate JumpLandingEvent;
    static event JumpDelgate BugFix;

    private void Awake()
    {
        string playerName = player.name;

        if(playerName == null)
        {
            Debug.Log("PlayerCollision - 플레이어를 찾지 못했습니다.");
        }
        else
        {
            pj = GameObject.Find(playerName).GetComponent<PlayerJump2D>();
            var playerScript = GameObject.Find(playerName).GetComponent<Rabbit>();
            InitJumpEvent += pj.JumpStateReset;
            if (playerScript != null)
            {
                JumpLandingEvent += playerScript.LandingAnimation;
                BugFix = playerScript.JumpBugFix;
            }
        }
       
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(collisionTag.Length > 0)
        {
            foreach(var v in collisionTag)
            {
                if (col.CompareTag(v))
                {
                    //print("PlayerCollision - 그라운드 충돌");
                    JumpLandingEvent();
                    InitJumpEvent();
                    stay = true;
                    break;
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(stay == false)
        {
            BugFix();
        }
    }

}
