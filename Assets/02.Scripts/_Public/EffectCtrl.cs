using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCtrl : MonoBehaviour {

	/* INSPECTOR VIRABLE */
	[SerializeField] EffectEventType _eventType = new EffectEventType();
	[Tooltip("Key 인덱스 순서와 같은 순서로 넣어주면 됩니다.")]
	[SerializeField] Object[] _effectObject;
	[Tooltip("Colider Tag Name 의 인덱스 순서와 같은 순서로 넣어주면 됩니다.")]
	[SerializeField] Object[] _effectColiderObject;
	


	/* COMPONENTS */

	/* PUBLIC VIRABLE */

	/* INNER VARIABLE */
	
	private void OnBecameVisible() {
		StartCoroutine("EffectCoroutine");
	}
	private void OnBecameInvisible() {
		StopCoroutine("EffectCoroutine");
	}
	IEnumerator EffectCoroutine()
	{
		WaitForFixedUpdate wf = new WaitForFixedUpdate();
		while(true)
		{
			if(_eventType.effectKey.Length > 0)
			{
				for(int i = 0; i < _eventType.effectKey.Length; i++)
				{
					if(Input.GetKey(_eventType.effectKey[i]))
					{
						//Debug.Log(_eventType.effectKey[i] + " Down");
						if(_effectObject[i] is GameObject)
						{
							GameObject temp = (GameObject)_effectObject[i];
							temp.SetActive(true);
						}
						break;
					}
				}
			}
			yield return wf;
		}
	}
	private void OnCollisionEnter2D(Collision2D other) {
		if(_eventType.useColider)
		{
			if(_eventType.coliderTagName.Length > 0)
			{
				for(int i = 0; i < _eventType.coliderTagName.Length; i++)
				{
					if(other.gameObject.CompareTag(_eventType.coliderTagName[i]))
					{
						if(_effectColiderObject[i] is GameObject)
						{
							GameObject temp = (GameObject)_effectColiderObject[i];
							temp.SetActive(true);
						}
						break;
					}
				}
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D other) {
		if(_eventType.useColider)
		{
			if(_eventType.coliderTagName.Length > 0)
			{
				for(int i = 0; i < _eventType.coliderTagName.Length; i++)
				{
					if(other.gameObject.CompareTag(_eventType.coliderTagName[i]))
					{
						if(_effectColiderObject[i] is GameObject)
						{
							GameObject temp = (GameObject)_effectColiderObject[i];
							temp.SetActive(true);
						}
						break;
					}
				}
			}
		}
	}
}
[System.Serializable]
class EffectEventType
{
	public KeyCode[] effectKey;
	public bool useColider;
	public string[] coliderTagName;
}
