using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PsybleScript
{
    public class ObjectMovement2D : MonoBehaviour
    {
    
        /// <summary>
        /// 2D 이동함수, not player.
        /// </summary>
        public Vector2 MovePos(Rigidbody2D rb, Vector2 direction, float speed , bool phsicsState = false)
        {
            //distanceToMove.Normalize(); //가야할 방향이 정해진다.
            if (!phsicsState) rb.MovePosition(rb.position + (direction * speed * Time.deltaTime));
            else rb.AddForce(direction * speed * 10);

            return direction;
        }
        /// <summary>
        /// transform을 이용하여 위치를 이동시키는 함수.
        /// </summary>
        public void MovePos(Transform tf, float x, float y, float z = 0, float speed = 1f)
        {

            Vector3 tempPos = tf.position;
            tempPos.x += x * speed * Time.deltaTime;
            tempPos.y += y * speed * Time.deltaTime;
            tempPos.z += z * speed * Time.deltaTime;

            tf.position = tempPos;
        }
        /// <summary>
        /// Velocity를 이용한 2D 이동함수.not player
        /// </summary>
        public Vector2 Move(Rigidbody2D rb, Vector2 distanceToMove, float speed = 1, bool isJump = true)
        {
            distanceToMove = distanceToMove.normalized * speed;
            if(isJump)distanceToMove.y = rb.velocity.y;

            rb.velocity = distanceToMove;

            return distanceToMove;
        }
        //목적지까지 가는 함수
        public IEnumerator MoveToDestination(Rigidbody2D rb, Vector2 destination, float speed = 1f, float accelation = 0, bool sleepState = false)
        {
            float gap = 0.02f * speed;
            Vector2 dir = destination - rb.position;
            dir.Normalize();

            while (Vector2.Distance(rb.position, destination) > gap)
            {
                MovePos(rb, dir, speed, sleepState);
                speed += accelation;
                yield return new WaitForFixedUpdate();
            }
            rb.MovePosition(destination);
        }
        /// <summary>
        /// 각도를 받아서 라디안으로 전환 후 cos sin 계산을 통해서 벡터로 전환한다.
        /// </summary>
        public Vector2 AngleToVector2(int angle)
        {
            Vector2 ret;
            float radian = Mathf.Deg2Rad * angle;
            ret = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            return ret;
        }
        /// <summary>
        /// 반사각을 벡터로 반환합니다.
        /// </summary>
        public Vector2 GetReflectAngleVector2D(Collision2D col,Vector2 incidenceAngle)
        {
            Vector2 ret;
            Vector2 nomalVector = col.contacts[0].normal;
            incidenceAngle = incidenceAngle.normalized;
            ret = Vector2.Reflect(incidenceAngle,nomalVector);
            return ret;
        }
        /// <summary>
        /// transform을 이용한 점프함수
        /// </summary>
        public IEnumerator JumpTransform(Transform tf, float jump = 1, float fallSpeed = 0.04f)
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
        public void Jump(Rigidbody2D rb, float jumpHeight = 2f)
        {
            Vector2 origin;

            origin = rb.velocity;
            origin.y = jumpHeight;

            rb.velocity = origin;
        }
        /// <summary>
        /// velocity를 이용한 점프함수
        /// </summary>
        public void Jump(Rigidbody2D rb, float jumpPower, ForceMode2D forceMode)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpPower, forceMode);
        }

        //매우 위험한 변수. 타 클래스에서 코루틴 무빙함수를 사용시 코루틴을 종료시키기 위해 사용.
        protected bool whileState = true; 
        public IEnumerator ObjectRepeatMove2D(GameObject selfGameObject, Vector2 direction, float repeatDistance, float moveSpeed = 1f, int repeatCount = 0, float stopTime = 0.5f)
        {
            WaitForSeconds wsStopTime = new WaitForSeconds(stopTime);
            WaitForFixedUpdate wsfixMoveDelay = new WaitForFixedUpdate();
            Vector2 origin = selfGameObject.transform.position;
            Vector2 distance = origin + (direction * repeatDistance);
        
            whileState = false;
            float gap = 0.1f;
            gap = gap * moveSpeed > repeatDistance ? 0.5f : gap * moveSpeed;
            if(repeatCount == 0) whileState = true;

            while(repeatCount-- > 0 || whileState)
            {
                while(Vector3.Distance(selfGameObject.transform.position,distance) > gap)
                {
                    //print(selfGameObject.transform.position + " " + distance + " " + origin + "\n" + (direction));
                    MovePos(selfGameObject.transform,direction.x,direction.y,0,moveSpeed);
                    yield return wsfixMoveDelay;
                }
                selfGameObject.transform.position = distance;
                yield return wsStopTime;
                while(Vector3.Distance(selfGameObject.transform.position,origin) > gap)
                {
                    MovePos(selfGameObject.transform,-direction.x,-direction.y,0,moveSpeed);
                    yield return wsfixMoveDelay;
                }
                selfGameObject.transform.position = origin;
                yield return wsStopTime;
            }
        }
        /// <summary>
        /// 매개변수로 받은 오브젝트를 repeatDistance만큼 반복이동 시킨다. repeatCount = 0일 경우 무한반복한다.
        /// </summary>
        public IEnumerator ObjectRepeatMove2D(Rigidbody2D selfRigidbody, Vector2 direction, float repeatDistance, float moveSpeed = 1f, int repeatCount = 0, float stopTime = 0.5f)
        {
            WaitForSeconds wsStopTime = new WaitForSeconds(stopTime);
            WaitForFixedUpdate wsfixMoveDelay = new WaitForFixedUpdate();
            Vector2 origin = selfRigidbody.position;
            Vector2 distance = origin + (direction * repeatDistance);
        
            whileState = false;
            float gap = 0.1f;
            gap = gap * moveSpeed > repeatDistance ? 0.5f : gap * moveSpeed;
            if(repeatCount == 0) whileState = true;

            while(repeatCount-- > 0 || whileState)
            {
                while(Vector3.Distance(selfRigidbody.position,distance) > gap)
                {
                    MovePos(selfRigidbody,direction,moveSpeed);
                    yield return wsfixMoveDelay;
                }
                selfRigidbody.position = distance;
                yield return wsStopTime;
                while(Vector3.Distance(selfRigidbody.position,origin) > gap)
                {
                    MovePos(selfRigidbody,-direction,moveSpeed);
                    yield return wsfixMoveDelay;
                }
                selfRigidbody.position = origin;
                yield return wsStopTime;
            }
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
                while (whileState && i < targetsLen)
                {
                    destination = targets[i].transform.position;
                    distanceToMove = destination - selfRigidbody.position;

                    while (whileState && !(Vector2.Distance(destination, selfRigidbody.position) < limit && Vector2.Distance(destination, selfRigidbody.position) > -limit))
                    {
                        if(destination != (Vector2)targets[i].transform.position)
                        {
                            destination = targets[i].transform.position;
                            distanceToMove = destination - selfRigidbody.position;
                            //print(distanceToMove);
                        }
                        speed += acceleration;
                        Move(selfRigidbody, distanceToMove, speed,false);
                        yield return null;
                    }
                    selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                    selfRigidbody.velocity = Vector2.zero;
                    i++;
                }
            }
            yield return null;
        }
        public IEnumerator ObjectTargetsTraceMovePos2D(Rigidbody2D selfRigidbody, GameObject[] targets, int targetsLen, float speed = 1, float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            int i = 0;
            Vector2 destination = Vector2.zero;
            Vector2 distanceToMove = Vector2.zero;
            if (targets.Length > 0)
            {
                while (whileState && i < targetsLen)
                {
                    print("len : " + targetsLen + " target : " + targets[i].name);
                    destination = targets[i].transform.position;
                    distanceToMove = destination - selfRigidbody.position;
                    print(distanceToMove);

                    while (whileState && Vector2.Distance(destination, selfRigidbody.position) > limit)
                    {
                        if(destination != (Vector2)targets[i].transform.position)
                        {
                            destination = targets[i].transform.position;
                            distanceToMove = destination - selfRigidbody.position;
                            print(distanceToMove);
                        }
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
        public IEnumerator ObjectTargetTraceMove2D(Rigidbody2D selfRigidbody, GameObject target, float speed = 1, float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            Vector2 destination = Vector2.zero;
            Vector2 distanceToMove = Vector2.zero;
            if (target != null)
            {
                do
                {
                    if(destination != (Vector2)target.transform.position)
                    {
                        destination = target.transform.position;
                        distanceToMove = destination - selfRigidbody.position;
                        //print(distanceToMove);
                    }
                    speed += acceleration;
                    Move(selfRigidbody, distanceToMove, speed,false);
                    yield return null;
                }while(whileState && !(Vector2.Distance(destination, selfRigidbody.position) < limit && Vector2.Distance(destination, selfRigidbody.position) > -limit));
                selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                selfRigidbody.velocity = Vector2.zero;
            }
            yield return null;
        }
        public IEnumerator ObjectTargetTraceMovePos2D(Rigidbody2D selfRigidbody, GameObject target, float speed = 1, bool phsicsState = false,float acceleration = 0)
        {
            float limit = 0.05f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            Vector2 destination = Vector2.zero;
            Vector2 distanceToMove = Vector2.zero;
            if (target != null)
            {
                do
                {
                    if(destination != (Vector2)target.transform.position)
                    {
                        destination = target.transform.position;
                        distanceToMove = destination - selfRigidbody.position;
                        //print(distanceToMove);
                    }
                    speed += acceleration;
                    MovePos(selfRigidbody, distanceToMove, speed,phsicsState);
                    yield return null;
                }while(whileState && !(Vector2.Distance(destination, selfRigidbody.position) < limit && Vector2.Distance(destination, selfRigidbody.position) > -limit));
                selfRigidbody.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                selfRigidbody.velocity = Vector2.zero;
            }
            yield return null;
        }

    }
}
