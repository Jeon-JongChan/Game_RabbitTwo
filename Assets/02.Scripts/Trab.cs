using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trab : ObjectInteraction {

    /* inspector variable */
    public int shootAngle = 0;
    public float distance = 1f;
    public float catchTime = 1f;
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
    }

    private void OnBecameVisible()
    {
        dir = AngleToVector2(shootAngle);
        dir = dir * distance;
        StartCoroutine( ActivateToTrap() );
    }

    IEnumerator ActivateToTrap()
    {
        Vector2 destination = initPos + dir;
        while (state)
        {
            while ((CollisionTargetTransform == null) && useTrigger) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복
            print("Trab.cs - 실행중입니다.");
            if (!arrivedState) StartCoroutine(MoveToDestination(rg2d, destination, speed,accelation));

            arrivedState = true; // 혹시라도 목적지에 도착하기 전 코루틴이 반복되지 않게 합니다.
            yield return new WaitForSeconds(speed * distance);

            if (catchState)
            {
                StopCoroutine("MoveToDestination");
                transform.position = CollisionTargetTransform.position;
                yield return new WaitForSeconds(catchTime);
                destination = initPos;
                StartCoroutine(MoveToDestination(rg2d, destination, speed, accelation));
            }
            Vector2 temp = transform.position;
            if (Vector2.Distance(temp, destination) < gap && arrivedState)
            {
                dir *= -1;
                destination = initPos + dir; // -1 을 곱해 방향과 거리를 거꾸로 해준후 더해주면 왔던길로 돌아가는 길이 됩니다.
                arrivedState = false;
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
