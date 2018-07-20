using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

public class BlankObject : ObjectInteraction {

    /* inspector variable */
    [Header("TYPE")]
    [Tooltip("0 - 투명해집니다. 콜리더는 존재합니다.\n1 - 주기적으로 깜박입니다. \n2 - 불규칙적으로 깜빡입니다. 최대 최소값을 지정해야 합니다.")]
    [Range(0,2)]
    public int selectType = 0;
    public bool useTrigger = false;
    [Tooltip("체크해제시 충돌체는 꺼지지 않습니다.")]
    public bool colliderOffTrigger = true;
    [Tooltip("트리거 사용 안할경우 블랭크 시작 딜레이 시간입니다.")]
    public int delayTime = 0;

    [Header("SelectType 1")]
    [Range(2,10)]
    [Tooltip("너무 빠르게 깜빡이면 사물이 끼어들어 오작동성이 높아짐.")]
    public float blankTime = 2f;

    [Header("SelectType 2")]
    [Tooltip("시간차를 너무 크게 안하는것을 추천합니다.")]
    [Range(2, 10)]
    public int maxTime = 4;
    [Range(2, 10)]
    public int minTime = 2;

    /* needs components */
    SpriteRenderer srComponets = null;
    Collider2D col = null;

    /* needs variable */
    bool SpriteRendererTrigger = true;
    bool ColliderTrigger = true;
    bool startStatus = false; //코루틴이 반복 호출되는 것을 막아줍니다.
    float detectRange = 0.5f;

    /* 세이브 변수 */
    bool colActive;
    bool srActive;

    private void Start()
    {
        srComponets = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        SaveState(false, gameObject.activeSelf, transform.position);
    }

    private void OnBecameVisible()
    {
        if (!startStatus)
        {
            if(!startStatus)StartCoroutine(StartBlank(srComponets, col,detectRange));
            startStatus = true;
        }
    }
    public override void SaveState(bool selfState, bool selfActive, Vector2 pos,bool init = false)
    {
        if (!col.enabled && col != null) colActive = col.enabled;
        if (!srComponets.enabled) srActive = srComponets.enabled;
        base.SaveState(selfState, selfActive, pos);
    }
    public override bool LoadState(bool init = false)
    {
        startStatus = initState;
        if (!srComponets.enabled) srComponets.enabled = srActive;
        if (!col.enabled && col != null) col.enabled = colActive;
        return base.LoadState();
    }

    IEnumerator StartBlank(SpriteRenderer srComponets, Collider2D col,float detectRange)
    {
        yield return new WaitForSeconds(delayTime); //일정시간동안 시작을 딜레이 합니다.
        int randomTime;
        int layerMask = 1 << 9;
        float detectDelayTime = blankTime;
        while ((CollisionTargetTransform == null) && useTrigger) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복
        while (startStatus)
        {
            while (RayScript.DetectedOverlapCircle2D(transform.position, detectRange, layerMask) != null)
            {
                if(!detectState)
                {
                    detectState = true;
                    offComponentState(srComponets, col);
                }
                print("실행");
                yield return new WaitForSeconds(detectDelayTime);
            }
            if(detectState) detectState = false;
            switch (selectType)
            {
                case 0:
                    ReverseComponentState(srComponets, col);
                    break;
                case 1:
                    //while 이 스위치 밖에 있을 경우 코드 실행 시간이 증가하므로 원하던 효과가 발생하지 않는다.
                    yield return new WaitForSeconds(blankTime);
                    /* bool 값을 처음 true로 저장 후 역전시켜가면서 깜빡이는 효과를 줍니다. */
                    ReverseComponentState(srComponets, col);
                    break;
                case 2:
                    randomTime = Random.Range(minTime, maxTime + 1); // 랜덤한 시간마다 깜빡입니다.
                    yield return new WaitForSeconds(randomTime);

                    ReverseComponentState(srComponets, col);
                    break;
            }
        }
    }

    void ReverseComponentState(SpriteRenderer srComponets, Collider2D col)
    {
        SpriteRendererTrigger = !SpriteRendererTrigger;
        ColliderTrigger = !ColliderTrigger;

        srComponets.enabled = SpriteRendererTrigger;
        if(col != null && colliderOffTrigger) col.enabled = ColliderTrigger;
    }

    void offComponentState(SpriteRenderer srComponets, Collider2D col)
    {
        SpriteRendererTrigger = false;
        ColliderTrigger = false;

        srComponets.enabled = SpriteRendererTrigger;
        if(col != null && colliderOffTrigger) col.enabled = ColliderTrigger;
    }
}
