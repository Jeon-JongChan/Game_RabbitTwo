﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
	[SerializeField] GameObject createObj;
	[SerializeField] float createDelay;
	[SerializeField] int limitCount;
	[SerializeField] bool isDisappear = false;

	WaitForSeconds wsCreateDelay;
	List<GameObject> createObjs;
	float startingDelay = 0;
	void OnBecameVisible() {
		StartCoroutine("StartSpooler");
	}
	private void Awake() {
		createObjs = new List<GameObject>();
		wsCreateDelay = new WaitForSeconds(createDelay);
		GameObject tempObj;
		Vector3 tempVec = transform.position;
		for(int i = 0; i < limitCount; i++)
		{
			float rand = Random.Range(-2.0f,2.0f);
			tempVec.x += rand;
			tempObj = Instantiate(createObj,tempVec,Quaternion.identity,transform);
			tempObj.SetActive(false);
			createObjs.Add(tempObj);
		}
	}
	IEnumerator StartSpooler()
	{
		print("실행");
		yield return wsCreateDelay;
		for(int createObjsIdx = 0; createObjsIdx < createObjs.Count; createObjsIdx++)
		{
			print(createObjs[createObjsIdx].name);
			createObjs[createObjsIdx].SetActive(true);
			yield return wsCreateDelay;
		}
	}
}
