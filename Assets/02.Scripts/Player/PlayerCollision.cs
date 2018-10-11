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


    //이벤트 변수
    public delegate void voidDelgate();
    public delegate void JumpDelgate(bool resetJumpPower);

    event JumpDelgate InitJumpEvent;
    event voidDelgate JumpLandingEvent;
    event voidDelgate ClearLimitStateEvent; 

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
            var playerInstance = GameObject.Find(playerName).GetComponent<Player>();
            InitJumpEvent += pj.JumpStateReset;
            if (playerInstance != null)
            {
                JumpLandingEvent += playerInstance.LandingAnimation;
                ClearLimitStateEvent += playerInstance.ClearLimitState;
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
                    InitJumpEvent(true);
                    break;
                }
            }
        }
    }

}
