using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectorTrigger : ObjectInteraction
{
	public bool useTrigger = true;
	public Collider2D col = null;
    // Use this for initialization
    void Awake () {
		SaveState(true,!useTrigger,transform.position);
		if(col != null)
		{
			col.enabled = active;
		}
	}
	
	private void OnBecameVisible()
	{
		StartCoroutine(StartEffector());
	}
	IEnumerator StartEffector()
	{
		while(state)
		{
			while(state && CollisionTargetTransform == null) yield return new WaitForFixedUpdate();
			if(useTrigger)
			{
				col.enabled = !active;
			}
			yield return new WaitForFixedUpdate();
		}
	}
}
