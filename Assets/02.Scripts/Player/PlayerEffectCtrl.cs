using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectCtrl : MonoBehaviour {

	enum PlayerEventType
	{
		JUMP,
		MOVE

	}
	/* INSPECTOR VIRABLE */
	[Header("PLAYER KEY EVENT")]
	[SerializeField] PlayerEventType[] _playerEventType;
	[Tooltip("PlayerEventType 인덱스 순서와 같은 순서로 넣어주면 됩니다.")]
	[SerializeField] Object[] _effectObject;
	[Header("PLAYER COLLISION EVENT")]
	[SerializeField] bool _useColider = false;
	[SerializeField] string[] _coliderTagName;
	[Tooltip("Colider Tag Name 의 인덱스 순서와 같은 순서로 넣어주면 됩니다.")]
	[SerializeField] Object[] _effectColiderObject;
	


	/* COMPONENTS */
	PlayerJump2D _playerJump2D;

	/* INNER VARIABLE */
	KeyCode keys = KeyCode.Space;
	
	private void Start() {
		_playerJump2D = GetComponent<PlayerJump2D>();
	}



	private void Update() {
		for(int idx = 0; idx < _playerEventType.Length; idx++)
		{
			
			switch(_playerEventType[idx])
			{
				case PlayerEventType.JUMP:
				if(Input.GetKeyDown(KeyCode.Space))
				{
					Debug.Log(KeyCode.Space + " Down");
					if(_effectObject[idx] is GameObject && _playerJump2D.JumpState < _playerJump2D.JumpLevel)
					{
						GameObject temp = (GameObject)_effectObject[idx];
						temp.SetActive(true);
					}
				}
				break;
				case PlayerEventType.MOVE:
				break;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if(_useColider)
		{
			if(_coliderTagName.Length > 0)
			{
				for(int i = 0; i < _coliderTagName.Length; i++)
				{
					if(other.gameObject.CompareTag(_coliderTagName[i]))
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
		if(_useColider)
		{
			if(_coliderTagName.Length > 0)
			{
				for(int i = 0; i < _coliderTagName.Length; i++)
				{
					if(other.gameObject.CompareTag(_coliderTagName[i]))
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
