using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
class StageSceneName
{
	public string[] level1 = new string[3];
	public string[] level2 = new string[3];
}
public class GameManager : MonoBehaviour {

	struct WarpPotalSt
	{
		public Transform tf;
		public WarpCtrl wcScript;
	}
	/* INSPECTOR VARIABLE */
	[SerializeField] GameObject _player;
	[SerializeField] GameObject _mainCamera;
	[SerializeField] bool _isSequential = false;
	//스테이지마다 활성화할 맵의 개수
	[SerializeField] int _stage1EnableMapCount = 5;
	[SerializeField] int _stage2EnableMapCount = 11;
	[SerializeField] int _stage3EnableMapCount = 18;
	[SerializeField] StageSceneName _sceneName = new StageSceneName();
    [SerializeField] float _dieDelayTime = 1.5f;
    public GameObject _pausePage;

    /* NEEDS COMPONENT */
    Player _playerInstance;

	List<GameObject> _stage1MapList;
	List<GameObject> _stage2MapList;
	List<GameObject> _stage3MapList;

	int[] _stage1EnableMapIdxs;
	int[] _stage2EnableMapIdxs;
	int[] _stage3EnableMapIdxs;


	int _stage1TotalMapCount = 9;
	int _stage2TotalMapCount = 16;
	int _stage3TotalMapCount = 25;

	bool _isMapWarp = false;
	bool _isStartStage = true;
	Vector2 _cameraWarpPos = Vector2.zero;
	Vector2 _startingPoint = Vector2.zero;

	static int _stageNum = 1;
    static int _mapNum = 1;

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

		_stage1MapList = new List<GameObject>();
		_stage2MapList = new List<GameObject>();
		_stage3MapList = new List<GameObject>();

		_stage1EnableMapIdxs = new int[_stage1EnableMapCount];
		_stage2EnableMapIdxs = new int[_stage2EnableMapCount];
		_stage3EnableMapIdxs = new int[_stage3EnableMapCount];

		if(!_isSequential)
		{
			StartMapRandomIdx(1,_stage1EnableMapCount,_stage1TotalMapCount);
			StartMapRandomIdx(2,_stage2EnableMapCount,_stage2TotalMapCount);
			StartMapRandomIdx(3,_stage3EnableMapCount,_stage3TotalMapCount);
		}
		else
		{
			for(int i = 0; i < _stage1EnableMapCount; i++)
			{
				_stage1EnableMapIdxs[i] = i+1;
			}
		}

		AddMap(_stageNum);
		ConnectMapWarp(_stageNum, _stage1EnableMapCount);
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
		switch(stageNum)
		{
			case 1:
			for(int i = 0; i < _stage1EnableMapCount; i++)
			{
				tempStr = mapStr + _stage1EnableMapIdxs[i].ToString();
				_stage1MapList.Add(GameObject.Find(tempStr));
			}
			break;
			case 2:
			for(int i = 0; i < _stage2EnableMapCount; i++)
			{
				tempStr = mapStr + _stage2EnableMapIdxs[i].ToString();
				_stage2MapList.Add(GameObject.Find(tempStr));
			}
			break;
			case 3:
			for(int i = 0; i < _stage3EnableMapCount; i++)
			{
				tempStr = mapStr + _stage3EnableMapIdxs[i].ToString();
				_stage3MapList.Add(GameObject.Find(tempStr));
			}
			break;
		}
	}
	void ConnectMapWarp(int stageNum, int mapEnableArrCount)
	{
		WarpPotalSt[] startWp = new WarpPotalSt[mapEnableArrCount];
		WarpPotalSt[] endWp = new WarpPotalSt[mapEnableArrCount];
		Transform[] mapCameraPos = new Transform[mapEnableArrCount];

		switch(stageNum)
		{
			case 1:
				/* start와 end potal 을 모두 가져온다. 카메라 이동위치도 갖고 온다.*/
				for(int i = 0; i < mapEnableArrCount; i++)
				{
					startWp[i].tf = _stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotalManager").Find("StartPotal");
					startWp[i].wcScript = startWp[i].tf.GetComponentInParent<WarpCtrl>();

					endWp[i].tf = _stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotalManager").Find("EndPotal");
					endWp[i].wcScript = endWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

					mapCameraPos[i] = _stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
				}
			break;
			case 2:
				/* start와 end potal 을 모두 가져온다. 카메라 이동위치도 갖고 온다.*/
				for(int i = 0; i < mapEnableArrCount; i++)
				{
					startWp[i].tf = _stage2MapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotalManager").Find("StartPotal");
					startWp[i].wcScript = startWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

					endWp[i].tf = _stage2MapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotalManager").Find("EndPotal");;
					endWp[i].wcScript = endWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

					mapCameraPos[i] = _stage2MapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
				}

			break;
			case 3:
				/* start와 end potal 을 모두 가져온다. 카메라 이동위치도 갖고 온다.*/
				for(int i = 0; i < mapEnableArrCount; i++)
				{
					startWp[i].tf = _stage3MapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotalManager").Find("StartPotal");
					startWp[i].wcScript = startWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

					endWp[i].tf = _stage3MapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotalManager").Find("EndPotal");
					endWp[i].wcScript = endWp[i].tf.parent.GetComponentInParent<WarpCtrl>();

					mapCameraPos[i] = _stage3MapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
				}
			break;
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

	void StartMapRandomIdx(int stageNum, int mapEnableArrCount, int mapTotalCount)
	{
		int rand = 0;
		switch(stageNum)
		{
			case 1:
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
						if(rand == _stage1EnableMapIdxs[j])
						{
							rand = Random.Range(2,mapTotalCount);
							j = 0;
							continue;
						}
					}
				}
				_stage1EnableMapIdxs[i] = rand;
			}
			break;
			case 2:
			for(int i = 0; i < mapEnableArrCount; i++)
			{
				if(i == 0)
				{
					rand = 1;
				}
				else
				{
					for(int j = 0; j < i;j++)
					{
						if(rand == _stage2EnableMapIdxs[j])
						{
							rand = Random.Range(2,mapTotalCount);
							j = 0;
							continue;
						}
					}
				}
				_stage2EnableMapIdxs[i] = rand;
			}
			break;
			case 3:
			for(int i = 0; i < mapEnableArrCount; i++)
			{
				if(i == 0)
				{
					rand = 1;
				}
				else
				{
					for(int j = 0; j < i;j++)
					{
						if(rand == _stage3EnableMapIdxs[j])
						{
							rand = Random.Range(2,mapTotalCount);
							j = 0;
							continue;
						}
					}
				}
				_stage3EnableMapIdxs[i] = rand;
			}
			break;
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
