using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Shoter : ObjectInteraction
{

    [Header("발사 타입 설정")]

    [Tooltip("0 - 자기자신 날리기\n1 - 맵에 보이면 방향지정 날리기 \n2 - 충돌 이벤트 발생 \n3 - 회전")]
    [Range(0,3)]
    public int selectedType = 0;
    [Tooltip("0, 2 번 선택시 0 - 방향날리기 1 - 타겟 충돌 위치에 날리기 \n3번 선택시 0 - 화면에 보이자 마자 1 - 트리거에 걸렸을때")]
    [Range(0, 1)]
    public int shotKey = 0;
    [Tooltip("3 번 선택시 0 - 순차 회전 발사 1 - 동시 회전 발사")]
    [Range(0, 1)]
    public int shotRotationKey = 0;

    [Header("공통 변수")]
    [Tooltip("발사 포인트가 없을 경우 shoter 중심지가 지정됨")]
    public GameObject shotPoint = null;
    [Tooltip("총알이 인식할 대상을 선택. 선택안하면 모두 선택")]
    public GameObject target = null;
    [Tooltip("다중 타겟인식시 개수를 입력하고 tag를 입력.")]
    [SerializeField]
    public List<string> collisionTagName = null;
    [Tooltip("날릴 각도를 지정해 주세요.\n0 - right\n90 - up\n180 - left\n270 - down")]
    public int shootAngle = 0;
    [Range(0,10)]
    public float bulletSpeed = 1;
    [Range(0, 5)]
    [Tooltip("0 is that not remove -> collision destroy")]
    public float bulletRemoveTime = 0;
    [Range(0, 20)]
    [Tooltip("화면에 보일경우 첫 발사를 delay 시킬수 있는 인수입니다. default = 0")]
    public float delayTime = 0;

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
    [Range(0, 36)]
    [Tooltip("회전총알 발사시 발사할 개수 - shotAngle / 개수 = 각도")]
    public int bulletRotationCount = 18;
    [Tooltip("총알이 날라갈 최대 각도입니다. 반시계 방향을 원하면 음수로 주세요")]
    public int shotMaxAngle = 360;
    [Tooltip("발사를 시작할 각도 입니다.\n0 - right\n90 - up\n180 - left\n270 - down")]
    public int shotStartAngle = 0;
    [Range(0, 1)]
    [Tooltip("3_0 선택 : 총알이 날라가는 시간 gap. default = 0.05f")]
    public float shotWaitRotation = 0.05f;
    [Range(0, 10)]
    [Tooltip("매번 날라갈 때마다 각도를 조금씩 비틉니다.. default = 1")]
    public int shotRotateAngle = 1;
    [Tooltip("발사체 리터닝선택지 입니다. ")]
    public bool shoterReturnTrigger = false;
    [Tooltip("발사체 이동선택지 입니다.")]
    public bool shoterMoveTrigger = false;
    [Range(0, 10)]
    [Tooltip("shoterMoveTrigger 선택 : 발사체 이동 거리입니다. default = 0")]
    public float shoterMoveDistance = 0;
    [Range(0, 10)]
    [Tooltip("shoterMoveTrigger 선택 : 발사체 이동 속도입니다. default = 0")]
    public float shoterMoveSpeed = 0;

    /* needs inner variable */
    Rigidbody2D rg2d = null;
    public delegate void ShootDelegate(Vector2 bulletStartingPoint, Vector2 dir);
    private struct BulletStruct
    {
        public GameObject obj;
        public Bullet btScript;
        public ShootDelegate ShootEvent;
    }
    List<BulletStruct> bullets;
    Vector2 bulletStartingPoint;
    Vector2 CollisionTargetDirection = Vector2.zero;
    public delegate bool BulletDelegate();
    public event BulletDelegate LoadInitBullet; //현재 사용되는 총알을 모두 비활성화 해야할 경우만 사용
    bool shotState = false; //화면에 shoter가 안보일경우 반복문 중지를 위해 사용
    int projectTileMutiple = 0; //회전 총알에서 생성할때 여유를 주기위해 정해진 생성 개수에 곱해줄 인수.

    /* save 변수 */
    Vector2 saveStartingPoint;
    bool saveMoveTrigger;

    private void Start()
    {
        bullets = new List<BulletStruct>();
        /* 지정된 발사포인트가 없을 경우 shoter의 중심지를 발사포인트로 지정한다. */
        if (shotPoint == null) bulletStartingPoint = transform.position;
        else bulletStartingPoint = shotPoint.transform.position;

        /* target만 인식하게 선택했을 경우 target의 태그를 collisionTagName에 추가한다. */
        /* 추가한 태그가 없다면 collisionTagName.Length = 0 이므로 인덱스 0에 추가될 것이다. */

        if (target != null) collisionTagName.Add(target.tag);

        rg2d = GetComponent<Rigidbody2D>();

        switch(selectedType)
        {
            case 1:
            case 2:
                CreateBullet(bulletReadyCount); //총알 생성
                break;
            case 3:
                projectTileMutiple = (int)(bulletRemoveTime / shotTimeGap) + 1; //사라지는 시간 대비 쏘는 시간을 계산하여 총 총알 개수를 만들기 위한 곱 정수
                CreateBullet(bulletRotationCount * projectTileMutiple); //총알 생성. 90도에서 180도를 쏠때 + 1을 안해주면 마지막 각도에서는 총알이 안나옴. ex) 1과 2는 총 2개의 숫자지만 2 - 1= 1임
                break;
        }
        SaveState(shotState, gameObject.activeSelf, transform.position);
    }

    //화면에 보일때 실행되는 구문들
    private void OnBecameVisible()
    {
        LoadState();
        shotState = true;
        StartCoroutine(ShotStartFunc());
    }
    //화면에 안보일때 실행되는 구문들
    private void OnBecameInvisible()
    {
        shotState = false;

        if (selectedType == 0 || selectedType == 1 || (selectedType * shotKey == 0))
        {
            StopAllCoroutines();
        }
        else if((CollisionTargetTransform != null))
        {
            print("shoter.cs - 화면에 안보임" + bullets.Count);
            StopAllCoroutines();
        }

    }
    /// <summary>
    /// shot 타입을 통해 shoter가 총을 쏘게 도와주는 코루틴
    /// </summary>
    IEnumerator ShotStartFunc()
    {
        yield return new WaitForSeconds(delayTime);//delay를 줘서 각 shoter마다 발사시작을 다르게 할 수 있다.

        switch (selectedType)
        {
            case 0: //shoter 가 총알. 충돌인식과 바로 발사 형식이 존재
                if (rg2d != null)
                {
                    if (shotKey == 0)
                    {
                        Vector2 dirVector = AngleToVector2(shootAngle);
                        StartCoroutine( BulletShot2D(rg2d, dirVector, bulletRemoveTime, bulletSpeed));
                        // 충돌을 감지하는 코루틴을 실행합니다.
                        print("Shoter.cs - 자기자신을 총알로 사용합니다. 방향 : " + dirVector);
                    }
                    else
                    {
                        /* SetCollisionTargetDirection 함수를 통해 충돌체 위치를 향한 방향을 받아오지 않으면 zero 값을 가지기에 계속 탐지를 기다린다 */
                        while (CollisionTargetDirection == Vector2.zero)
                        {
                            print("Shoter.cs - Target 탐지중");
                            yield return new WaitForFixedUpdate();
                        }
                        StartCoroutine(BulletShot2D(rg2d, CollisionTargetDirection, bulletRemoveTime, bulletSpeed));
                        // 충돌을 감지하는 코루틴을 실행합니다.
                    }
                }
                else
                {
                    Debug.Log("Shoter.cs - 해당 오브젝트에 Rigidbody 가 존재하지 않습니다.");
                }
                break;
            case 1: // 화면 인식시 총알 발사 시작
                    //bullets = new GameObject[bulletReadyCount];
                if (bullet != null)
                {

                    Vector2 dirVector = AngleToVector2(shootAngle);

                    do //화면에 shoter가 안보이면 비활성화 되고 반복문 종료
                    {
                        for (int i = 0; (i < bulletReadyCount) && shotState; i++)
                        {
                            bullets[i].ShootEvent(bulletStartingPoint, dirVector);
                            yield return new WaitForSeconds(shotTimeGap);
                        }
                        yield return new WaitForFixedUpdate(); ;//동작 오류가 나더라도 게임이 멈추지 않도록 - 무한반복이 돌면 안되기에
                    } while (shotState && shotInfinityTrigger); //화면에 shoter가 안보이면 비활성화 되고 반복문 종료
                }
                else
                {
                    Debug.Log("Shoter.cs - 지정된 총알이 없습니다. plz reload");
                }
                break;
            case 2:
                if (bullet != null)
                {
                    while (shotState)
                    {
                        /* SetCollisionTargetDirection 함수를 통해 충돌체 위치를 향한 방향을 받아오지 않으면 zero 값을 가지기에 계속 탐지를 기다린다 */
                        while (CollisionTargetDirection == Vector2.zero)
                        {
                            yield return new WaitForFixedUpdate();
                        }
                        if (shotKey == 0)
                        {
                            Vector2 dirVector = AngleToVector2(shootAngle);
                            do
                            {
                                for (int i = 0; (i < bulletReadyCount) && shotState; i++)
                                {
                                    bullets[i].ShootEvent(bulletStartingPoint, dirVector);
                                    yield return new WaitForSeconds(shotTimeGap);
                                }
                            } while (shotState && shotInfinityTrigger); //화면에 shoter가 안보이면 비활성화 되고 반복문 종료
                            CollisionTargetDirection = Vector2.zero; //타겟 위치를 초기화 하여 다음 이벤트를 기다리게 만든다.
                        }
                        else
                        {
                            do //화면에 shoter가 안보이면 비활성화 되고 반복문 종료
                            {
                                for (int i = 0; (i < bulletReadyCount) && shotState; i++)
                                {
                                    bullets[i].ShootEvent(bulletStartingPoint ,CollisionTargetDirection);
                                    yield return new WaitForSeconds(shotTimeGap);
                                }
                            } while (shotState && shotInfinityTrigger);
                            CollisionTargetDirection = Vector2.zero; //타겟 위치를 초기화 하여 다음 이벤트를 기다리게 만든다.
                        }

                        yield return new WaitForFixedUpdate();//동작 오류가 나더라도 게임이 멈추지 않도록 - 무한반복이 돌면 안되기에

                    }
                }
                else
                {
                    Debug.Log("Shoter.cs - 지정된 총알이 없습니다. plz reload");
                }

                break;
            case 3:
                if (bullet != null)
                {
                    /* 회전 발사에 필요한 변수 선언 */
                    int angle = shotMaxAngle / bulletRotationCount; //발사시 이동각도 설정
                    int currentAngle = shotStartAngle; //시작 각도 및 발사시 현재 각도
                    Vector2 angleDirection; // 각도를 벡터값으로 표현할때 저장할 벡터
                    Vector2 dirVector;

                    while (shotState)
                    {
                        /* shotkey 가 0 이면 화면 인식 후, 1이면 트리거를 통한 발사 */
                        /* dirVector를 사용하여 간단하게 구현하게 만든다. */
                        if(shotKey == 1)
                        {
                            /* SetCollisionTargetDirection 함수를 통해 충돌체 위치를 향한 방향을 받아오지 않으면 zero 값을 가지기에 계속 탐지를 기다린다 */
                            while (CollisionTargetDirection == Vector2.zero) yield return new WaitForFixedUpdate();
                            dirVector = CollisionTargetDirection;
                        }
                        /* 한번만 옮기면 되므로 shoterMoveTrigger = false 로 꺼준다. */
                        if (shoterMoveTrigger)
                        {
                            dirVector = AngleToVector2(shootAngle);
                            print("shoter.cs - 회전발사대 이동");
                            //rg2d.position+(dirVector * shoterMoveDistance) 를 통해 해당 목적지 좌표를 표시한다.
                            StartCoroutine(MoveToDestination(rg2d, rg2d.position + (dirVector * shoterMoveDistance), shoterMoveSpeed));
                            shoterMoveTrigger = false;
                        }

                        int currentShotCount = 0; //생성된 총알이 발사된 총알보다 많으므로 인덱스를 맞추기 위한 것
                        int cnt = 1; //각도 비틀때 사용하는 카운터
                        if (shoterReturnTrigger) //리터닝 총
                        {
                            if (shotRotationKey == 0) //순차 회전 발사
                            {
                                do
                                {
                                    currentAngle = shotStartAngle;
                                    for (int i = 0 + currentShotCount; i < bulletRotationCount + currentShotCount && shotState; i++)
                                    {
                                        angleDirection = AngleToVector2(currentAngle);
                                        bullets[i].btScript.SetReturnBullet(); //시간초가 다 되도 그 자리에서 멈추도록 한다. 충돌시에는 제자리로 돌아온다.
                                        bullets[i].btScript.Shoot(transform.position, angleDirection);
                                        currentAngle += angle; //shot 각도를 돌린다.
                                        yield return new WaitForSeconds(shotWaitRotation);
                                    }

                                    if (currentShotCount != (bulletRotationCount * (projectTileMutiple - 1)))
                                    {
                                        currentShotCount += bulletRotationCount;
                                        currentAngle = shotStartAngle + shotRotateAngle * cnt++;
                                        yield return new WaitForSeconds(bulletRemoveTime);
                                        continue;
                                    }
                                    else
                                    {
                                        currentShotCount = 0;
                                        yield return new WaitForSeconds(bulletRemoveTime);
                                        while (currentShotCount != (bulletRotationCount * (projectTileMutiple)))
                                        {
                                            for (int i = 0 + currentShotCount; i < bulletRotationCount + currentShotCount && shotState; i++)
                                            {
                                                bullets[i].btScript.Shoot();
                                                yield return new WaitForSeconds(shotWaitRotation);
                                                print("실행");
                                            }
                                            currentShotCount += bulletRotationCount;
                                        }
                                        /* 다시 초기 설정부터 쏘기위한 설정부분 */
                                        currentShotCount = 0;
                                        currentAngle = shotStartAngle;
                                        cnt = 1;
                                    }
                                    yield return new WaitForSeconds(bulletRemoveTime);
                                } while (shotState && shotInfinityTrigger);

                            }
                            else if (shotRotationKey == 1) // 동시 회전 발사
                            {
                                do
                                {
                                    for (int i = 0 + currentShotCount; i < bulletRotationCount + currentShotCount && shotState; i++)
                                    {
                                        angleDirection = AngleToVector2(currentAngle);
                                        bullets[i].btScript.SetReturnBullet(); //시간초가 다 되도 그 자리에서 멈추도록 한다. 충돌시에는 제자리로 돌아온다.
                                        bullets[i].btScript.Shoot(transform.position, angleDirection);
                                        currentAngle += angle; //shot 각도를 돌린다.
                                        //yield return new WaitForFixedUpdate();
                                    }
                                    if (currentShotCount != (bulletRotationCount * (projectTileMutiple - 1)))
                                    {
                                        currentShotCount += bulletRotationCount;
                                        currentAngle = shotStartAngle + shotRotateAngle * cnt++;
                                        yield return new WaitForSeconds(shotTimeGap);
                                        continue;
                                    }
                                    else
                                    {
                                        currentShotCount = 0;
                                        yield return new WaitForSeconds(bulletRemoveTime);
                                        while (currentShotCount != (bulletRotationCount * (projectTileMutiple)))
                                        {
                                            for (int i = 0 + currentShotCount; i < bulletRotationCount + currentShotCount && shotState; i++)
                                            {
                                                bullets[i].btScript.Shoot();
                                                //yield return new WaitForFixedUpdate();
                                            }
                                            currentShotCount += bulletRotationCount;
                                            yield return new WaitForSeconds(shotTimeGap);
                                        }
                                        /* 다시 초기 설정부터 쏘기위한 설정부분 */
                                        currentShotCount = 0;
                                        currentAngle = shotStartAngle;
                                        cnt = 1;
                                    }
                                    yield return new WaitForSeconds(bulletRemoveTime);
                                } while (shotState && shotInfinityTrigger);
                            }
                        }
                        else
                        {
                            if (shotRotationKey == 0) //순차 회전 발사
                            {
                                do
                                {
                                    currentAngle = shotStartAngle;
                                    for (int i = 0 + currentShotCount; i < bulletRotationCount + currentShotCount && shotState; i++)
                                    {
                                        angleDirection = AngleToVector2(currentAngle);
                                        bullets[i].ShootEvent(transform.position, angleDirection);
                                        currentAngle += angle; //shot 각도를 돌린다.
                                        yield return new WaitForSeconds(shotWaitRotation);
                                    }
                                    if (currentShotCount != (bulletRotationCount * (projectTileMutiple - 1)))
                                    {
                                        currentShotCount += bulletRotationCount;
                                        currentAngle = shotStartAngle + shotRotateAngle * cnt++;
                                        yield return new WaitForSeconds(bulletRemoveTime);
                                        continue;
                                    }
                                    else
                                    {
                                        /* 다시 초기 설정부터 쏘기위한 설정부분 */
                                        currentShotCount = 0;
                                        currentAngle = shotStartAngle;
                                        cnt = 1;
                                    }
                                    yield return new WaitForSeconds(bulletRemoveTime);
                                }while (shotState && shotInfinityTrigger) ;
                                
                            }
                            else if (shotRotationKey == 1) // 동시 회전 발사
                            {
                                do
                                {
                                    currentAngle = shotStartAngle;
                                    for (int i = 0 + currentShotCount; i < bulletRotationCount + currentShotCount && shotState; i++)
                                    {
                                        angleDirection = AngleToVector2(currentAngle);
                                        bullets[i].ShootEvent(transform.position, angleDirection);
                                        currentAngle += angle; //shot 각도를 돌린다.
                                        //yield return new WaitForFixedUpdate();
                                    }
                                    if (currentShotCount != (bulletRotationCount * (projectTileMutiple - 1)))
                                    {
                                        currentShotCount += bulletRotationCount;
                                        currentAngle = shotStartAngle + shotRotateAngle * cnt++;
                                        yield return new WaitForSeconds(shotTimeGap);
                                        continue;
                                    }
                                    else
                                    {
                                        /* 다시 초기 설정부터 쏘기위한 설정부분 */
                                        currentShotCount = 0;
                                        currentAngle = shotStartAngle;
                                        cnt = 1;
                                    }
                                    yield return new WaitForSeconds(bulletRemoveTime);
                                } while (shotState && shotInfinityTrigger);

                            }
                        }
                        yield return new WaitForFixedUpdate();//동작 오류가 나더라도 게임이 멈추지 않도록 - 무한반복이 돌면 안되기에
                    }
                }
                else
                {
                    Debug.Log("Shoter.cs - 지정된 총알이 없습니다. plz reload");
                }
                break;
            default:
                break;
        }
        yield break;
    }

    /// <summary>
    /// Shoter.cs 종속함수, 발사할 총알을 생성한다. 
    /// </summary>
    void CreateBullet(int projectTileCount)
    {
        BulletStruct tempBs ;
        /* BulletStruct라는 구조체를 사용하여 총알 오브젝트와 그 오브젝트 컴포넌트인 Bullet을 저장 */
        for (int i = 0; i < projectTileCount; i++)
        {
            tempBs.obj = Instantiate(bullet, transform) as GameObject;
            //tempBs.obj.SetActive(false); //생성시 총알이 보이면 안되므로 비활성화 시켜준다.
            if((tempBs.btScript = tempBs.obj.GetComponent<Bullet>()) != null)
            {
                tempBs.btScript.InitBaseProperty(bulletStartingPoint, bulletSpeed, collisionTagName, bulletRemoveTime);
                tempBs.btScript.GetBulletComponent();
                tempBs.ShootEvent = tempBs.btScript.Shoot;
                LoadInitBullet += tempBs.btScript.LoadState; // 총알을 한번에 없애기 위해서 이벤트 변수에 종료 함수를 넣어줍니다.
            }
            else{
                var script = tempBs.obj.GetComponent<BounceObject>();
                script.InitBaseProperty(bulletStartingPoint, bulletSpeed, collisionTagName, bulletRemoveTime);
                script.GetBulletComponent();
                tempBs.btScript = null;
                tempBs.ShootEvent = script.Shoot;
            }
            bullets.Add(tempBs);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (collisionTagName.Count == 0)
        {
            print("shoter.cs - 모두 충돌 발생 : selectedType");
            LifeInteraction interaction = col.gameObject.GetComponent<LifeInteraction>();
            if (interaction != null) interaction.TakeHit(1);
            transform.position = bulletStartingPoint;
            gameObject.SetActive(false);
        }
        else if (selectedType == 0 || selectedType == 3)
        {
            foreach (var v in collisionTagName)
            {
                if (col.CompareTag(v))
                {
                    LifeInteraction interaction = col.gameObject.GetComponent<LifeInteraction>();
                    print("shoter.cs - 충돌 발생 : selectedType");
                    if (selectedType == 0)
                    {
                        print("shoter.cs - shotkey : 0");
                        if (interaction != null) interaction.TakeHit(1);
                        transform.position = bulletStartingPoint;
                        gameObject.SetActive(false);
                    }
                    else if (shoterMoveTrigger)
                    {
                        if(interaction != null) interaction.TakeHit(1);
                    }
                }
            }
        }
    }
    public override void SetCollisionTarget(Transform tf)
    {
        base.SetCollisionTarget(tf);
        CollisionTargetDirection = - (transform.position - CollisionTargetTransform.position).normalized;
    }

    /* 세이브 함수들 */
    public override void SaveState(bool selfState, bool selfActive, Vector2 pos)
    {
        base.SaveState(selfState, selfActive, pos);
        saveStartingPoint = bulletStartingPoint;
        saveMoveTrigger = shoterMoveTrigger;
    }
    public override bool LoadState()
    {
        shoterMoveTrigger = saveMoveTrigger;
        bulletStartingPoint = saveStartingPoint;
        LoadInitBullet();
        return base.LoadState();
    }
}
