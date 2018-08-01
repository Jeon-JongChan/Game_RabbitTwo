using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCtrl : MonoBehaviour {

	/* INSPECTOR VARIABLE */
	[Tooltip("If you have selected this, select isBiDirect")]
	[SerializeField] bool isMapWarp = false;
	[SerializeField] Transform mapCameraPos;
	[SerializeField] bool isBiDirect = false;
	[SerializeField] bool isRandom = false;
	[SerializeField] float warpDelayTime = 1f;
	[SerializeField] string[] warpTargetTag; 
	[SerializeField] List<Transform> warpPos;

	event System.Action<Vector2> MapWarpEvent;
	int warpPosIdx = 0;

	WaitForSeconds wsWarpDelay;
	// Use this for initialization
	void Awake () {
		if(!isBiDirect)
		{
			GetComponentsInChildren<Transform>(warpPos);
			warpPos.RemoveAt(0);
		}
		wsWarpDelay = new WaitForSeconds(warpDelayTime);
		// warpPos = GetComponentsInChildren<Transform>();
		// print(warpPos.Length);
	}
	private void Start() {
		if(isMapWarp) MapWarpEvent += GameObject.Find("GameManager").GetComponent<GameManager>().MapWarpEvent;
	}
	private void OnTriggerEnter2D(Collider2D col) {
		foreach(var v in warpTargetTag)
		{
			if(col.CompareTag(v))
			{	
				StartCoroutine(StartWarp(col));
			}
		}
	}
	
	IEnumerator StartWarp(Collider2D col)
	{
		/* 워프시 속도 제거 */
		Rigidbody2D tempRb2d =  col.GetComponent<Rigidbody2D>();
		if(tempRb2d != null) tempRb2d.velocity = Vector2.zero;
		if(warpPos.Count > 0) //이동 포탈이 존재할 경우에만 이동 시킨다.
		{
			/* 랜덤 워프 체크시  */
			if(isRandom) warpPosIdx = Random.Range(0,warpPos.Count);

			/* 맵 이동 워프일 경우 이벤트 발생 */
			if(isMapWarp) MapWarpEvent(mapCameraPos.position);

			warpPos[warpPosIdx].gameObject.SetActive(false);
			col.gameObject.transform.position = warpPos[warpPosIdx].position;

			yield return wsWarpDelay;
			warpPos[warpPosIdx].gameObject.SetActive(true);
		}
	}

	public void ConnectedPotal(Transform _warpPos, Transform _mapCameraPos)
	{
		isMapWarp = true;
		isBiDirect = true;
		warpPos.Add(_warpPos);
		mapCameraPos = _mapCameraPos;
	}
}
