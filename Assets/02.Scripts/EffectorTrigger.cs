using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectorTrigger : ObjectInteraction
{
	public bool useTrigger = true;
	public Collider2D col = null;
	WaitForFixedUpdate wFixUp;
    // Use this for initialization
    void Awake () {
		SaveState(true,!useTrigger,transform.position);
		if(col != null)
		{
			col.enabled = currActive;
		}
	}
	
	private void OnBecameVisible()
	{
		if(currActive) StartCoroutine(StartEffector());
	}
	IEnumerator StartEffector()
	{
		while(state)
		{
			while(useTrigger && CollisionTargetTransform == null) yield return new WaitForFixedUpdate();
			if(useTrigger)
			{
				col.enabled = !currActive;
			}
			yield return new WaitForFixedUpdate();
		}
	}
}
