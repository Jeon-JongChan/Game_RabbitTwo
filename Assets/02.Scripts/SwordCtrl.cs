using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WayPoint
{
	public Transform startPoint;
	public Transform enterPoint;
	public Transform[] departurePoint;
}
[RequireComponent(typeof(Rigidbody2D))]
public class SwordCtrl : ObjectInteraction {

	/* inspector variable */
	public bool useTrigger = false;
	[Tooltip("시작 지점을 랜덤하게 작동시킵니다. 다수의 포인트를 만든 경우만 사용해주세요.")]
	public bool isRandom = false;
	[Range(0, 10)]
	public float speed = 1f;
	public float delayMoveTime = 1f;
	[SerializeField]
	public WayPoint[] paths;

	/* needs components */
	Rigidbody2D rb2d;

	/* needs variable */
	WaitForSeconds wsDelayMove;
	WaitForFixedUpdate wsFixUp;

	private void Awake() {
		wsDelayMove = new WaitForSeconds(delayMoveTime);
		wsFixUp = new WaitForFixedUpdate();
		rb2d = GetComponent<Rigidbody2D>();
	}

	private void OnEnable() {
		StartCoroutine(StartSword());
	}

	IEnumerator StartSword()
	{
		if(useTrigger) while(CollisionTargetTransform == null) yield return new WaitForFixedUpdate();
		while(state && paths.Length > 0)
		{
			float limit = 0.5f * speed; //속도에 따라 타겟에 접근했는지 판단할 거리를 늘려준다.
            int i = 0;
            Vector2 destination = Vector2.zero;
            Vector2 distanceToMove = Vector2.zero;
			int randomDeparturePointIdx = 0;


			//sword 를 진행시킬 path들을 하나씩 받아온다.
            foreach(var v in paths)
            {
				// 한번에 이동할 start, enter, departure 포인트들을 순서대로 하나의 배열에 입력
				if(paths[0].departurePoint.Length > 0)randomDeparturePointIdx = Random.Range(0,v.departurePoint.Length);
				List<Transform> targets = new List<Transform>();
				transform.position = v.startPoint.position;
				targets.Add(v.enterPoint);
				targets.Add(v.departurePoint[randomDeparturePointIdx]);
				//순서대로 위치포인트로 이동시킨다.
                while (state && i < targets.Count)
                {
                    destination = targets[i].position;
                    distanceToMove = destination - rb2d.position;
                    print(distanceToMove + "  " + i);

                    while (state && !(Vector2.Distance(destination, rb2d.position) < limit && Vector2.Distance(destination, rb2d.position) > -limit))
                    {
                        if(destination != (Vector2)targets[i].position)
                        {
                            destination = targets[i].position;
                            distanceToMove = destination - rb2d.position;
                            //print(distanceToMove);
                        }
                        MovePos(rb2d, distanceToMove, speed);
                        yield return wsFixUp;
                    }
                    rb2d.position = destination; // limit 변수에 따른 위치오차를 없앤다.
                    rb2d.velocity = Vector2.zero;
                    i++;
					yield return wsDelayMove;
                }
            }


		}
	}


}
