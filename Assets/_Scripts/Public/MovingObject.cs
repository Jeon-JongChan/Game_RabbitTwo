using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[ExecuteInEditMode]
public class MovingObject : ObjectMovement
{
    [Header("오브젝트 무빙 타입")]
    [Tooltip(" 0 - 슈팅 1 - 목적지 도달 2 - 반복. 대각선은 지원하지 않습니다.")]
    [Range(0, 2)]
    public int selectType = 0;

    [Header("이동시 필요한 변수")]
    [Range(0,20)]
    public int speed = 0;
    [Tooltip("0 - UP 1 - RIGHT 2 - DOWN 3 - LEFT ")]
    [Range(0, 3)]
    public int direction = 0;
    [Tooltip("selected 1 or 2")]
    public float distance = 0;
    [Tooltip("selected 0 or 1")]
    [Range(0f,0.1f)]
    public float accelation = 0;
    [Tooltip("selected 2 - 반복, Default 0.5f")]
    public float stopTime = 0.5f;
    [Tooltip("selected 2")]
    public int repeatCount = 0;

    /* 필요 컴포넌트 */
    Rigidbody2D rg2d;

    /* 필요 변수 */
    Vector2 originPos;
    Vector2 dir;

    private void Awake()
    {
        originPos = transform.position;
        rg2d = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        switch(direction)
        {
            case 0:
                dir = Vector2.up;
                break;
            case 1:
                dir = Vector2.right;
                break;
            case 2:
                dir = Vector2.down;
                break;
            case 3:
                dir = Vector2.left;
                break;
            default:
                print("방향설정 오류");
                break;
        }
    }
    private void OnBecameVisible()
    {
        switch(selectType)
        {
            case 0:
                StartCoroutine(StartMove(rg2d, dir, speed, accelation));
                print("슈팅 - movingObject");
                break;
            case 1:
                Vector2 destination = originPos + (dir * distance);
                print("transform : " + originPos + " destination : " + destination);
                StartCoroutine(MoveToDestination(rg2d, destination, speed, accelation));
                print("목적지 - movingObject");
                break;
            case 2:
                print("반복 - movingObject");
                StartCoroutine(ObjectRepeatMove2D(rg2d, direction, distance, speed, repeatCount, stopTime));
                break;
        }
    }

    IEnumerator StartMove(Rigidbody2D rg2d,Vector2 dir, float speed, float accelation = 0)
    {
        while (true)
        {
            MovePos(rg2d, dir, speed);
            speed += accelation;
            yield return new WaitForEndOfFrame();
        }
    }

    /* 파괴하거나 비활성화시 동작을 멈추게 한다. */
    private void OnDisable()
    {
        print("정지 - movingObject");
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        print("정지 - movingObject");
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag != "Player")
        {
            print("충돌정지 - movingObject");
            StopAllCoroutines();
        }
    }

}
