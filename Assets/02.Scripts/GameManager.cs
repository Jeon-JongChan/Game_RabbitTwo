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
	[SerializeField] int _startStageNum = 1;
	[SerializeField] GameObject _player;
	[SerializeField] GameObject _mainCamera;
	[SerializeField] bool _isSequential = false;
	//스테이지마다 활성화할 맵의 개수
	[SerializeField] StageSceneName _sceneName = new StageSceneName();
    [SerializeField] float _dieDelayTime = 1.5f;
	[SerializeField] StageInfo[] _stageInfo;
    public GameObject _pausePage;

    /* NEEDS COMPONENT */
    Player _playerInstance;

	int[] _stageMapCount = {9 ,16 ,25};

	bool _isMapWarp = false;
	bool _isStartStage = true;
	Vector2 _cameraWarpPos = Vector2.zero;
	Vector2 _startingPoint = Vector2.zero;

	static int _stageNum = 1;

    bool isPauseing = false;
    event System.Action PlayerDie;


	private void Start()
    {
        //pausePage.SetActive(false);
		_mainCamera = GameObject.Find("Main Camera");
        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player");
		_playerInstance = _player.GetComponent<Player>();
        PlayerDie += _playerInstance.PlayerDie;
    }
	private void Awake() {
		//DontDestroyOnLoad(gameObject);
		_stageNum = _startStageNum;
		foreach(var v in _stageInfo)
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

		if(!_isSequential)
		{
			StartMapRandomIdx(1,_stageMapCount[0]);
			StartMapRandomIdx(2,_stageMapCount[1]);
			StartMapRandomIdx(3,_stageMapCount[2]);
		}
		else
		{
			for(int i = 0; i < _stageInfo[0].stageEnableMapCount; i++)
			{
				_stageInfo[0].stageEnableMapIdxs[i] = i+1;
			}
			for(int i = 0; i < _stageInfo[1].stageEnableMapCount; i++)
			{
				_stageInfo[1].stageEnableMapIdxs[i] = i+1;
			}
			for(int i = 0; i < _stageInfo[2].stageEnableMapCount; i++)
			{
				_stageInfo[2].stageEnableMapIdxs[i] = i+1;
			}
		}

		AddMap(_stageNum);
		ConnectMapWarp(_stageNum);
	}

	private void Update() {
		// if (Input.GetButton("Cancel")&&!isPauseing) {
        //     GamePause();
        // }
		if(_isMapWarp)
		{
			_isMapWarp = false;
			Vector3 temp = new Vector3(_cameraWarpPos.x,_cameraWarpPos.y,_mainCamera.transform.position.z);
			_mainCamera.transform.position = temp;
			print(_cameraWarpPos);
			_cameraWarpPos = Vector2.zero;
		}

		if(_isStartStage)
		{
			_player = GameObject.FindGameObjectWithTag("Player");
			_player.transform.position =  _startingPoint;
			_isStartStage = false;
		}

        if (_playerInstance.Dead)
        {
            AfterDie();
        }
    }



    void AfterDie()
    {
        PlayerDie();
        StartCoroutine("LoadMap");
    }

    IEnumerator LoadMap()
    {
        yield return new WaitForSeconds(_dieDelayTime);
        switch (_stageNum)
        {
            case 1:
                SceneManager.LoadScene(_sceneName.level1[_stageNum - 1]);
                break;
            case 2:
                SceneManager.LoadScene(_sceneName.level2[_stageNum - 1]);
                break;
            default:
                Debug.Log("GameManager.cs - Stage Num Error");
                break;
        }
        _isStartStage = true;
    }
	void AddMap(int stageNum)
	{
		string mapStr = "Map";
		string tempStr = "";

		for(int i = 0; i < _stageInfo[stageNum-1].stageEnableMapCount; i++)
		{
			tempStr = mapStr + _stageInfo[stageNum-1].stageEnableMapIdxs[i].ToString();
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
			startWp[i].tf = _stageInfo[stageNum-1].stageMapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotalManager").Find("StartPotal");
			startWp[i].wcScript = startWp[i].tf.GetComponentInParent<WarpCtrl>();

			endWp[i].tf = _stageInfo[stageNum-1].stageMapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotalManager").Find("EndPotal");
			endWp[i].wcScript = endWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

			mapCameraPos[i] = _stageInfo[stageNum-1].stageMapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
		}
		
		for(int i = 0; i < mapEnableArrCount; i++)
		{
			if(i == 0)
			{
				endWp[i].wcScript.ConnectedPotal(startWp[i+1].tf,mapCameraPos[i+1]);
			}
			else if(i == mapEnableArrCount - 1)
			{
				startWp[i].wcScript.ConnectedPotal(endWp[i-1].tf,mapCameraPos[i-1]);
			}
			else
			{
				startWp[i].wcScript.ConnectedPotal(endWp[i-1].tf,mapCameraPos[i-1]);
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

	public void MapWarpEvent(Vector2 CameraWarpPos)
	{
		_isMapWarp = true;
		_cameraWarpPos = CameraWarpPos;
	}
    // public void GamePause()
    // {
    //     Time.timeScale = 0;
    //     pausePage.SetActive(true);
    //     isPauseing = true;
    // }
    // public void GameResume() {
    //     Time.timeScale = 1.0f;
    //     pausePage.SetActive(false);
    //     isPauseing = false;
    // }

    // public void GameExit() {
    //     print("게임을 종료합니다.");
    //     UnityEditor.EditorApplication.isPlaying = false;
    //     //Application.Quit();
    // }
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