using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    NONE,
    SHOOTING,
    GOTODESTINATION,
    REPEATING,
    TRACEOBJECTS,
    TARGETING
}

[System.Serializable]
public class TypeRepeating
{
    [Tooltip("반복 반환점에 도달시 잠시 멈추는 시간입니다. Default 0.5f")]
    public float stopTime;
    [Tooltip("반복 횟수입니다. 0을 줄 경우 무한히 반복합니다.")]
    public int repeatCount;

}
[System.Serializable]
public class TypeTraceObject
{
    [Tooltip("이동 지점. 순서대로 이동합니다.")]
    public GameObject[] point;
}
[RequireComponent(typeof(Rigidbody2D))]
public class MovingObject : ObjectInteraction
{
    [Header("오브젝트 무빙 타입")]
    public MoveType selectType = MoveType.SHOOTING;
    [SerializeField] bool useTrigger = false;
    [Tooltip("트리거 사용시 재작동을 위한 코드입니다.")]
    [SerializeField] bool reuseTrigger = false;
    [Header("이동시 필요한 변수")]
    [Range(0,100)]
    [SerializeField] int speed = 0;
    [Tooltip("0 도부터 반시계방향. 0도는 오른쪽이다.")]
    [SerializeField] int direction = 0;
    [Tooltip("selected 1 or 2")]
    [SerializeField] float distance = 0;
    [SerializeField] float _startDelayTime = 0;
    [Tooltip("selected 0 or 1")]
    [Range(0f,0.1f)]
    [SerializeField] float accelation = 0;
    [SerializeField] TypeRepeating repeating;
    [SerializeField] TypeTraceObject tracing;

    /* 필요 컴포넌트 */
    Rigidbody2D rb2d;

    /* 필요 변수 */
    Vector2 originPos;
    Vector2 dir;
    Coroutine returnCoroutine;
    WaitForSeconds wsReuseDelay;
    float reuseDelayTime = 1f; //무빙오브젝트 작동이 반복될 때 하고있는 작업이 끝날때 까지의 시간.
    int nonReuseCnt = 1;

    private void Awake()
    {
        originPos = transform.position;
        SaveState(!useTrigger, gameObject.activeSelf,transform.position);
    }
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if(MoveType.REPEATING == selectType)
        {
            if(repeating.repeatCount != 0)wsReuseDelay = new WaitForSeconds((repeating.stopTime * 2)  * repeating.repeatCount);
        }
        else
        {
            wsReuseDelay = new WaitForSeconds(reuseDelayTime);
        }
        dir = AngleToVector2(direction);
        //returnCoroutine = StartCoroutine(StartMovingObject());
    }
    private void OnBecameVisible()
    {
        if(gameObject.activeSelf) returnCoroutine = StartCoroutine(StartMovingObject());
    }
    private void OnBecameInvisible() {
        CollisionTargetTransform = null;
        if(returnCoroutine != null)StopCoroutine(returnCoroutine);
    }

    IEnumerator StartMovingObject()
    {
        yield return new WaitForSeconds(_startDelayTime);
        //한번은 무조건 실행되게 한다.
        while(reuseTrigger || nonReuseCnt-- > 0)
        {
            if(useTrigger) while(CollisionTargetTransform == null) yield return new WaitForFixedUpdate();
            switch(selectType)
            {
                case MoveType.SHOOTING:
                    returnCoroutine = StartCoroutine(StartObjectShoot(rb2d, dir, speed, accelation));
                    //print("movingObject.cs : 슈팅");
                    break;
                case MoveType.GOTODESTINATION:
                    Vector2 destination = originPos + (dir * distance);
                    //print("movingObject.cs : transform : " + originPos + " destination : " + destination);
                    returnCoroutine = StartCoroutine(MoveToDestination(rb2d, destination, speed, accelation));
                    //print("movingObject.cs : 목적지 : " + dir);
                    break;
                case MoveType.REPEATING:
                    //print("movingObject.cs : 반복");
                    returnCoroutine = StartCoroutine(ObjectRepeatMove2D(rb2d, dir, distance, speed, repeating.repeatCount, repeating.stopTime));
                    break;
                case MoveType.TRACEOBJECTS:
                    //print("movingObject.cs : 추적");
                    if(tracing.point.Length > 0)
                    {
                        returnCoroutine = StartCoroutine( ObjectTargetsTraceMovePos2D(rb2d, tracing.point,tracing.point.Length, speed) );
                    }
                    break;
                case MoveType.TARGETING:
                    if(CollisionTargetTransform != null)
                    {
                        returnCoroutine = StartCoroutine(ObjectTargetTraceMove2D(rb2d,CollisionTargetTransform.gameObject,speed,accelation));
                    }
                    break;
                case MoveType.NONE:
                    break;
            }
            CollisionTargetTransform = null;
            yield return wsReuseDelay;
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
        returnCoroutine = StartCoroutine(MoveToDestination(rb2d, destination, speed, accelation));
        print("movingObject.cs : 목적지");
    }
    /* 파괴하거나 비활성화시 동작을 멈추게 한다. */
    private void OnDisable()
    {
        //print("movingObject.cs : 정지");
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        //print("movingObject.cs : 정지");
        StopAllCoroutines();
    }

}
