using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierCtr : MonoBehaviour {
	public Sprite s;
	Animator barrierAnim;
	SpriteRenderer sr;
	bool animState = false;
	// Use this for initialization
	void Awake () {
		barrierAnim = GetComponent<Animator>();
	}

	private void OnEnable() {
		barrierAnim.SetBool("ani", true);
	}
	private void OnDisable() {
		
	}
	
}
