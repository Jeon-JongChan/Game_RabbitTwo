using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GuideMissile : ObjectInteraction {

    public bool useTrigger = false;
    [Tooltip("Trigger 선택시 타겟을 넣지 않아도 됩니다.")]
    public GameObject target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnBecameVisible()
    {
        //StartCoroutine( Shoot() );
    }
    //IEnumerator Shoot()
    //{
    //    if(state)
    //    {
    //        if(useTrigger) // 트리거 사용시
    //        {
    //            //while (CollisionTargetDirection == Vector2.zero) yield return new WaitForFixedUpdate(); //트리거가 타겟을 발생하기 전까지 반복



    //        }
    //        else
    //        {

    //        }
    //    }
    //}
}
