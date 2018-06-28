﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : ObjectMovement
{
    //상속 받은 자식만 state 변수 사용이 가능하다.
    protected bool state;


    /// <summary>
    /// 발사체를 발사시키는 함수
    /// </summary>
    /// <param name="selectedRayType"> If it is 0, it is OverlapCircle. 1 is RayCast</param>
    public IEnumerator BulletShot2D(Rigidbody2D rigidbody2D, Vector2 direction, float extinctionTime, float speed = 1f, int layerMask = 1, float rayScale = 1f, int selectedRayType = 0, bool sleepState = false)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;

        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if (extinctionTime > 0)
        {
            print("ObjectInteraction - 가동중 ");
            /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
            while (currentTime - startedTime < extinctionTime)
            {
                //print("ObjectInteraction - 남은시간 " + (currentTime - startedTime));
                /* 충돌한 오브젝트가 없을 경우 이동 */
                if (!SelectedDetectMethod(rigidbody2D.position, direction, selectedRayType, rayScale, layerMask))
                {
                    MovePos(rigidbody2D, direction, speed, sleepState);
                    print("ObjectInteraction - 가동중 " );
                    yield return new WaitForEndOfFrame();
                    currentTime = Time.time;
                }
            }
        }
        /* 0일 경우 오브젝트와 충돌 할 때까지 이동한다. */
        else if (extinctionTime == 0)
        {
            while(!SelectedDetectMethod(rigidbody2D.position, direction, selectedRayType, rayScale, layerMask))
            {
                MovePos(rigidbody2D, direction, speed);

                yield return new WaitForEndOfFrame();
            }
        }
        // 오브젝트와 충돌하고 나면 비활성화 한다.
        Debug.Log("ObjectInteraction - 총알의 움직임이 종료되었습니다.");
        rigidbody2D.gameObject.SetActive(false);
    }

    /// <summary>
    /// self를 기준으로 방향에 따라 오브젝트를 탐지하는 선형 ray를 RayDistance까지 쏜다.
    /// </summary>
    public GameObject DetectedRayCast2D(Vector2 selfPosition, Vector2 direction, float RayDistance = 1f,int exceptLayerNum = 0)
    {
        GameObject ret;
        RaycastHit2D hit2D;
        int layerMask = 1 << exceptLayerNum; //레이어 마스크 제작
        //레이 캐스트를 입력받은 방향으로 길이만큼 쏜다.
        hit2D = Physics2D.Raycast(selfPosition, direction, RayDistance, layerMask);
        //if noting, return null
        if(hit2D.collider == null)
        {
            return null;
        }
        ret = hit2D.collider.gameObject;
        return ret;
    }
    /// <summary>
    /// self를 기준으로 오브젝트를 탐지하는 원형 ray를 생성한다.
    /// </summary>
    public GameObject DetectedOverlapCircle2D(Vector2 selfPosition, float radius = 1f, int exceptLayerNum = 0)
    {
        GameObject ret;
        Collider2D hit2D;
        int layerMask = 1 << exceptLayerNum; //레이어 마스크 제작
        //레이 캐스트를 입력받은 방향으로 길이만큼 쏜다.
        hit2D = Physics2D.OverlapCircle(selfPosition, radius,layerMask);
        //if noting, return null
        if (hit2D == null)
        {
            return null;
        }
        ret = hit2D.gameObject;
        return ret;
    }
    /// <summary>
    /// selectedKey를 통해 탐지방법을 전달받고 탐지 결과에 따라 bool 변수를 반환.
    /// </summary>
    bool SelectedDetectMethod(Vector2 self, Vector2 direction, int selectedKey, float rayScale, int layerMask)
    {
        GameObject ret = null;
        switch(selectedKey)
        {
            case 0:
                ret = DetectedOverlapCircle2D( self, rayScale, layerMask);
                break;
            case 1:
                ret = DetectedRayCast2D(self, direction, rayScale, layerMask);
                break;
        }

        if (ret == null) return false;
        else
        {
            Debug.Log("ObjectInteraction - " + gameObject.name + "가 " + ret.name + "과 충돌했습니다.");
            return true;
        }

    }
}
