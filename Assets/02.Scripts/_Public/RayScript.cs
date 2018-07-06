﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PsybleScript
{
    public class RayScript : MonoBehaviour
    {

        /// <summary>
        /// self를 기준으로 방향에 따라 오브젝트를 탐지하는 선형 ray를 RayDistance까지 쏜다.
        /// </summary>
        public static GameObject DetectedRayCast2D(Vector2 selfPosition, Vector2 direction, float RayDistance = 1f, int layerMask = 0)
        {
            GameObject ret;
            RaycastHit2D hit2D;
            //레이 캐스트를 입력받은 방향으로 길이만큼 쏜다.
            hit2D = Physics2D.Raycast(selfPosition, direction, RayDistance, layerMask);
            //if noting, return null
            if (hit2D.collider == null)
            {
                return null;
            }
            ret = hit2D.collider.gameObject;
            return ret;
        }
        /// <summary>
        /// self를 기준으로 오브젝트를 탐지하는 원형 ray를 생성한다.
        /// </summary>
        public static GameObject DetectedOverlapCircle2D(Vector2 selfPosition, float radius = 0.5f, int layerMask = 0)
        {
            GameObject ret;
            Collider2D hit2D;
            //레이 캐스트를 입력받은 방향으로 길이만큼 쏜다.
            hit2D = Physics2D.OverlapCircle(selfPosition, radius, layerMask);
            //if noting, return null
            if (hit2D == null)
            {
                return null;
            }
            ret = hit2D.gameObject;
            return ret;
        }
    }
}
