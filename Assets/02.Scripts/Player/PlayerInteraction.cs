using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    /* inspector variable */
    [Header("TYPE")]
    public GameObject player = null;
    [Tooltip("0 - 점프횟수를 조작합니다 \n 1 - 점프나 움직임에 속력을 조작합니다.")]
    [Range(0,1)]
    public int selectedType = 0;
    [Tooltip("버프 or 디버프가 작동하는 시간입니다.")]
    public float delayTime = 1f;
    [Tooltip("작동후 끕니다.")]
    public bool disableTrigger = false;

    [Header("Selected 0")]
    public int addJumpLevel = 0;
    [Header("Selected 1")]
    public int limitPlayer = 0;

    /* needs components */
    Player playerInstance;
    PlayerJump2D playerJumpInstance;
    Collider2D colComponent;
    SpriteRenderer srComponent;

    /* needs variable */
    bool bugState = false;

    private void Start()
    {
        if (player != null)
        {
            playerInstance = player.GetComponent<Player>();
            playerJumpInstance = player.GetComponent<PlayerJump2D>();
        }
        else { bugState = true; }
        colComponent = GetComponent<BoxCollider2D>();
        srComponent = GetComponent<SpriteRenderer>();
    }
    void DisableObject()
    {
        if (disableTrigger)
        {
            colComponent.enabled = false;
            srComponent.enabled = false;
        }
    }
    public IEnumerator ControlJump()
    {
        DisableObject();
        playerJumpInstance.JumpLevelPlus(addJumpLevel);
        yield return new WaitForSeconds(delayTime);
        playerJumpInstance.JumpLevelMinus(addJumpLevel);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!bugState)
        {
            if (col.CompareTag(player.tag))
            {
                switch(selectedType)
                {
                    case 0:
                        StartCoroutine(ControlJump());
                        break;
                    case 1:
                        StartCoroutine(playerInstance.LimitVelocity(delayTime,limitPlayer));
                        DisableObject();
                        break;
                }
            }
        }
    }
}
