using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSave : MonoBehaviour {

	GameManager gameManager;
	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	private void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Player"))
		{
			print("충돌 세이브");
			gameManager.SaveGameState();
		}
	}
}
