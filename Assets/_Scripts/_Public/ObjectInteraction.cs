﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

public class ObjectInteraction : ObjectMovement
{
    protected bool detectState = false;

    /// <summary>
    /// 발사체를 발사시키는 함수
    /// </summary>
    /// <param name="selectedRayType"> If it is 0, it is OverlapCircle. 1 is RayCast</param>
    public IEnumerator BulletShot2D(Rigidbody2D rigidbody2D, Vector2 direction, float extinctionTime, float speed = 1f, bool sleepState = false)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;
        sleepState = false;
        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if (extinctionTime > 0)
        {
            /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
            while (currentTime - startedTime < extinctionTime)
            {
                //print("ObjectInteraction - 남은시간 " + (currentTime - startedTime));
                    //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red);
                MovePos(rigidbody2D, direction, speed, sleepState);
                yield return new WaitForFixedUpdate();
                currentTime = Time.time;
            }
        }
        /* 0일 경우 오브젝트와 충돌 할 때까지 이동한다. */
        else if (extinctionTime == 0)
        {
            //Debug.DrawRay(rigidbody2D.position, Vector2.up * rayScale, Color.red, 5);
            MovePos(rigidbody2D, direction, speed);

            yield return new WaitForFixedUpdate();
        }
        // 오브젝트와 충돌하고 나면 비활성화 한다.
        //Debug.Log("ObjectInteraction - 총알의 움직임이 종료되었습니다.");
        rigidbody2D.gameObject.SetActive(false);
    }
    /// <summary>
    /// selectedKey를 통해 탐지방법을 전달받고 탐지 결과에 따라 protected 속성인 detectState 변수를 트리거.
    /// </summary>
    protected IEnumerator DetectObject(Rigidbody2D self, Vector2 direction, int selectedKey, float rayScale = 0.5f, int layerMask = 0, float detectTime = 0.01f, string targetTag = null)
    {
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


}
