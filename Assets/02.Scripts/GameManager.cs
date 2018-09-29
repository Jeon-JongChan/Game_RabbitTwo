using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	struct WarpPotalSt
	{
		public Transform tf;
		public WarpCtrl wcScript;
	}
	/* INSPECTOR VARIABLE */
	//[SerializeField] int _startStageNum = 1;
	[SerializeField] GameObject _player;
	[SerializeField] GameObject _mainCamera;
	[SerializeField] bool _isSequential = false;
	//스테이지마다 활성화할 맵의 개수
	[SerializeField] StageSceneName _sceneName = new StageSceneName();
    [SerializeField] float _dieDelayTime = 1.5f;
	[SerializeField] StageInfo[] _stageInfo;

    /* NEEDS COMPONENT */
    Player _playerInstance;
	SaveLoadManager saveManager;

	/* NEEDS VARIABLE */

	readonly int[] _stageMapCount = {9 ,16 ,25};

	Vector2 _cameraWarpPos = Vector2.zero;
	Vector2 _startingPoint = Vector2.zero;

	public int _stageNum = 1; // 각 스테이지 넘버
	int _mapNum = 1; //각 스테이지마다 맵 넘버
	bool _isLoadScene = false;

    event System.Action PlayerDie;
	static bool single = false; // GAMEMANAGER 오브젝트는 하나만 존재해야하므로 STATIC 선언. 첫 시작전에만 FALSE
	bool _isChangeStage = false;
	
    private void Awake()
    {

		// 처음에만 비파괴로 만들어 주고 single 변수가 true면 삭제한다. -> single 변수는 인스턴스마다 존재하는 변수가 아니라
		// GAMEMANAGER 클래스를 가진 모든 인스턴스가 공용으로 사용하는 단 하나의 변수이므로 이미 하나가 생성되어 TRUE 가 되면 다른 인스턴스에서도 TRUE
		if(!single)
		{
			DontDestroyOnLoad(this.gameObject);
			single = true;	
		} 
		else
		{
			Debug.LogWarning("중복된 게임매니저 파괴");
			Destroy(this.gameObject);
		} 

        //_stageNum = _startStageNum;
        foreach (var v in _stageInfo)
        {
            try
            {
                v.stageMapList = new List<GameObject>();
                v.stageEnableMapIdxs = new int[v.stageEnableMapCount];
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        if (!_isSequential)
        {
			for(int i = 0; i < _stageMapCount.Length; i++) StartMapRandomIdx(i+1, _stageMapCount[i]);
        }
        else
        {
            for (int i = 0; i < _stageInfo[0].stageEnableMapCount; i++)
            {
                _stageInfo[0].stageEnableMapIdxs[i] = i + 1;
            }
            for (int i = 0; i < _stageInfo[1].stageEnableMapCount; i++)
            {
                _stageInfo[1].stageEnableMapIdxs[i] = i + 1;
            }
            for (int i = 0; i < _stageInfo[2].stageEnableMapCount; i++)
            {
                _stageInfo[2].stageEnableMapIdxs[i] = i + 1;
            }
        }
    }

    private void Start()
    {
		saveManager = GetComponent<SaveLoadManager>();
		//_stageNum = SceneManager.GetActiveScene().buildIndex;
    }
	

	private void Update() 
	{
		//외부 요인으로 스테이지 변경시 자동으로 스테이지 추적. 에러확률 높음.
		if(SceneManager.GetActiveScene().buildIndex != _stageNum && !_isLoadScene )
		{
			Debug.LogWarning("스테이지 외부요인에 의한 변경");
			_stageNum = SceneManager.GetActiveScene().buildIndex;
			if(_stageNum > 0) GetStageInfo();
		}

		if(_playerInstance != null)
		{
			if (_playerInstance.Dead && !_isLoadScene)
			{
				PlayerDie();
				LoadStage();
				if(!_isLoadScene)LoadGameState();
			}
		}
    }

	public void SaveGameState()
	{
		if(saveManager != null)
		{
			//print("세이브호출");
			if(saveManager.SaveObjectLen > 0)
			{
				saveManager.Save();
			}
			else 
			{
				saveManager.Save(_player.transform,0);
				saveManager.Save(_mainCamera.transform,0);
			}
			
		}
		//print(_player.transform.position);
		PlayerPrefs.SetInt("STAGE",_stageNum);
		PlayerPrefs.SetInt("MAP",_mapNum);
	}
	/// <summary>
	/// 저장된 게임데이터를 불러옵니다.
	/// <summary>
	public void LoadGameState()
	{
		if(saveManager != null)
		{
			if(saveManager.SaveObjectLen > 0)
			{
				saveManager.Load();
			}
			else 
			{
				/* 저장값이 없다면 defalt zero 값이 저장되므로 미리 위치를 백업 - 첫 시작위치 */
				Vector3 tempPlayerPos = _player.transform.position;
				Vector3 tempCameraPos = _mainCamera.transform.position;

				saveManager.Load(_player.transform,0);
				saveManager.Load(_mainCamera.transform,0);

				if(_player.transform.position == Vector3.zero) _player.transform.position = tempPlayerPos;
				if(_mainCamera.transform.position == Vector3.zero) _mainCamera.transform.position = tempCameraPos;
			}
			
		}
		if(PlayerPrefs.HasKey("STAGE"))
		{
			_stageNum = PlayerPrefs.GetInt("STAGE");
			_mapNum = PlayerPrefs.GetInt("MAP");
		}
		//print("Load : " + _player.transform.position);
	}

	public void OnNextStage()
	{
		_isLoadScene = true;
		_stageNum++;
		PlayerPrefs.SetInt("STAGE",_stageNum);
		PlayerPrefs.SetInt("MAP",1);
		LoadStage();
	}

	void LoadStage()
	{
		StartCoroutine("LoadStageCoroutine");
	}
	void GetStageInfo()
	{
		_mainCamera = GameObject.Find("Main Camera");
        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player");
		_playerInstance = _player.GetComponent<Player>();
        PlayerDie += _playerInstance.PlayerDie;

        AddMap(_stageNum);
        ConnectMapWarp(_stageNum);
	}
    IEnumerator LoadStageCoroutine()
    {
		_isLoadScene = true;
        yield return new WaitForSeconds(_dieDelayTime);

		AsyncOperation asynOper = SceneManager.LoadSceneAsync(_sceneName.level1[_stageNum - 1]);
		Time.timeScale = 0;
		while(!asynOper.isDone) 
		{
			if(asynOper.progress < 0.9)
			{
				print("아직 로딩중");
				yield return null;
			}
		}
		Time.timeScale = 1;
		GetStageInfo();
		_isLoadScene = false;
    }
	

	public void MapWarpEvent(Vector2 CameraWarpPos,string map)
	{
		_cameraWarpPos = CameraWarpPos;

		Vector3 temp = new Vector3(_cameraWarpPos.x,_cameraWarpPos.y,_mainCamera.transform.position.z);
		_mainCamera.transform.position = temp;
		ThisMap(map);
	}
	// public void ChaneStage(int stageNum = -1)
	// {
	// 	if(stageNum != -1) _stageNum++;
	// 	_isChangeStage = true;
	// }
	void ThisMap(string map)
	{
		string mapString = "Map";
		
		for(int i = 1; i < _stageMapCount[_stageNum - 1] ; i++)
		{
			if(map == mapString + i.ToString())
			{
				_mapNum = i;
				break;
			}
		}
		print("_mapNum : " + _mapNum + " " + map);
	}


	void AddMap(int stageNum)
	{
		string mapStr = "Map";
		string tempStr = "";
		_stageInfo[stageNum-1].stageMapList.Clear();
		for(int i = 0; i < _stageInfo[stageNum-1].stageEnableMapCount; i++)
		{
			tempStr = mapStr + _stageInfo[stageNum-1].stageEnableMapIdxs[i].ToString();
			//print(tempStr);
			_stageInfo[stageNum-1].stageMapList.Add(GameObject.Find(tempStr));
		}
	}
	void ConnectMapWarp(int stageNum)
	{
		int mapEnableArrCount = _stageInfo[stageNum-1].stageEnableMapCount;

		WarpPotalSt[] startWp = new WarpPotalSt[mapEnableArrCount];
		WarpPotalSt[] endWp = new WarpPotalSt[mapEnableArrCount];
		Transform[] mapCameraPos = new Transform[mapEnableArrCount];

		/* start와 end potal 을 모두 가져온다. 카메라 이동위치도 갖고 온다.*/
		for(int i = 0; i < mapEnableArrCount; i++)
		{
			//print(SceneManager.GetActiveScene().buildIndex);
            //print(_stageInfo[stageNum - 1].stageMapList[i].transform.name);
			startWp[i].tf = _stageInfo[stageNum-1].stageMapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotalManager").Find("StartPotal");
			startWp[i].wcScript = startWp[i].tf.GetComponentInParent<WarpCtrl>();

			endWp[i].tf = _stageInfo[stageNum-1].stageMapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotalManager").Find("EndPotal");
			endWp[i].wcScript = endWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

			mapCameraPos[i] = _stageInfo[stageNum-1].stageMapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
		}
		
		/* 각 포탈마다 접촉시 이동할 위치를 넣어준다. */
		for(int i = 0; i < mapEnableArrCount; i++)
		{
			if(i == 0)
			{
				endWp[i].wcScript.ConnectedPotal(startWp[i+1].tf,mapCameraPos[i+1]);
			}
			else if(i == mapEnableArrCount - 1)
			{
				//startWp[i].wcScript.ConnectedPotal(endWp[i-1].tf,mapCameraPos[i-1]);
			}
			else
			{
				//startWp[i].wcScript.ConnectedPotal(endWp[i-1].tf,mapCameraPos[i-1]);
				endWp[i].wcScript.ConnectedPotal(startWp[i+1].tf,mapCameraPos[i+1]);
			}
		}

		_startingPoint = startWp[0].tf.position;
	}
	void StartMapRandomIdx(int stageNum, int mapTotalCount)
	{
		int rand = 0;
		int mapEnableArrCount = _stageInfo[stageNum - 1].stageEnableMapCount;

		for(int i = 0; i < mapEnableArrCount; i++)
		{
			/* 맵 처음과 끝은 0 과 마지막 맵으로 고정. */
			if(i == 0)
			{
				rand = 1;
			}
			else if(i == mapEnableArrCount -1) rand = mapTotalCount;
			else
			{
				for(int j = 0; j < i;j++)
				{
					if(rand ==  _stageInfo[stageNum - 1].stageEnableMapIdxs[j])
					{
						rand = Random.Range(2,mapTotalCount);
						j = 0;
						continue;
					}
				}
			}
			_stageInfo[stageNum - 1].stageEnableMapIdxs[i] = rand;

		}
	}
}
[System.Serializable]
class StageSceneName
{
	public string[] level1 = new string[3];
	public string[] level2 = new string[3];
}
[System.Serializable]
class StageInfo
{
	public int stageEnableMapCount = 0;
	public List<GameObject> stageMapList;
	public int[] stageEnableMapIdxs;
}