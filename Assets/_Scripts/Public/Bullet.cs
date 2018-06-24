using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : ObjectInteraction
{
    public IEnumerator BulletDirectionShot2D(Rigidbody2D rigidbody2D ,Vector2 direction, float extinctionTime, float speed = 1f)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;

        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if (extinctionTime > 0)
        {
            /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
            if(currentTime - startedTime < extinctionTime)
            {
                MovePos(rigidbody2D, direction, speed);

                yield return new WaitForEndOfFrame();
                currentTime = Time.time;
            }
        }
        else if(extinctionTime == 0)
        {
            MovePos(rigidbody2D, direction, speed);

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator BulletTargetShot2D(Rigidbody2D rigidbody2D, Vector2 direction, float extinctionTime, float speed = 1f)
    {
        float startedTime = Time.time;
        float currentTime = startedTime;

        /* extinctionTime이 0일 경우 충돌이외에는 사라지지 않는다. */
        if (extinctionTime > 0)
        {
            /* 시작 시간과 현재 시간을 뺀 시간이 소멸시간보다 작을 경우 계속 이동 */
            if (currentTime - startedTime < extinctionTime)
            {
                MovePos(rigidbody2D, direction, speed);

                yield return new WaitForEndOfFrame();
                currentTime = Time.time;
            }
        }
        else if (extinctionTime == 0)
        {
            MovePos(rigidbody2D, direction, speed);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }

    void ExitBullet()
    {
        StopAllCoroutines();
    }
}
