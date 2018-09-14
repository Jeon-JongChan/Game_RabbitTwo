using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu 
{
    bool isPause = false;


    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            if (!isPause)
            {
                GamePause();
            }
            else {
                GameResume();
            }
        }
    }
    public void GameResume() {
        isPause = false;
        Time.timeScale = 1;
    }

	public void GamePause()
	{
		Time.timeScale = 0;
		isPause = true;
	}

	public void GameExit() {
		UnityEditor.EditorApplication.isPlaying = false;
		//Application.Quit();
	}
}
