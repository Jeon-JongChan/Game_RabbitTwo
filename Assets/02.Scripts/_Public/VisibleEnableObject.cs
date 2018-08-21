using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleEnableObject : ObjectInteraction {

	[SerializeField] Collider2D selfCol;
	[SerializeField] bool isInitDisable = false;
	[SerializeField] bool useTrigger = false;
	Rigidbody2D rb2d;
	SpriteRenderer sr;
	WaitForSeconds wsDelayTrigger;
	Color srColor;
	
	// Use this for initialization
	void Start () {
		initPos = transform.position;
		rb2d = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		if(isInitDisable)
		{
			if(rb2d != null) rb2d.gravityScale = 0;
			if(selfCol != null) selfCol.enabled = false;
			if(sr != null)
			{
				srColor = sr.color;
				srColor.a = 0;
				sr.color = srColor;
			}
		}
		wsDelayTrigger = new WaitForSeconds(0.2f);
	}

	private void OnBecameVisible() {
		print("VisibleEnableObject.cs : 화면에 보임");
		//StopCoroutine("StartVisibleEnableObject");
		StartCoroutine("StartVisibleEnableObject");
	}

	private void OnBecameInvisible() {
		print("VisibleEnableObject.cs : 화면에 안보임");
		//transform.position = initPos;
		if(rb2d != null) rb2d.gravityScale = 0;
		//if(selfCol != null) selfCol.enabled = false; //김태윤이 오브젝트가 자꾸 다른 맵으로 침범하는 문제때문에 막아둠.
		if(sr != null)
		{
			srColor.a = 0;
			sr.color = srColor;
		}
	}

	IEnumerator StartVisibleEnableObject()
	{
		while(CollisionTargetTransform == null && useTrigger) 
		{
			//print("반복수행 " + gameObject.name + CollisionTargetTransform  +  useTrigger + rb2d.gravityScale);
			yield return wsDelayTrigger;
		}
		if(rb2d != null) rb2d.gravityScale = 1;
		if(selfCol != null) selfCol.enabled = true;
		if(sr != null)
		{
			srColor.a = 1;
			sr.color = srColor;
		}
	}

}
