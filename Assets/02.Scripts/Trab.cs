using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trab : ObjectInteraction {

    /* inspector variable */
    public int shootAngle = 0;
    public float distance = 1f;
    [Range(0, 4)]
    public float speed = 1f;
    [Tooltip("target을 쫒아갈때 가속 속도입니다. default = 0")]
    [Range(0, 0.5f)]
    public float accelation = 0;
    [Tooltip("target을 쫒아갈때 한계 속도입니다. default = 10")]
    [Range(1, 4)]
    public float limitActiveTime = 4f;
    public bool tracerTarget = false;

    /* needs component */
    Rigidbody2D rg2d;
    Animator animator;
    SpriteRenderer sr;
    BoxCollider2D box;

    /* need variable */
    Vector2 dir = new Vector2(0,0);
    Vector3 scale;
    Quaternion rotation;
    bool catchState = false;
    bool arrivedState = false;
    float gap = 0.1f;

    private void Start()
    {
        SaveState(true, gameObject.activeSelf, transform.position);
        rg2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        animator.SetInteger("aniState", 0);

    }

    private void OnBecameVisible()
    {
        dir = AngleToVector2(shootAngle);
        StartCoroutine( ActivateToTrap() );
    }

    IEnumerator ActivateToTrap()
    {
        Vector2 destination = initPos + dir;

        while ((CollisionTargetTransform == null)) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복
        if (tracerTarget) destination = CollisionTargetTransform.position;

        StartCoroutine(MoveToDestination(rg2d, destination, speed , 0, true));

        yield return new WaitForSeconds(limitActiveTime);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(CollisionTargetTransform != null)
        {
            if (col.CompareTag(CollisionTargetTransform.tag))
            {
                catchState = true;
                state = false;
                if (sr != null) sr.enabled = false;
                if (box != null) box.enabled = false;
                rg2d.velocity = Vector2.zero;
            }
        }
    }
}
