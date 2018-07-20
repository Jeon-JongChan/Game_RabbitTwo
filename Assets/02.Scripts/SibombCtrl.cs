using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SibombCtrl : MonoBehaviour {
	public GameObject explosionPrefab;
	public GameObject explosionForce;
	public float delayBombTime = 0;
	public bool isDisable = false;
	const float offBombTime = 0.5f;

	float bombDistance = 10f;
	bool explosionState = false;

	private void Awake() {
	}
	IEnumerator StartSibomb()
	{
		yield return new WaitForSeconds(delayBombTime);
		explosionPrefab.SetActive(!explosionState);
		explosionForce.SetActive(!explosionState);
		yield return new WaitForSeconds(offBombTime);
		explosionPrefab.SetActive(explosionState);
		explosionForce.SetActive(explosionState);
		gameObject.SetActive(!isDisable);
	}

	private void OnCollisionEnter2D(Collision2D col) {
		StartCoroutine(StartSibomb());
	}
	private void OnTriggerEnter2D(Collider2D col) {
		StartCoroutine(StartSibomb());
	}
}
