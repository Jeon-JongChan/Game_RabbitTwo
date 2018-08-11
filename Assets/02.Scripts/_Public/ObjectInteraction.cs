using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

public class ObjectInteraction : ObjectMovement2D, ISaveObject{


    protected bool detectState = false; //탐지를 정지시킬 수 있는 변수.
    protected Transform collisionTargetTransform = null; //Trigger 객체에서 전달하는 충돌체의 위치 포인트를 가리키는 방향벡터
    public Transform CollisionTargetTransform
    {
        get{ return collisionTargetTransform;}
        set
        {
            if(value is Transform)
            {
                collisionTargetTransform = value;
            }
        }
    }
    /* 초기화에 영향을 주는 변수들 */
    protected bool initState = true;
    protected bool state = true;
    protected bool initActive = true;
    protected Vector2 initPos = Vector2.zero;
    protected bool currActive = true;
    protected Vector2 currPos = Vector2.zero;


    /// <summary>
    /// 발사체를 발사시키는 함수
    /// </summary>
    /// <param name="selectedRayType"> If it is 0, it is OverlapCircle. 1 is RayCast</param>
    public IEnumerator BulletShot2D(Rigidbody2D rigidbody2D, Vector2 direction, float extinctionTime, float speed = 1f, bool phsicsState = false)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;

        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if(phsicsState) MovePos(rigidbody2D, direction, speed * 10, phsicsState);
        else
        {
            if (extinctionTime > 0)
            {
                /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
                while (currentTime - startedTime < extinctionTime && state)
                {
                    //print("ObjectInteraction - 남은시간 " + (currentTime - startedTime));
                        //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red);
                    MovePos(rigidbody2D, direction, speed, phsicsState);
                    yield return new WaitForFixedUpdate();
                    currentTime = Time.time;
                }
            }
            /* 0일 경우 오브젝트와 충돌 할 때까지 이동한다. */
            else if (extinctionTime == 0)
            {
                while(state)
                {
                    //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red, 5);
                    MovePos(rigidbody2D, direction, speed, phsicsState);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // 오브젝트와 충돌하고 나면 비활성화 한다.
        //Debug.Log("ObjectInteraction - 총알의 움직임이 종료되었습니다.");
        if(!phsicsState)rigidbody2D.gameObject.SetActive(false);
    }
    /// <summary>
    /// selectedKey를 통해 탐지방법을 전달받고 탐지 결과에 따라 protected 속성인 detectState 변수를 트리거.
    /// </summary>
    protected IEnumerator DetectObject(Rigidbody2D self, Vector2 direction, int selectedKey, float rayScale = 0.5f, int layerMask = 0, float detectTime = 0.01f, string targetTag = null)
    {
        int xxx = 0;
        GameObject ret = null;
        detectState = true;
        while (detectState)
        {
            switch (selectedKey)
            {
                case 0:
                    // print("ObjectInteraction - 0 번시작 " + layerMask);
                    ret = RayScript.DetectedOverlapCircle2D(self.position, rayScale, layerMask);
                    break;
                case 1:
                    //print("ObjectInteraction - 1 번시작 " + layerMask);
                    ret = RayScript.DetectedRayCast2D(self.position, direction, rayScale, layerMask);
                    break;
            }
            if (ret != null)
            {
                Debug.Log("ObjectInteraction - " + ret.name + "와 충돌했습니다.");
                if (targetTag == null) detectState = false;
                else
                {
                    if (ret.tag == targetTag) detectState = false;
                    else detectState = true;
                }
            }
            yield return new WaitForSeconds(detectTime);
        }

    }
    public virtual void DisableObject2D(SpriteRenderer sr,Collider2D col2d)
    {
        if(sr != null) sr.enabled = false;
        if(col2d != null) col2d.enabled = false;
    }
    public virtual void EnableObject2D(SpriteRenderer sr,Collider2D col2d)
    {
        if(sr != null) sr.enabled = true;
        if(col2d != null) col2d.enabled = true;
    }
    /// <summary>
    /// Trigger를 통해 목표의 위치를 갖고 오는 함수
    /// </summary>
    public virtual void SetCollisionTarget(Transform tf)
    {
        CollisionTargetTransform = tf;
    }

    /***************************  세이브 구현에 필요한  공통 함수들 *****************************/
    public virtual void SaveState(bool selfState, bool selfActive, Vector2 pos, bool init = false)
    {
        if(init)
        {
            initState = selfState;
            initActive = selfActive;
            initPos = pos;
            //현재 상태에 초기상태를 저장.
            state = initState;
            currActive = initActive;
            currPos = initPos;
        }
        else
        {
            state = selfState;
            currActive = selfActive;
            currPos = pos;
        }
    }

    public virtual bool LoadState(bool init = false)
    {
        if(init)
        {
            if ((Vector2)(transform.position) != initPos) transform.position = initPos;
            gameObject.SetActive(initActive);
            return initState;
        }
        else
        {
            if ((Vector2)(transform.position) != currPos) transform.position = currPos;
            gameObject.SetActive(currActive);
        }
        return state;
    }
}
