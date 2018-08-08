using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    // Inspector 변수들
    public GameObject player;
    public string[] collisionTag;

    //Components
    PlayerJump2D pj;

    //static 변수들. 플레이어 충돌체가 3개이므로 특정 상황마다 3번씩 실행되는것을 막음.
    static bool stay = false;
    static bool exit = false;

    //필요 변수들


    //이벤트 변수
    public delegate void voidDelgate();
    public delegate void JumpDelgate(bool resetJumpPower);
    public delegate IEnumerator LimitDelgate(float time, float speed);
    public event JumpDelgate InitJumpEvent;
    public event LimitDelgate LimitPlayerEvent;
    public event voidDelgate JumpLandingEvent;
    public event voidDelgate ClearLimitStateEvent; 

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
            var playerScript = GameObject.Find(playerName).GetComponent<Player>();
            InitJumpEvent += pj.JumpStateReset;
            if (playerScript != null)
            {
                JumpLandingEvent += playerScript.LandingAnimation;
                LimitPlayerEvent += playerScript.LimitVelocity;
                ClearLimitStateEvent += playerScript.ClearLimitState;
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
