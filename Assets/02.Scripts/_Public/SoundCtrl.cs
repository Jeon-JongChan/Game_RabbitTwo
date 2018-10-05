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
	[Tooltip("음악의 시작과 끝을 정합니다. \n Percent 기준입니다.")]
	[SerializeField] Vector2 _audioStartEndTimePer = new Vector2(0,1);
	[SerializeField] int _audioPlayNum = 0;

	/* COMPONENTS */

	/* PUBLIC VIRABLE */

	/* INNER VARIABLE */
	Coroutine _retCoroutine;

	private void Start() {
		_audioSource = GetComponent<AudioSource>();

		if(_audioPlayNum >= 0 && _audioPlayNum < _audioClip.Length)
		{
			switch(_audioPlayType)
			{
				case AudioPlayType.NOMAL:
					_audioSource.clip = _audioClip[_audioPlayNum];
					_retCoroutine = StartCoroutine(AudioPlayControl(_audioSource,_audioStartEndTimePer));
					break;
				case AudioPlayType.ALLONEPLAY:
					_retCoroutine = StartCoroutine(AllPlayLoop(1));
					break;
				case AudioPlayType.ALLPLAYLOOP:
					_retCoroutine = StartCoroutine(AllPlayLoop(0));
					break;
				case AudioPlayType.ALLPLAYCOUNTLOOP:
					_retCoroutine = StartCoroutine(AllPlayLoop(_loopCount));
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
		Coroutine retCoroutine;
		bool loopState = false;

		if(loopCnt == 0) loopState = true;
		_audioSource.loop = false; //한가지 곡만 돌릴게 아니기에 loop를 꺼준다.
		while(loopState || loopCnt > 0)
		{
			if(_audioSource.clip == null) 
			{
				_audioSource.clip = _audioClip[_audioPlayNum++];
				retCoroutine = StartCoroutine(AudioPlayControl(_audioSource,_audioStartEndTimePer));
			}
			else
			{
				if(!_audioSource.isPlaying)
				{
					_audioSource.clip = _audioClip[_audioPlayNum++];
					retCoroutine = StartCoroutine(AudioPlayControl(_audioSource,_audioStartEndTimePer));
				}
			}
			/* 전체곡을 한바퀴 돌았는지 확인 */
			if(_audioPlayNum > _audioClip.Length - 1)
			{
				_audioPlayNum = 0;
				loopCnt--;
			}
			yield return wsDelay;
		}
	}
	IEnumerator AudioPlayControl(AudioSource audioSource, Vector2 startEndTimePer)
	{
		WaitForFixedUpdate wf = new WaitForFixedUpdate();
		float totalMusicTime = audioSource.clip.length;
		float startTime = totalMusicTime * startEndTimePer.x;
		float endTime = totalMusicTime * startEndTimePer.y;

		if(startEndTimePer.x == 0 && startEndTimePer.y == 1) 
		{
			Debug.Log("SoundCtrl - entire play");
			audioSource.Play();
			yield break;
		}
		Debug.Log("SoundCtrl - no entire play " + totalMusicTime + " : " + startTime + " " + endTime);
		audioSource.PlayDelayed(startTime);
		if(totalMusicTime == endTime) yield break;

		while(audioSource.isPlaying)
		{
			if(audioSource.time >= endTime) audioSource.Stop();
			yield return wf;
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

