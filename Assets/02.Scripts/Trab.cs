using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trab : ObjectInteraction {

    /* inspector variable */
    public int shootAngle = 0;
    public float distance = 1f;
    public float limitDistance = 0.5f;
    public float catchTime = 4f;
    [Range(0, 4)]
    public float speed = 1f;
    [Tooltip("target을 쫒아갈때 가속 속도입니다. default = 0")]
    [Range(0, 0.5f)]
    public float accelation = 0;
    [Tooltip("target을 쫒아갈때 한계 속도입니다. default = 10")]
    [Range(1, 4)]
    public float limitSpeed = 4f;
    public bool useTrigger = true;

    /* needs component */
    Rigidbody2D rg2d;
    Animator animator;

    /* need variable */
    Vector2 initPos;
    Vector2 dir = new Vector2(0,0);
    bool catchState = false;
    bool arrivedState = false;
    float gap = 0.1f;

    private void Start()
    {
        initPos = transform.position;
        rg2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetInteger("aniState", 0);
    }

    private void OnBecameVisible()
    {
        dir = AngleToVector2(shootAngle);
        StartCoroutine( ActivateToTrap() );
    }

    IEnumerator ActivateToTrap()
    {
        Vector2 destination = initPos + dir;
        while (state)
        {
            while ((CollisionTargetTransform == null) && useTrigger) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복
            print("Trab.cs - 실행중입니다.");
            if (!arrivedState) StartCoroutine(ObjectRepeatMove2D(rg2d, dir, distance, speed, 1));
            animator.SetInteger("aniState", 1);

            arrivedState = true; // 혹시라도 목적지에 도착하기 전 코루틴이 반복되지 않게 합니다.
            yield return new WaitForSeconds(speed * distance * 2);

            if (catchState)
            {
                animator.SetInteger("aniState", 2);
                rg2d.velocity = Vector3.zero;

                StopCoroutine("ObjectRepeatMove2D");
                float start = Time.time;
                float end = start;
                Vector2 catchPoint = transform.position;
                /* 시작 시간과 종료시간을 측정하면서 trap을 캐릭터에 고정시킨다. */
                while (end - start < catchTime)
                {
                    Vector2 limitMoveDistance;

                    /* limited player when trap catched player */
                    //if(CollisionTargetTransform.position.x > catchPoint.x + limitDistance || CollisionTargetTransform.position.x < catchPoint.x - limitDistance)
                    //{
                    //    CollisionTargetTransform.GetComponent<Rabbit>().
                    //}
                    //else if()
                    //{

                    //}

                    //if()
                    //{

                    //}
                    //else if()
                    //{

                    //}

                    //print(catchPoint);
                    yield return new WaitForFixedUpdate();
                    end += Time.deltaTime;
                }
                catchState = false;
                //yield return new WaitForSeconds(catchTime);
                CollisionTargetTransform = null; //충돌체를 초기화합니다.
                StartCoroutine(MoveToDestination(rg2d, initPos, speed * 2, accelation));
            }
            else
            {
                if (Vector2.Distance(transform.position, initPos) < gap && arrivedState)
                {
                    arrivedState = false;
                    animator.SetInteger("aniState", 0);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(CollisionTargetTransform != null)
        {
            if(col.CompareTag(CollisionTargetTransform.tag)) catchState = true;
        }
        else
        {
            if (col.CompareTag("Player")) catchState = true;
        }
    }
}
