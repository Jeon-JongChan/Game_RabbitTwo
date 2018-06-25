using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoter : ObjectControl
{

    [Header("타입 설정")]
    [Tooltip("0 - 자기자신 날리기\n1 - 총알 정한 방향으로 날리기 \n2 - 총알 타겟 향해 날리기(충돌체 배치 필요) \n3 - 회전총알 날리기")]
    public int selectType = 0;

    [Header("공통 변수")]
    public GameObject bullet;
    [Tooltip("-1 - 타겟지정 \n0 - UP \n 1 - Right\n 2 - Down\n 3- Left")]
    public int dir = -1;
    [Range(0,10)]
    public float bulletSpeed = 1;
    [Range(0, 10)]
    [Tooltip("0 is that not remove -> collision destroy")]
    public float removeBulletTime = 0;

    [Header("selectType - 1 or 2 or 3")]
    [Tooltip("총알을 한발만 발싸할지 무한대로 발사할지")]
    public bool infinityTrigger = true;
    [Range(0,10)]
    public float shotTimeGap = 1f;
    [Range(0, 100)]
    [Tooltip("default 10, 오브젝트마다 생성할 총알 개수")]
    public int bulletReadyCount = 10;

    [Header("selectType - 3")]
    [Range(0, 180)]
    [Tooltip("회전총알 발사시 발사할 개수")]
    public int rotationBulletCount = 36;

    /* needs inner variable */
    GameObject[] bullets;
    Vector2 startingBulletPoint;

    private void Start()
    {
        startingBulletPoint = transform.position;
        print(transform.parent);

    }

    IEnumerator ShotOneself(Rigidbody2D oneself,int direction, float bulletSpeed,float removeBulletTime)
    {
        Vector2 dir = new Vector2(0,0);
        switch (direction)
        {
            case 0:
                dir = Vector2.up;
                break;
            case 1:
                dir = Vector2.right;
                break;
            case 2:
                dir = Vector2.down;
                break;
            case 3:
                dir = Vector2.left;
                break;
            default:
                print("방향설정 오류");
                break;
        }
        MovePos(oneself, dir, bulletSpeed);

        yield return new WaitForSeconds(removeBulletTime);

    }
}
