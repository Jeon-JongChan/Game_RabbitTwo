using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour {
	public float rotateSpeed = 1;
	bool rotateState = true;
	WaitForFixedUpdate wFixUp;
	// Use this for initialization
	private void Start() {
		wFixUp = new WaitForFixedUpdate();
		StartCoroutine(StartRotation());
	}
	private void OnBecameVisible() {
		print("보입니다");
		rotateState = true;
		StartCoroutine(StartRotation());
	}
	private void OnBecameInvisible() {
		print("안보입니다");
		rotateState = false;
	}
	IEnumerator StartRotation()
	{
		while(rotateState)
		{		
			transform.Rotate(0,0,rotateSpeed);
			yield return wFixUp;
		}
	}
}
