using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankObject : ObjectInteraction {

    /* inspector variable */
    [Header("TYPE")]
    [Tooltip("0 - 투명해집니다. 콜리더는 존재합니다.\n1 - 주기적으로 깜박입니다. \n2 - 불규칙적으로 깜빡입니다. 최대 최소값을 지정해야 합니다.")]
    public int selectType = 0;
    public bool useTrigger = false;
    [Tooltip("체크해제시 충돌체는 꺼지지 않습니다.")]
    public bool colliderOffTrigger = true;
    [Tooltip("트리거 사용 안할경우 블랭크 시작 딜레이 시간입니다.")]
    public int delayTime = 0;

    [Header("SelectType 1")]
    public float blankTime = 1f;

    [Header("SelectType 2")]
    [Tooltip("시간차를 너무 크게 안하는것을 추천합니다.")]
    public int maxTime = 2;
    public int minTime = 1;

    /* needs components */
    SpriteRenderer srComponets = null;
    Collider2D col = null;

    /* needs variable */
    bool SpriteRendererTrigger = true;
    bool ColliderTrigger = true;

    private void Start()
    {
        srComponets = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    private void OnBecameVisible()
    {
        StartCoroutine(StartBlank());
    }

    public override void LoadInitState()
    {
        base.LoadInitState();
        if (!srComponets.enabled) srComponets.enabled = true;
        if (!col.enabled && col != null) col.enabled = true;
    }

    IEnumerator StartBlank()
    {
        yield return new WaitForSeconds(delayTime); //일정시간동안 시작을 딜레이 합니다.
        int randomTime;
        while ((CollisionTargetTransform == null) && useTrigger) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복
        switch (selectType)
        {
            case 0:
                ReverseComponentState();
                break;
            case 1:
                //while 이 스위치 밖에 있을 경우 코드 실행 시간이 증가하므로 원하던 효과가 발생하지 않는다.
                while (state)
                {
                    /* bool 값을 처음 true로 저장 후 역전시켜가면서 깜빡이는 효과를 줍니다. */
                    ReverseComponentState();
                    yield return new WaitForSeconds(blankTime);
                }
                break;
            case 2:
                while (state)
                {
                    randomTime = Random.Range(minTime, maxTime + 1); // 랜덤한 시간마다 깜빡입니다.
                    yield return new WaitForSeconds(randomTime);

                    ReverseComponentState();
                    print("깜빡이는중 2 ");
                }
                break;
        }
        yield return new WaitForFixedUpdate();
    }

    void ReverseComponentState()
    {
        SpriteRendererTrigger = !SpriteRendererTrigger;
        ColliderTrigger = !ColliderTrigger;

        srComponets.enabled = SpriteRendererTrigger;
        if(col != null && colliderOffTrigger) col.enabled = ColliderTrigger;
    }
}
