using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsybleScript;

[RequireComponent(typeof(Rigidbody2D))]
public class FishAI : ObjectInteraction {
	/* inspector variable */
	[Tooltip("if you check this, started each ohter")]
	public bool isRandomMoveTime = true;
	[Range(0,10)]
	public float moveTime = 1f;
	[Range(0,10)]
	[Tooltip("it is max value as Random")]
	public float maxRandomMoveTime = 3f;
	[Range(0,10)]
	public float force = 1f;
	[Range(0,10)]
	[Tooltip("same Trigger's Radius")]
	public float distanceToTarget = 2f;

    /* needs component */
    Rigidbody2D rb2d;

    /* need variable */
	Transform target = null;
	Vector2 dirVec = Vector2.zero;
	int directionAngle = 0;
	bool detectedTarget = false;
	bool isTrace = false;
	bool inWater = true; //중요함수. 물안에 있음을 뜻한다.

	private void Start() {
		rb2d = GetComponent<Rigidbody2D>();
		state = true;
	}
	private void OnBecameVisible() {
		StartCoroutine(StartAI());
		StartCoroutine(CheckWater());
	}
	IEnumerator StartAI()
	{
		StartCoroutine( StartFishMove(rb2d));
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
						StopCoroutine("ObjectTargetTraceMove2D");
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
		if(isRandomMoveTime) moveTime = Random.Range(1,maxRandomMoveTime);
		while(state)
		{
			/* 트리거에 아무것도 없을 경우 랜덤한 방향으로 이동합니다. */
			if(!detectedTarget && target == null && inWater)
			{
				yield return new WaitForSeconds(moveTime);
				directionAngle = Random.Range(0,360);
				dirVec = AngleToVector2(directionAngle);
				MovePos(rb2d,dirVec,force * 10,true);
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
			yield return new WaitForFixedUpdate();
		}
	}
	IEnumerator CheckWater()
	{
		int layer = 1 << 10;
		while(state)
		{
			yield return new WaitForSeconds(moveTime * 2);
			if(!RayScript.DetectedOverlapCircle2D(transform.position,0.2f,layer))
			{
				Debug.Log("FishAI.cs - not water platform");
				inWater = false;
				MovePos(rb2d,Vector2.down,force * 10,true);
			}
			else
			{
				inWater = true;
			}
		}
	}
	private void OnTriggerExit2D(Collider2D col) {
		if(col.CompareTag("Water"))
		{
			if(target == null) MovePos(rb2d,-dirVec,force * 10,true);
			else
			{
				print("FishAI.cs - 추적을 종료합니다_충돌함수.");
				CollisionTargetTransform = null;
				target = null;
				detectedTarget = false;
				MovePos(rb2d,Vector2.down,force * 10,true);
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
