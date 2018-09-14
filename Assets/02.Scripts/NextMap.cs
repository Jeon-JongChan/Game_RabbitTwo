using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextMap : MonoBehaviour {

	[SerializeField] string[] _targetTagName;
	GameManager _gameManager;
	System.Action OnNext;

	private void Start() {
		_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();	
		if(_gameManager != null) OnNext = _gameManager.OnNextStage; 
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(_targetTagName.Length > 0)
		{
			foreach(var v in _targetTagName)
			{
				if(other.CompareTag(v))
				{
					OnNext();
				}
			}
		}
		else{
			OnNext();
		}
		gameObject.SetActive(false);
	}

}
