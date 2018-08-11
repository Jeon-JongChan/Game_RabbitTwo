using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    JUMP,
    LIMITVELOCITY,
    BARRIER,
    WATER
}
public class PlayerInteraction : ObjectInteraction {

    /* inspector variable */
    [Header("TYPE")]
    public GameObject player = null;
    [Tooltip(" 점프횟수를 조작합니다 \n  점프나 움직임에 속력을 조작합니다.\n 베리어를 동작 시키는 오브젝트 입니다. \nwater 전용값")]
    public EffectType selectedType = EffectType.JUMP;
    [Tooltip("버프 or 디버프가 작동하는 시간입니다.")]
    public float delayTime = 1f;
    [Tooltip("작동후 끕니다.")]
    public bool disableTrigger = false;

    [Header("Selected JUMP")]
    public int addJumpLevel = 0;
    [Header("Selected LIMIT OR WARTER")]
    public int limitPlayer = 3;
    [Header("Selected WARTER")]
    public float delayDamageTime = 3f;
    public int damage = 1;

    /* needs components */
    Player playerInstance;
    PlayerJump2D playerJumpInstance;
    Collider2D colComponent;
    SpriteRenderer srComponent;

    /* needs variable */
    bool bugState = false; //플레이어 오브젝트가 없으면 true
    bool isWater = false;
    Coroutine returnCoroutine =null;
    string playerTag = null;

    private void Start()
    {
        player = player == null ? GameObject.FindGameObjectWithTag("Player"):player;
        if (player != null)
        {
            playerTag = player.tag;
            playerInstance = player.GetComponent<Player>();
            playerJumpInstance = player.GetComponent<PlayerJump2D>();
        }
        else { bugState = true; }

        colComponent = GetComponent<BoxCollider2D>();
        srComponent = GetComponent<SpriteRenderer>();
 
    }
    public IEnumerator ControlJump()
    {
        if(returnCoroutine != null)StopCoroutine(returnCoroutine);
        if(disableTrigger) DisableObject2D(srComponent,colComponent);
        print(playerJumpInstance.jumpLevel.ToString() + " ");
        playerJumpInstance.JumpLevelPlus(addJumpLevel);
        print(playerJumpInstance.jumpLevel.ToString() + " ");
        yield return new WaitForSeconds(delayTime);
        playerJumpInstance.SetJumpLevel(playerJumpInstance.InitJumpLevel);
        //print(" 2번째 기회 "+playerJumpInstance.jumpLevel);
    }
    public IEnumerator DelayDamageToPlayer(float delayDamageTime)
    {
        while(isWater)
        {
            yield return new WaitForSeconds(delayDamageTime);
            if(!playerInstance.BarrierState) playerInstance.TakeHit(damage);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!bugState)
        {
            if (playerTag != null && col.CompareTag(playerTag))
            {
                switch(selectedType)
                {
                    case EffectType.JUMP:
                        returnCoroutine = StartCoroutine(ControlJump());
                        break;
                    case EffectType.LIMITVELOCITY:
                        returnCoroutine = StartCoroutine(playerInstance.LimitVelocity(delayTime,limitPlayer));
                        if(disableTrigger) DisableObject2D(srComponent,colComponent);
                        break;
                    case EffectType.BARRIER:
                        playerInstance.BarrierState = true;
                        if(disableTrigger) DisableObject2D(srComponent,colComponent);
                        break;
                    case EffectType.WATER:
                        isWater = true;
                        StartCoroutine(DelayDamageToPlayer(delayDamageTime));
                        playerJumpInstance.SetJumpLevel(1);
                        returnCoroutine = StartCoroutine( playerInstance.LimitVelocity(0, limitPlayer));
                        break;
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if(gameObject.CompareTag("Water"))
        {
            if (playerTag != null && col.CompareTag(playerTag))
            {
                playerJumpInstance.JumpStateReset(false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col) {
        if(gameObject.CompareTag("Water"))
        {
            if (playerTag != null && col.CompareTag(playerTag))
            {
                //print("실행 " + col.name);
                playerInstance.ClearLimitState();
                playerJumpInstance.SetJumpLevel(playerJumpInstance.InitJumpLevel);
                isWater = false;
            }
        }
    }
}
