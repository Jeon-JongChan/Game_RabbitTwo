using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    SHOOTING,
    GOTODESTINATION,
    REPEATING,
    TRACEOBJECTS
}
public enum DirType
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}
[System.Serializable]
public class TypeRepeating
{
    [Tooltip("반복 반환점에 도달시 잠시 멈추는 시간입니다. Default 0.5f")]
    public float stopTime;
    [Tooltip("반복 횟수입니다. 0을 줄 경우 무한히 반복합니다.")]
    public int repeatCount;
    public DirType repeatDir;

}
[System.Serializable]
public class TypeTraceObject
{
    [Tooltip("이동 지점. 순서대로 이동합니다.")]
    public GameObject[] point;
}
[RequireComponent(typeof(Rigidbody2D))]
[ExecuteInEditMode]
public class MovingObject : ObjectInteraction
{
    [Header("오브젝트 무빙 타입")]
    public MoveType selectType = MoveType.SHOOTING;
    public bool useTrigger = false;
    [Header("이동시 필요한 변수")]
    [Range(0,20)]
    public int speed = 0;
    [Tooltip("0 도부터 반시계방향. 0도는 오른쪽이다.")]
    [Range(0, 3)]
    public int direction = 0;
    [Tooltip("selected 1 or 2")]
    public float distance = 0;
    [Tooltip("selected 0 or 1")]
    [Range(0f,0.1f)]
    public float accelation = 0;
    public TypeRepeating repeating;
    public TypeTraceObject tracing;

    /* 필요 컴포넌트 */
    Rigidbody2D rb2d;

    /* 필요 변수 */
    Vector2 originPos;
    Vector2 dir;
    Coroutine returnCoroutine;

    private void Awake()
    {
        originPos = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        SaveState(!useTrigger, gameObject.activeSelf,transform.position);
    }
    private void Start()
    {
        if(selectType == MoveType.REPEATING)
        {
            switch(repeating.repeatDir)
            {
                case DirType.UP:
                    direction = 0;
                    break;
                case DirType.RIGHT:
                    direction = 1;
                    break;
                case DirType.DOWN:
                    direction = 2;
                    break;
                case DirType.LEFT:
                    direction = 3;
                    break;
                default:
                    print("방향설정 오류");
                    break;
            }
        }
        else dir = AngleToVector2(direction);
    }
    private void OnBecameVisible()
    {
        StartCoroutine(StartMovingObject());
    }
    private void OnBecameInvisible() {
        CollisionTargetTransform = null;
        if(returnCoroutine != null)StopCoroutine(returnCoroutine);
    }

    IEnumerator StartMovingObject()
    {
        if(useTrigger) while(CollisionTargetTransform == null) yield return new WaitForFixedUpdate();
        switch(selectType)
        {
            case MoveType.SHOOTING:
                returnCoroutine = StartCoroutine(StartObjectShoot(rb2d, dir, speed, accelation));
                print("movingObject.cs : 슈팅");
                break;
            case MoveType.GOTODESTINATION:
                Vector2 destination = originPos + (dir * distance);
                print("movingObject.cs : transform : " + originPos + " destination : " + destination);
                returnCoroutine = StartCoroutine(MoveToDestination(rb2d, destination, speed, accelation));
                print("movingObject.cs : 목적지");
                break;
            case MoveType.REPEATING:
                print("movingObject.cs : 반복");
                returnCoroutine = StartCoroutine(ObjectRepeatMove2D(rb2d, direction, distance, speed, repeating.repeatCount, repeating.stopTime));
                break;
            case MoveType.TRACEOBJECTS:
                print("movingObject.cs : 추적");
                if(tracing.point.Length > 0)
                {
                    returnCoroutine = StartCoroutine( ObjectTargetsTraceMovePos2D(rb2d, tracing.point,tracing.point.Length, speed) );
                }
                break;
        }
    }
    protected IEnumerator StartObjectShoot(Rigidbody2D rb2d,Vector2 dir, float speed, float accelation = 0)
    {
        while (true)
        {
            MovePos(rb2d, dir, speed);
            speed += accelation;
            yield return new WaitForEndOfFrame();
        }
    }
    protected void ToDestination()
    {
        Vector2 destination = originPos + (dir * distance);
        print("movingObject.cs : transform : " + originPos + " destination : " + destination);
        StartCoroutine(MoveToDestination(rb2d, destination, speed, accelation));
        print("movingObject.cs : 목적지");
    }
    /* 파괴하거나 비활성화시 동작을 멈추게 한다. */
    private void OnDisable()
    {
        print("movingObject.cs : 정지");
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        print("movingObject.cs : 정지");
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag != "Player")
        {
            print("movingObject.cs : 충돌정지");
            StopAllCoroutines();
        }
    }

}
