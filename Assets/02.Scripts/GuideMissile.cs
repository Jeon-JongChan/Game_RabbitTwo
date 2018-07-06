using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GuideMissile : ObjectInteraction{

    [Tooltip(" 선택을 안할 경우 화면을 인식하자마자 플레이어를 쫒아갑니다. ")]
    public bool useTrigger = false;
    [Tooltip("Trigger 선택시 타겟을 넣지 않아도 됩니다.")]
    public GameObject target = null;
    [Tooltip("target을 쫒아갈때 탐지하는 delay 입니다.")]
    public float delayTimeToFindTarget = 1f;
    [Range(0,10)]
    public float speed = 1f;
    [Tooltip("target을 쫒아갈때 가속 속도입니다. default = 0")]
    [Range(0,1)]
    public float accelation = 0;
    [Tooltip("target을 쫒아갈때 한계 속도입니다. default = 10")]
    [Range(1,20)]
    public float limitSpeed = 10f;

    /* needs variable */
    Rigidbody2D rg2d;
    bool stopState = true;

    // Use this for initialization
    void Start () {
        rg2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnBecameVisible()
    {
        StartCoroutine( Shoot() );
    }
    IEnumerator Shoot()
    {
        if (state)
        {
            while ((CollisionTargetTransform == null) && useTrigger) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복

            float finalSpeed = speed;
            while (state)
            {
                stopState = true;
                if (useTrigger) StartCoroutine(MoveToGuideMissile(target.transform.position, finalSpeed));
                else if (target != null) StartCoroutine(MoveToGuideMissile(target.transform.position, finalSpeed));
                else
                {
                    Debug.Log("target이 존재하지 않습니다.");
                    break;
                }
                yield return new WaitForSeconds(delayTimeToFindTarget);
                stopState = false;
                if (finalSpeed < limitSpeed) finalSpeed += accelation;
            }

        }
        CollisionTargetTransform = null;
    }
    IEnumerator MoveToGuideMissile(Vector2 destination,float speed)
    {
        //float gap = 0.02f * speed;
        Vector2 dir = destination - rg2d.position;
        dir.Normalize();
        while (stopState)
        {
            MovePos(rg2d, dir, speed, false);
            yield return new WaitForFixedUpdate();
        }
    }

    void CollisionReaction()
    {
        rg2d.velocity = Vector2.zero;
        state = false;
        stopState = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (target != null)
        {
            if (col.CompareTag(target.tag)) CollisionReaction();
            
        }
        else if(useTrigger)
        {
            if (col.CompareTag(CollisionTargetTransform.tag)) CollisionReaction();
        }
    }

}
