using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundCtrl : MonoBehaviour {

	/* INSPECTOR VIRABLE */
	[SerializeField] AudioPlayType _audioPlayType = AudioPlayType.NOMAL;
	[SerializeField] AudioSource _audioSource;
	[SerializeField] AudioClip[] _audioClip;
	[Tooltip("ALLPLAYCOUNTLOOP 선택 할 경우 반복할 횟수입니다.")]
	[SerializeField] int _loopCount = 0;
	[SerializeField] int _audioPlayNum = 0;

	/* COMPONENTS */

	/* PUBLIC VIRABLE */

	/* INNER VARIABLE */
	Coroutine retCoroutine;

	private void Start() {
		_audioSource = GetComponent<AudioSource>();

		if(_audioPlayNum >= 0 && _audioPlayNum < _audioClip.Length)
		{
			switch(_audioPlayType)
			{
				case AudioPlayType.NOMAL:
					_audioSource.clip = _audioClip[_audioPlayNum];
					_audioSource.Play();
					break;
				case AudioPlayType.ALLONEPLAY:
					retCoroutine = StartCoroutine(AllPlayLoop(1));
					break;
				case AudioPlayType.ALLPLAYLOOP:
					retCoroutine = StartCoroutine(AllPlayLoop(0));
					break;
				case AudioPlayType.ALLPLAYCOUNTLOOP:
					retCoroutine = StartCoroutine(AllPlayLoop(_loopCount));
					break;
			}
		}
		else{
			Debug.LogError("SoundCtrl - play 인덱스 범위 초과");
		}
	}

	///<summary>
	/// 전체곡을 반복해서 플레이 합니다.
	///<summary>
	IEnumerator AllPlayLoop(int loopCnt = 0)
	{
		WaitForSeconds wsDelay = new WaitForSeconds(0.1f);
		bool loopState = false;

		if(loopCnt == 0) loopState = true;
		_audioSource.loop = false; //한가지 곡만 돌릴게 아니기에 loop를 꺼준다.
		while(loopState || loopCnt > 0)
		{
			if(_audioSource.clip == null) 
			{
				_audioSource.clip = _audioClip[_audioPlayNum++];
			}
			else
			{
				if(!_audioSource.isPlaying)
				{
					_audioSource.clip = _audioClip[_audioPlayNum++];
				}
			}
			_audioSource.Play();
			/* 전체곡을 한바퀴 돌았는지 확인 */
			if(_audioPlayNum > _audioClip.Length - 1)
			{
				_audioPlayNum = 0;
				loopCnt--;
			}
			yield return wsDelay;
		}
	}
	enum AudioPlayType
	{
		NOMAL,
		ALLONEPLAY,
		ALLPLAYLOOP,
		ALLPLAYCOUNTLOOP
	}
}

