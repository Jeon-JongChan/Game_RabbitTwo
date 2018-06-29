using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

public class ObjectInteraction : ObjectMovement,IDamageable
{
    [Tooltip("이것은 해당 오브젝트의 체력입니다. \n-1일경우 무적입니다.")]
    public int health = -1;

    //상속 받은 자식만 변수 사용이 가능하다. static 선언이 없으면 상속받은 객체마다 다른 메모리 공간을 가지게 된다.
    protected bool detectState = true;
    protected bool dead = false;


    /// <summary>
    /// 발사체를 발사시키는 함수
    /// </summary>
    /// <param name="selectedRayType"> If it is 0, it is OverlapCircle. 1 is RayCast</param>
    public IEnumerator BulletShot2D(Rigidbody2D rigidbody2D, Vector2 direction, float extinctionTime, float speed = 1f,int layerMask = 0, float rayScale = 0.5f, int selectedRayType = 0, string targetTag = null, bool sleepState = false)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;

        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if (extinctionTime > 0)
        {
            /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
            while (currentTime - startedTime < extinctionTime)
            {
                //print("ObjectInteraction - 남은시간 " + (currentTime - startedTime));
                    //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red);
                MovePos(rigidbody2D, direction, speed, sleepState);
                yield return new WaitForEndOfFrame();
                currentTime = Time.time;
            }
        }
        /* 0일 경우 오브젝트와 충돌 할 때까지 이동한다. */
        else if (extinctionTime == 0)
        {
            //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red, 5);
            MovePos(rigidbody2D, direction, speed);

            yield return new WaitForEndOfFrame();
        }
        // 오브젝트와 충돌하고 나면 비활성화 한다.
        Debug.Log("ObjectInteraction - 총알의 움직임이 종료되었습니다.");
        rigidbody2D.gameObject.SetActive(false);
    }
    /// <summary>
    /// selectedKey를 통해 탐지방법을 전달받고 탐지 결과에 따라 bool 변수를 반환.
    /// </summary>
    protected IEnumerator DetectObject(Vector2 self, Vector2 direction, int selectedKey, float rayScale = 0.5f, int layerMask = 0, float detectTime = 0.01f, string targetTag = null)
    {
        GameObject ret = null;
        detectState = true;
        while (detectState)
        {
            switch (selectedKey)
            {
                case 0:
                    print("ObjectInteraction - 0 번시작 " + layerMask);
                    ret = RayScript.DetectedOverlapCircle2D(self, rayScale, layerMask);
                    break;
                case 1:
                    print("ObjectInteraction - 1 번시작 " + layerMask);
                    ret = RayScript.DetectedRayCast2D(self, direction, rayScale, layerMask);
                    break;
            }

            if (ret == null)
            {
                print("ObjectInteraction -  없다");
                detectState = true;
            }
            else
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
    public void TakeHit(int Damage)
    {
        if(health > 0)
        {
            health -= Damage;
        }
        else if(health == 0)
        {
            Die();
        }
        else if(health < 0)
        {
            Debug.Log("ObjectInteraction.cs - it's invincibility");
        }
    }
    void Die()
    {
        dead = true;
    }
}
