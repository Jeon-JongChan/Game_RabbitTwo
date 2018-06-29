using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoter : ObjectInteraction
{

    [Header("타입 설정")]

    [Tooltip("0 - 자기자신 날리기\n1 - 맵에 보이면 방향지정 날리기 \n2 - 충돌 이벤트 발생 \n3 - 회전")]
    [Range(0,3)]
    public int selectedType = 0;
    [Tooltip("0, 2 번 선택시 0 - 방향날리기 1 - 타겟 충돌 위치에 날리기")]
    [Range(0, 1)]
    public int selfShotKey = 1;

    [Header("공통 변수")]
    [Tooltip("발사 포인트가 없을 경우 shoter 중심지가 지정됨")]
    public GameObject shotPoint = null;
    [Tooltip("총알이 인식할 대상을 선택 \n-> 안하면 모든 오브젝트를 인식")]
    public GameObject target = null;
    [Tooltip("해당 타겟만 인식시키고 싶을 경우 선택한다.")]
    public bool onlyTargetDetect = false;
    [Tooltip("-1 - 타겟지정 \n0 - UP \n 1 - Right\n 2 - Down\n 3- Left")]
    [Range(-1,3)]
    public int shotDir = -1;
    [Range(0,10)]
    public float bulletSpeed = 1;
    [Range(0, 10)]
    [Tooltip("0 is that not remove -> collision destroy")]
    public float bulletRemoveTime = 0;
    [Range(0, 5)]
    [Tooltip("총알이 충돌체를 인식하는 범위를 나타냅니다. default = 1")]
    public float rayScale = 1f;

    [Header("selectType - 1 or 2 or 3")]
    public GameObject bullet = null;
    [Tooltip("총알을 한발만 발싸할지 무한대로 발사할지")]
    public bool shotInfinityTrigger = true;
    [Range(0,10)]
    public float shotTimeGap = 1f;
    [Range(0, 100)]
    [Tooltip("default 10, 오브젝트마다 생성할 총알 개수")]
    public int bulletReadyCount = 10;

    [Header("selectType - 3")]
    [Range(0, 180)]
    [Tooltip("회전총알 발사시 발사할 개수")]
    public int bulletRotationCount = 36;

    /* needs inner variable */
    private struct BulletStruct
    {
        public GameObject obj;
        public Bullet btScript;
    }
    List<BulletStruct> bullets;
    Vector2 bulletStartingPoint;
    Vector2 CollisionTargetDirection = Vector2.zero; //Trigger 객체에서 전달하는 충돌체의 위치 포인트를 가리키는 방향벡터
    public delegate void BulletDelegate();
    private static event BulletDelegate ExitBullet; //현재 사용되는 총알을 모두 비활성화 해야할 경우만 사용
    bool shotState = false; //화면에 shoter가 안보일경우 반복문 중지를 위해 사용
    string targetTag = null; // 타겟만 충돌시키고 싶을때 태그가 추가되는 스트링 문.
    int bulletLayermask = 0; // 충돌에 제외할 레이어가 있는 경우 사용할 것.

    private void Start()
    {
        bullets = new List<BulletStruct>();
        /* 지정된 발사포인트가 없을 경우 shoter의 중심지를 발사포인트로 지정한다. */
        if (shotPoint == null) bulletStartingPoint = transform.position;
        else bulletStartingPoint = shotPoint.transform.position;

        /* target만 인식하게 선택했을 경우 target의 태그를 입력한다. */
        if (!onlyTargetDetect && target != null) targetTag = target.tag;

        //if(target != null) bulletLayermask = 1 << target.layer;
    }
    //화면에 보일때 실행되는 구문들
    private void OnBecameVisible()
    {
        StartCoroutine(ShotStartFunc());
    }
    //화면에 안보일때 실행되는 구문들
    private void OnBecameInvisible()
    {
        shotState = false;

        if (selectedType == 0)
        {
            print("화면에 안보임");
            gameObject.SetActive(false);
            StopAllCoroutines();
        }

    }
    /// <summary>
    /// shot 타입을 통해 shoter가 총을 쏘게 도와주는 코루틴
    /// </summary>
    IEnumerator ShotStartFunc()
    {
        shotState = true;

        switch (selectedType)
        {
            case 0: //shoter 가 총알. 충돌인식과 바로 발사 형식이 존재
                Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
                if (rigidbody != null)
                {
                    if (selfShotKey == 0)
                    {
                        Vector2 dirVector = CalclulateTheDirection(shotDir);
                        StartCoroutine( BulletShot2D(rigidbody ,dirVector, bulletRemoveTime, bulletSpeed, bulletLayermask, rayScale));
                        print("Shoter.cs - 자기자신을 총알로 사용합니다. 방향 : " + dirVector);
                    }
                    else
                    {
                        /* SetCollisionTargetDirection 함수를 통해 충돌체 위치를 향한 방향을 받아오지 않으면 zero 값을 가지기에 계속 탐지를 기다린다 */
                        while (CollisionTargetDirection == Vector2.zero)
                        {
                            print("Shoter.cs - Target 탐지중");
                            yield return new WaitForEndOfFrame();
                        }
                        StartCoroutine(BulletShot2D(rigidbody, CollisionTargetDirection, bulletRemoveTime, bulletSpeed, bulletLayermask, rayScale));
                    }
                }
                else
                {
                    Debug.Log("Shoter.cs - 해당 오브젝트에 Rigidbody 가 존재하지 않습니다.");
                }
                break;
            case 1: // 화면 인식시 총알 발사 시작
                //bullets = new GameObject[bulletReadyCount];
                if(bullet != null)
                {
                    CreateBullet(bulletLayermask); //총알 생성

                    Vector2 dirVector = CalclulateTheDirection(shotDir);

                    if (shotInfinityTrigger == false)
                    {
                        for(int i = 0; i < bulletReadyCount; i++)
                        {
                            bullets[i].btScript.Shoot(dirVector);
                            yield return new WaitForSeconds(shotTimeGap);
                        }
                    }
                    else
                    {
                        while(shotState) //화면에 shoter가 안보이면 비활성화 되고 반복문 종료
                        {
                            for (int i = 0; (i < bulletReadyCount) && shotState ; i++)
                            {
                                bullets[i].btScript.Shoot(dirVector);
                                yield return new WaitForSeconds(shotTimeGap);
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Shoter.cs - 지정된 총알이 없습니다. plz reload");
                }
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
        yield break;
    }
    /// <summary>
    /// 정수형 direction을 받아 정해진 규칙을 통해 방향을 가리키는 벡터를 반환한다.
    /// </summary>
    /// <param name="direction">-1 - 타겟지정 \n0 - UP \n 1 - Right\n 2 - Down\n 3- Left</param>
    Vector2 CalclulateTheDirection(int direction)
    {
        Vector2 ret;
        switch (direction)
        {
            case 0:
                ret = Vector2.up;
                break;
            case 1:
                ret = Vector2.right;
                break;
            case 2:
                ret = Vector2.down;
                break;
            case 3:
                ret = Vector2.left;
                break;
            default:
                ret = Vector2.zero;
                print("Shoter.cs - 방향설정 오류");
                break;
        }

        return ret;
    }
    /// <summary>
    /// ShotTrigger를 통해 목표의 위치를 갖고 오는 함수
    /// </summary>
    public void SetCollisionTargetDirection(Transform tf)
    {
        CollisionTargetDirection = -(transform.position - tf.position).normalized;
    }
    /// <summary>
    /// Shoter.cs 종속함수, 발사할 총알을 생성한다.
    /// </summary>
    void CreateBullet(int bulletLayermask)
    {
        BulletStruct tempBs;
        /* BulletStruct라는 구조체를 사용하여 총알 오브젝트와 그 오브젝트 컴포넌트인 Bullet을 저장 */
        for (int i = 0; i < bulletReadyCount; i++)
        {
            tempBs.obj = Instantiate(bullet, transform) as GameObject;
            tempBs.obj.SetActive(false); //생성시 총알이 보이면 안되므로 비활성화 시켜준다.
            tempBs.btScript = tempBs.obj.GetComponent<Bullet>();

            tempBs.btScript.InitBaseProperty(bulletStartingPoint, bulletSpeed, bulletLayermask, bulletRemoveTime, rayScale);
            tempBs.btScript.GetRigidbodyComponent();

            ExitBullet += tempBs.btScript.ExitBullet; // 총알을 한번에 없애기 위해서 이벤트 변수에 종료 함수를 넣어줍니다.
            bullets.Add(tempBs);
        }
    }

    IEnumerator SelfShotDetectObject()
    {
        while (detectState) yield return new WaitForEndOfFrame();
    }

}
