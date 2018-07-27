using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject pausePage;
    bool isPauseing = false;
    private void Start()
    {
        pausePage.SetActive(false);
    }
    void Update () {
        if (Input.GetButton("Cancel")&&!isPauseing) {
            GamePause();
        }
	}
    public void GamePause()
    {
        Time.timeScale = 0;
        pausePage.SetActive(true);
        isPauseing = true;
    }
    public void GameResume() {
        Time.timeScale = 1.0f;
        pausePage.SetActive(false);
        isPauseing = false;
    }

    public void GameExit() {
        print("게임을 종료합니다.");
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }
}
