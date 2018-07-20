using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCtrl : MonoBehaviour {
	public bool isRandom = false;
	public int warpPosIdx = 0;
	[SerializeField]
	public string[] warpTargetTag; 
	//Transform[] warpPos;
	public List<Transform> warpPos;
	// Use this for initialization
	void Awake () {
		GetComponentsInChildren<Transform>(warpPos);
		warpPos.RemoveAt(0);
		// warpPos = GetComponentsInChildren<Transform>();
		// print(warpPos.Length);
	}

	private void OnTriggerEnter2D(Collider2D col) {
		foreach(var v in warpTargetTag)
		{
			if(col.CompareTag(v))
			{	
				//if(isRandom)warpPosIdx = Random.Range(0,warpPos.Length);
				if(isRandom)warpPosIdx = Random.Range(0,warpPos.Count);
				col.gameObject.transform.position = warpPos[warpPosIdx].position;
				Rigidbody2D tempRb2d =  col.GetComponent<Rigidbody2D>();
				if(tempRb2d != null) tempRb2d.velocity = Vector2.zero;
			}
		}
	}
}
