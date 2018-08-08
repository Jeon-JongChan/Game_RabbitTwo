using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCtrl : MonoBehaviour {

	[SerializeField] string[] targetTag;
	Animator switchAnimator;
	
	// Use this for initialization
	void Start () {
		switchAnimator = GetComponent<Animator>();	
	}
	private void OnTriggerEnter2D(Collider2D col) {
		if(targetTag.Length > 0)
		{
			foreach(var v in targetTag)
			{
				if(col.CompareTag(v))
				{
					switchAnimator.SetTrigger("SWITCH");
				}
			}
		}
		else
		{
			switchAnimator.SetTrigger("SWITCH");
		}
	}
}
