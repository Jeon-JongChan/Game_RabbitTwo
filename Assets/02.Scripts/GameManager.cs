using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	struct WarpPotalSt
	{
		public Transform tf;
		public WarpCtrl wcScript;
	}
	/* INSPECTOR VARIABLE */
	[SerializeField] GameObject player;
	[SerializeField] GameObject mainCamera;
	[SerializeField] bool isSequential = false;
	List<GameObject> stage1MapList;
	List<GameObject> stage2MapList;
	List<GameObject> stage3MapList;

	int[] stage1EnableMapIdxs;
	int[] stage2EnableMapIdxs;
	int[] stage3EnableMapIdxs;

	//스테이지마다 활성화할 맵의 개수
	[SerializeField] int stage1EnableMapCount = 5;
	[SerializeField] int stage2EnableMapCount = 11;
	[SerializeField] int stage3EnableMapCount = 18;

	int stage1TotalMapCount = 9;
	int stage2TotalMapCount = 16;
	int stage3TotalMapCount = 25;

	bool isMapWarp = false;
	bool isStartStage = true;
	Vector2 cameraWarpPos = Vector2.zero;
	Vector2 startingPoint = Vector2.zero;

	static int _stageNum = 1;

	public GameObject pausePage;
    bool isPauseing = false;
	private void Start()
    {
        //pausePage.SetActive(false);
		mainCamera = GameObject.Find("Main Camera");
    }
	private void Awake() {
		stage1MapList = new List<GameObject>();
		stage2MapList = new List<GameObject>();
		stage3MapList = new List<GameObject>();

		stage1EnableMapIdxs = new int[stage1EnableMapCount];
		stage2EnableMapIdxs = new int[stage2EnableMapCount];
		stage3EnableMapIdxs = new int[stage3EnableMapCount];

		if(!isSequential)StartMapRandomIdx(1,stage1EnableMapCount,stage1TotalMapCount);
		else
		{
			for(int i = 0; i < stage1EnableMapCount; i++)
			{
				stage1EnableMapIdxs[i] = i+1;
			}
		}
		StartMapRandomIdx(2,stage2EnableMapCount,stage2TotalMapCount);
		StartMapRandomIdx(3,stage3EnableMapCount,stage3TotalMapCount);

		AddMap(_stageNum);
		ConnectMapWarp(_stageNum, stage1EnableMapCount);
	}

	private void Update() {
		// if (Input.GetButton("Cancel")&&!isPauseing) {
        //     GamePause();
        // }
		if(isMapWarp)
		{
			isMapWarp = false;
			Vector3 temp = new Vector3(cameraWarpPos.x,cameraWarpPos.y,mainCamera.transform.position.z);
			mainCamera.transform.position = temp;
			print(cameraWarpPos);
			cameraWarpPos = Vector2.zero;
		}

		if(isStartStage)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			player.transform.position =  startingPoint;
			isStartStage = false;
		}
	}
	void AddMap(int stageNum)
	{
		string mapStr = "Map";
		string tempStr = "";
		switch(stageNum)
		{
			case 1:
			for(int i = 0; i < stage1EnableMapCount; i++)
			{
				tempStr = mapStr + stage1EnableMapIdxs[i].ToString();
				stage1MapList.Add(GameObject.Find(tempStr));
			}
			break;
			case 2:
			for(int i = 0; i < stage2EnableMapCount; i++)
			{
				tempStr = mapStr + stage2EnableMapIdxs[i].ToString();
				stage2MapList.Add(GameObject.Find(tempStr));
			}
			break;
			case 3:
			for(int i = 0; i < stage3EnableMapCount; i++)
			{
				tempStr = mapStr + stage3EnableMapIdxs[i].ToString();
				stage3MapList.Add(GameObject.Find(tempStr));
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
					startWp[i].tf = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotal");
					startWp[i].wcScript = startWp[i].tf.GetComponent<WarpCtrl>();

					endWp[i].tf = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotal");
					endWp[i].wcScript = endWp[i].tf.gameObject.GetComponent<WarpCtrl>();

					mapCameraPos[i] = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
				}
			break;
			case 2:
				/* start와 end potal 을 모두 가져온다. 카메라 이동위치도 갖고 온다.*/
				for(int i = 0; i < mapEnableArrCount; i++)
				{
					startWp[i].tf = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotal");
					startWp[i].wcScript = startWp[i].tf.GetComponent<WarpCtrl>();

					endWp[i].tf = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotal");
					endWp[i].wcScript = endWp[i].tf.GetComponent<WarpCtrl>();

					mapCameraPos[i] = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
				}

			break;
			case 3:
				/* start와 end potal 을 모두 가져온다. 카메라 이동위치도 갖고 온다.*/
				for(int i = 0; i < mapEnableArrCount; i++)
				{
					startWp[i].tf = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("StartPotal");
					startWp[i].wcScript = startWp[i].tf.GetComponent<WarpCtrl>();

					endWp[i].tf = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("EndPotal");
					endWp[i].wcScript = endWp[i].tf.GetComponent<WarpCtrl>();

					mapCameraPos[i] = stage1MapList[i].transform.Find("_NeedNextMapMoving").Find("MapCameraPos");
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

		startingPoint = startWp[0].tf.position;
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
						if(rand == stage1EnableMapIdxs[j])
						{
							rand = Random.Range(2,mapTotalCount);
							j = 0;
							continue;
						}
					}
				}
				stage1EnableMapIdxs[i] = rand;
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
						if(rand == stage2EnableMapIdxs[j])
						{
							rand = Random.Range(2,mapTotalCount);
							j = 0;
							continue;
						}
					}
				}
				stage2EnableMapIdxs[i] = rand;
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
						if(rand == stage3EnableMapIdxs[j])
						{
							rand = Random.Range(2,mapTotalCount);
							j = 0;
							continue;
						}
					}
				}
				stage3EnableMapIdxs[i] = rand;
			}
			break;
		}
	}

	public void MapWarpEvent(Vector2 CameraWarpPos)
	{
		isMapWarp = true;
		cameraWarpPos = CameraWarpPos;
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
