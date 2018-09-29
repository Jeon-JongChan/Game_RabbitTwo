using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

[RequireComponent(typeof(Rigidbody2D))]
public class FishAI : ObjectInteraction {
	/* inspector variable */
	[Range(0,10)]
	[Tooltip("물고기의 이동경로를 바꾸는 시간")]
	public float _changeDirectionTime = 3f;
	[Range(0,30)]
	public float force = 1f;
	[Range(0,10)]
	[Tooltip("same Trigger's Radius")]
	public float distanceToTarget = 2f;

    /* needs component */
    Rigidbody2D rb2d;

    /* need variable */
	Transform target = null;
	Vector2 dirVec = Vector2.zero;
	float checkWaterTime = 1f;
	int directionAngle = 0;
	bool detectedTarget = false;
	bool isTrace = false;
	bool inWater = true; //중요함수. 물안에 있음을 뜻한다.
	bool colisionTrigger = false;

	private void Start() {
		rb2d = GetComponent<Rigidbody2D>();
		state = true;
	}
	private void OnBecameVisible() {
		StartCoroutine("StartAI");
		StartCoroutine("CheckWater");
	}
	private void OnBecameInvisible() {
		StopCoroutine("StartAI");
		StopCoroutine("CheckWater");
	}
	IEnumerator StartAI()
	{
		StartCoroutine( StartFishMove(rb2d));
		StartCoroutine("ChangeDirection");
		/* 상황에 따라 트리거를 조정하는 부분 */
		while(state)
		{
			if(CollisionTargetTransform != null)
			{
				if(target == null) target = CollisionTargetTransform;
				else
				{
					/* 타겟과 거리가 멀어지면 추적을 포기한다. */
					if(Vector2.Distance(target.position, transform.position) > distanceToTarget)
					{
						print("FishAI.cs - 타겟이 멀어졌습니다.");
						CollisionTargetTransform = null;
						target = null;
						detectedTarget = false;
					}
				}
			}

			yield return new WaitForFixedUpdate();
		}
	}
	///<summary>
	///it is cooroutine that control the fish's move.
	///</summary>
	IEnumerator StartFishMove(Rigidbody2D rb2d)
	{
		while(state)
		{
			print("FishAI.cs - 움직임이 시작됩니다.");
			/* 트리거에 아무것도 없을 경우 랜덤한 방향으로 이동합니다. */
			if(!detectedTarget && target == null && inWater)
			{
				print("FishAI.cs - 움직입니다." + dirVec);
				rb2d.velocity = Vector2.zero;
				MovePos(rb2d,dirVec,force,true);
			}
			else if(!detectedTarget && inWater && target != null) detectedTarget = true;
			else if(detectedTarget && inWater && target != null )
			{
				print("FishAI.cs - 타겟을 탐지했습니다");
				float limit = 0.05f * force; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            	Vector2 destination = Vector2.zero;
            	Vector2 distanceToMove = Vector2.zero;
                do
                {
                    if(destination != (Vector2)target.transform.position)
                    {
                        destination = target.transform.position;
                        distanceToMove = destination - rb2d.position;
                        //print(distanceToMove);
                    }
                    Move(rb2d, distanceToMove, force,false);
                    yield return null;
                }while( detectedTarget && (!(Vector2.Distance(destination, rb2d.position) < limit && Vector2.Distance(destination, rb2d.position) > -limit)));
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
	IEnumerator CheckWater()
	{
		int layer = 1 << 10;
		while(state)
		{
			yield return new WaitForSeconds(checkWaterTime);
			if(!RayScript.DetectedOverlapCircle2D(transform.position,0.2f,layer))
			{
				Debug.Log("FishAI.cs - not water platform");
				inWater = false;
				rb2d.velocity = Vector2.zero;
				MovePos(rb2d,Vector2.down,force * 2,true);
			}
			else
			{
				inWater = true;
			}
		}
	}
	IEnumerator ChangeDirection()
	{
		WaitForSeconds ws = new WaitForSeconds(_changeDirectionTime);
		while(state)
		{
			if(colisionTrigger) colisionTrigger = false;
			else if(!colisionTrigger)
			{
				directionAngle = ((directionAngle + 180) %360) + Random.Range(-60, 60);
				dirVec = AngleToVector2(directionAngle);
			}
			yield return ws;
		}
	}
	private void OnCollisionEnter2D(Collision2D col) {
		print("FishAI.cs - 충돌객체 : " + col.gameObject.name);
		colisionTrigger = true;
		directionAngle = ((directionAngle + 180) %360) + Random.Range(-60, 60);
		dirVec = AngleToVector2(directionAngle);
	}
	private void OnCollisionStay2D(Collision2D col) {
		if(col.gameObject.CompareTag("Water"))
		{
			print("FishAI.cs - 물 속 입니다.");
			if(!inWater) inWater = true;
		}
	}
	private void OnCollisionExit2D(Collision2D col) {
		if(col.gameObject.CompareTag("Water"))
		{
			if(target == null) MovePos(rb2d,-dirVec,force * 2,true);
			else
			{
				print("FishAI.cs - 추적을 종료합니다_충돌함수.");
				CollisionTargetTransform = null;
				target = null;
				detectedTarget = false;
				rb2d.velocity = Vector2.zero;
				MovePos(rb2d,Vector2.down,force * 2,true);
			}
			print("FishAI.cs - 물밖입니다." + detectedTarget + isTrace);
			inWater = false;
			rb2d.velocity = Vector2.zero;
		}
	}
	private void OnTriggerExit2D(Collider2D col) {
		if(col.CompareTag("Water"))
		{
			if(target == null) MovePos(rb2d,-dirVec,force * 2,true);
			else
			{
				print("FishAI.cs - 추적을 종료합니다_충돌함수.");
				CollisionTargetTransform = null;
				target = null;
				detectedTarget = false;
				rb2d.velocity = Vector2.zero;
				MovePos(rb2d,Vector2.down,force * 2,true);
			}
			print("FishAI.cs - 물밖입니다." + detectedTarget + isTrace);
			inWater = false;
			rb2d.velocity = Vector2.zero;
		}
	}
	private void OnTriggerStay2D(Collider2D col) {
		if(col.CompareTag("Water"))
		{
			if(!inWater) inWater = true;
		}
	}

}
