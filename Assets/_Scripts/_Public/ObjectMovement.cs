﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PsybleScript
{
    public class ObjectMovement : MonoBehaviour
    {

        /// <summary>
        /// transform을 이용하여 위치를 이동시키는 함수.
        /// </summary>
        public static void MovePos(Transform tf, float x, float y, float z = 0, float speed = 1f)
        {

            Vector3 tempPos = tf.position;
            tempPos.x += x * speed * Time.deltaTime;
            tempPos.y += y * speed * Time.deltaTime;
            tempPos.z += z * speed * Time.deltaTime;

            tf.position = tempPos;
        }
        /// <summary>
        /// RIGIDBODY를 사용하여 3D OBJECET 위치를 이동시키는 함수.
        /// </summary>
        public static Vector3 MovePos(Rigidbody rb, Vector3 distanceToMove, float speed = 1)
        {
            Vector3 initVelocity = new Vector3(0, rb.velocity.y, 0); //점프에 해당하는 y축 힘 빼고 나머지 축을 0으로 초기화해서 물체 충돌시 반발력 제거
            rb.velocity = initVelocity;

            distanceToMove.Normalize(); //가야할 방향이 정해진다.
            rb.MovePosition(rb.position + (distanceToMove * speed * Time.deltaTime));

            return distanceToMove;
        }
        /// <summary>
        /// Velocity를 이용한 3D OBJECT 이동함수.
        /// </summary>
        public static Vector3 Move(Rigidbody rb, Vector3 distanceToMove, float speed = 1)
        {

            distanceToMove = distanceToMove.normalized * speed; //가야할 방향이 정해진다.
            distanceToMove.y = rb.velocity.y; //점프에 해당하는 velocity는 보존해줘야 한다.

            rb.velocity = distanceToMove;

            return distanceToMove;
        }
        /// <summary>
        /// 2D 이동함수, not player.
        /// </summary>
        /// <param name="sleepState"> 비활성화에서 켜지는 경우 sleep 상태일 경우가 있다. 이 경우 외부충격으로 awake 시켜야 한다.</param>
        public static Vector2 MovePos(Rigidbody2D rb, Vector2 distanceToMove, float speed = 1f, bool sleepState = false)
        {
            //distanceToMove.Normalize(); //가야할 방향이 정해진다.
            if (!sleepState) rb.MovePosition(rb.position + (distanceToMove * speed * Time.deltaTime));
            else rb.AddForce(distanceToMove * speed);

            return distanceToMove;
        }
        /// <summary>
        /// Velocity를 이용한 2D 이동함수.not player
        /// </summary>
        public static Vector2 Move(Rigidbody2D rb, Vector2 distanceToMove, float speed = 1)
        {
            distanceToMove = distanceToMove.normalized * speed;
            distanceToMove.y = rb.velocity.y;

            rb.velocity = distanceToMove;

            return distanceToMove;
        }
        //목적지까지 가는 함수
        public static IEnumerator MoveToDestination(Rigidbody2D rb, Vector2 destination, float speed = 1f, float accelation = 0)
        {
            float gap = 0.02f * speed;
            Vector2 dir = destination - rb.position;
            dir.Normalize();

            while (Vector2.Distance(rb.position, destination) > gap)
            {
                MovePos(rb, dir, speed);
                speed += accelation;
                yield return new WaitForEndOfFrame();
            }
            rb.MovePosition(destination);
        }
        //목적지까지 가는 함수
        public static IEnumerator MoveToDestination(Rigidbody rb, Vector3 destination, float speed = 1f, float accelation = 0)
        {
            float gap = 0.02f * speed;
            Vector3 dir = destination - rb.position;
            dir.Normalize();

            while (Vector3.Distance(rb.position, destination) > gap)
            {
                MovePos(rb, dir, speed);
                speed += accelation;
                yield return new WaitForEndOfFrame();
            }
            rb.MovePosition(destination);
        }

        public static void Turn(Rigidbody rb, Vector3 moveRotation, float rotationSpeed = 1f)
        {
            moveRotation.Normalize();
            Quaternion rot = Quaternion.LookRotation(moveRotation);

            rb.rotation = Quaternion.Slerp(rb.rotation, rot, rotationSpeed * Time.deltaTime);
        }
        /// <summary>
        /// 각도를 받아서 라디안으로 전환 후 cos sin 계산을 통해서 벡터로 전환한다.
        /// </summary>
        public static Vector2 AngleToVector2(int angle)
        {
            Vector2 ret;
            float radian = Mathf.Deg2Rad * angle;
            ret = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            return ret;
        }
        /// <summary>
        /// transform을 이용한 점프함수
        /// </summary>
        public static IEnumerator JumpTransform(Transform tf, float jump = 1, float fallSpeed = 0.04f)
        {
            float i = -1f, originY = tf.position.y, distanceY = tf.position.y + jump;
            Vector3 distance, origin;

            while (i < 1)
            {
                origin = tf.position;
                distance = tf.position;

                origin.y = originY;
                distance.y = distanceY;

                tf.position = Vector3.Lerp(origin, distance, -(i * i) + 1);
                i += fallSpeed;

                yield return null;//new WaitForFixedUpdate();
            }
        }
        /// <summary>
        /// velocity를 이용한 점프함수
        /// </summary>
        public static void Jump(Rigidbody rb, float jumpHeight = 1f)
        {
            Vector3 origin;

            origin = rb.velocity;
            origin.y = jumpHeight;

            rb.velocity = origin;

        }
        /// <summary>
        /// velocity를 이용한 점프함수
        /// </summary>
        public static void Jump(Rigidbody2D rb, float jumpHeight = 2f)
        {
            Vector2 origin;

            origin = rb.velocity;
            origin.y = jumpHeight;

            rb.velocity = origin;
        }
        /// <summary>
        /// velocity를 이용한 점프함수
        /// </summary>
        public static void Jump(Rigidbody2D rb, float jumpHeight = 2f, ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            rb.AddForce(Vector2.up * jumpHeight, forceMode);
        }
        //방향 설정 열거형 변수들 - 가독성증가를 위해 사용
        enum direction { moveUp = 0, moveRight = 1, moveDown = 2, moveLeft = 3, moveFront = 4, moveBack = 5 };
        /// <summary>
        /// 매개변수로 받은 오브젝트를 repeatDistance만큼 반복이동 시킨다.
        /// transform을 사용해서 움직인다.
        /// </summary>
        public static IEnumerator ObjectRepeatMove2D(GameObject selfGameObject, int desiredDirection, float repeatDistance, float moveSpeed = 1f, int repeatCount = 0, float stopTime = 0.5f)
        {
            int i = 0;
            Vector3 destination;
            bool whileState = false;

            destination = selfGameObject.transform.position;

            // repeatCount가 0일경우 무한반복으로 정의하기 위해 whileState를 true로 설정해준다.
            if (repeatCount == 0) whileState = true;

            // 목적지까지 도달하면 dir변수에 -1을 곱해서 되돌아가는 패턴
            float dir;

            // 아래와 왼쪽은 시작시 dir -1 로 시작해야한다.
            if (desiredDirection < 2) dir = 1;
            else dir = -1;

            switch (desiredDirection)
            {
                case (int)direction.moveUp:
                    destination.y += repeatDistance * dir;
                    // i 와 repeatCount를 비교한후 i를 +1 증가 시킨다. i 가 repeatCount보다 크거나 whileState가 true면 반복된다.
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfGameObject.transform, 0, dir, 0, moveSpeed);

                        if (dir > 0)
                        {
                            if (selfGameObject.transform.position.y > destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfGameObject.transform.position.y < destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveRight:
                    destination.x += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfGameObject.transform, dir, 0, 0, moveSpeed);
                        if (dir > 0)
                        {
                            if (selfGameObject.transform.position.x > destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfGameObject.transform.position.x < destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveDown:
                    destination.y += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfGameObject.transform, 0, dir, 0, moveSpeed);

                        if (dir > 0)
                        {
                            if (selfGameObject.transform.position.y > destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfGameObject.transform.position.y < destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveLeft:
                    destination.x += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfGameObject.transform, dir, 0, 0, moveSpeed);

                        if (dir > 0)
                        {
                            if (selfGameObject.transform.position.x > destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfGameObject.transform.position.x < destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                default:
                    whileState = false;
                    i = 0;
                    break;
            }

            yield return null;
        }
        /// <summary>
        /// 매개변수로 받은 오브젝트를 repeatDistance만큼 반복이동 시킨다. repeatCount = 0일 경우 무한반복한다.
        /// </summary>
        public static IEnumerator ObjectRepeatMove2D(Rigidbody2D selfRigidbody, int desiredDirection, float repeatDistance, float moveSpeed = 1f, int repeatCount = 0, float stopTime = 0.5f)
        {
            int i = 0;
            Vector3 destination;
            bool whileState = false;

            destination = selfRigidbody.transform.position;

            // repeatCount가 0일경우 무한반복으로 정의하기 위해 whileState를 true로 설정해준다.
            if (repeatCount == 0) whileState = true;

            // 목적지까지 도달하면 dir변수에 -1을 곱해서 되돌아가는 패턴
            float dir;

            // 아래와 왼쪽은 시작시 dir -1 로 시작해야한다.
            if (desiredDirection < 2) dir = 1;
            else dir = -1;

            switch (desiredDirection)
            {
                case (int)direction.moveUp:
                    destination.y += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector2(0, dir), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.y > destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.y < destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveRight:
                    destination.x += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector2(dir, 0), moveSpeed);
                        if (dir > 0)
                        {
                            if (selfRigidbody.position.x > destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.x < destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveDown:
                    destination.y += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector2(0, dir), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.y > destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.y < destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveLeft:
                    destination.x += repeatDistance * dir;
                    while (whileState || i++ < repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector2(dir, 0), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.x > destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.x < destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                default:
                    whileState = false;
                    i = 0;
                    break;
            }

            yield return null;
        }
        /// <summary>
        /// 매개변수로 받은 오브젝트를 repeatDistance만큼 반복이동 시킨다. repeatCount = 0일 경우 무한반복한다.
        /// </summary>
        public  IEnumerator ObjectRepeatMove(Rigidbody selfRigidbody, int desiredDirection, float repeatDistance, float moveSpeed = 1f, int repeatCount = 0, float stopTime = 0.5f)
        {
            int i = 0;
            Vector3 destination;
            bool whileState = false;

            destination = selfRigidbody.transform.position;
            // repeatCount가 0일경우 무한반복으로 정의하기 위해 whileState를 true로 설정해준다.
            if (repeatCount == 0) whileState = true;

            // 목적지까지 도달하면 dir변수에 -1을 곱해서 되돌아가는 패턴
            float dir = 0;

            // 아래와 왼쪽, 뒷면은 시작시 dir -1 로 시작해야한다.
            if (desiredDirection < 2 || desiredDirection == 4) dir = 1;
            else if ((desiredDirection >= 2 && desiredDirection < 4) || desiredDirection == 5) dir = -1;
            switch (desiredDirection)
            {
                case (int)direction.moveUp:
                    destination.y += repeatDistance * dir;
                    while (whileState || i++ > repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector3(0, dir, 0), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.y > destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.y < destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir == 0) break;
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveRight:
                    destination.x += repeatDistance * dir;
                    while (whileState || i++ > repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector3(dir, 0, 0), moveSpeed);
                        if (dir > 0)
                        {
                            if (selfRigidbody.position.x > destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.x < destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir == 0) break;
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveDown:
                    destination.y += repeatDistance * dir;
                    while (whileState || i++ > repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector3(0, dir, 0), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.y > destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.y < destination.y)
                            {
                                dir = -dir;
                                destination.y += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir == 0) break;
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveLeft:
                    destination.x += repeatDistance * dir;
                    while (whileState || i++ > repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector3(dir, 0, 0), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.x > destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.x < destination.x)
                            {
                                dir = -dir;
                                destination.x += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir == 0) break;
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveFront:
                    destination.z += repeatDistance * dir;
                    while (whileState || i++ > repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector3(0, 0, dir), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.z > destination.z)
                            {
                                dir = -dir;
                                destination.z += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.z < destination.z)
                            {
                                dir = -dir;
                                destination.z += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir == 0) break;
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case (int)direction.moveBack:
                    destination.z += repeatDistance * dir;
                    while (whileState || i++ > repeatCount)
                    {
                        MovePos(selfRigidbody, new Vector3(0, 0, dir), moveSpeed);

                        if (dir > 0)
                        {
                            if (selfRigidbody.position.z > destination.z)
                            {
                                dir = -dir;
                                destination.z += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir < 0)
                        {
                            if (selfRigidbody.position.z < destination.z)
                            {
                                dir = -dir;
                                destination.z += repeatDistance * dir;
                                yield return new WaitForSeconds(stopTime);
                            }
                        }
                        else if (dir == 0) break;
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                default:
                    whileState = false;
                    i = 0;
                    break;
            }

            yield return null;
        }
        /// <summary>
        /// 오브젝트 배열 순서대로 추적해서 오브젝트를 이동시키는 함수
        /// </summary>
        /// <param name="targets">도착지점을 가리키는 오브젝트들의 배열</param>
        public IEnumerator ObjectTargetsTraceMove2D(Rigidbody2D selfRigidbody, GameObject[] targets, int targetsLen, float speed = 1, float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            int i = 0;
            Vector2 destination;
            Vector2 distanceToMove;
            if (targets.Length > 0)
            {
                while (i < targetsLen)
                {
                    destination = targets[i].transform.position;
                    distanceToMove = destination - selfRigidbody.position;
                    print(distanceToMove);

                    while (!(Vector2.Distance(destination, selfRigidbody.position) < limit && Vector2.Distance(destination, selfRigidbody.position) > -limit))
                    {
                        speed += acceleration;
                        MovePos(selfRigidbody, distanceToMove, speed);
                        yield return null;
                    }
                    selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                    selfRigidbody.velocity = Vector2.zero;
                    i++;
                }
            }
            yield return null;
        }
        /// <summary>
        /// 목표 오브젝트가 있던 곳으로 오브젝트를 이동시키는 함수
        /// </summary>
        /// <param name="targets">도착지점을 가리키는 오브젝트</param>
        public IEnumerator ObjectTargetTraceMove2D(Rigidbody2D selfRigidbody, GameObject target, int targetsLen, float speed = 1, float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            Vector2 destination;
            Vector2 distanceToMove;
            if (target != null)
            {
                destination = target.transform.position;
                distanceToMove = destination - selfRigidbody.position;
                print(distanceToMove);

                while (!(Vector2.Distance(destination, selfRigidbody.position) < limit && Vector2.Distance(destination, selfRigidbody.position) > -limit))
                {
                    speed += acceleration;
                    MovePos(selfRigidbody, distanceToMove, speed);
                    yield return null;
                }
                selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                selfRigidbody.velocity = Vector2.zero;
            }
            yield return null;
        }
        /// <summary>
        /// 오브젝트 배열 순서대로 추적해서 오브젝트를 이동시키는 함수
        /// </summary>
        public IEnumerator ObjectTargetsTraceMove(Rigidbody selfRigidbody, GameObject[] targets, int targetsLen, float speed = 1, float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            int i = 0;
            Vector3 destination;
            Vector3 distanceToMove;
            if (targets.Length > 0)
            {
                while (i < targetsLen)
                {
                    destination = targets[i].transform.position;
                    distanceToMove = destination - selfRigidbody.position;
                    distanceToMove.Normalize();
                    print(distanceToMove);

                    while (!(Vector3.Distance(destination, selfRigidbody.position) < limit && Vector3.Distance(destination, selfRigidbody.position) > -limit))
                    {
                        speed += acceleration;

                        MovePos(selfRigidbody, distanceToMove, speed);
                        yield return null;
                    }
                    selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                    i++;
                    selfRigidbody.velocity = Vector3.zero;
                }
            }
            yield return null;
        }
        /// <summary>
        /// 목표 오브젝트가 있던 곳으로 오브젝트를 이동시키는 함수
        /// </summary>
        public IEnumerator ObjectTargetTraceMove(Rigidbody selfRigidbody, GameObject target, int targetsLen, float speed = 1, float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            Vector3 destination;
            Vector3 distanceToMove;
            if (target != null)
            {
                destination = target.transform.position;
                distanceToMove = destination - selfRigidbody.position;
                distanceToMove.Normalize();
                print(distanceToMove);

                while (!(Vector3.Distance(destination, selfRigidbody.position) < limit && Vector3.Distance(destination, selfRigidbody.position) > -limit))
                {
                    speed += acceleration;

                    MovePos(selfRigidbody, distanceToMove, speed);
                    yield return null;
                }
                selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                selfRigidbody.velocity = Vector3.zero;
            }
            yield return null;
        }
    }
}