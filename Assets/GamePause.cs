using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour {
    [SerializeField]
    GameObject pausePage;
    bool isPause = false;


    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            if (!isPause)
            {
                isPause = true;
                Time.timeScale = 0f;
                pausePage.SetActive(true);
            }
            else {
                isPause = false;
                Time.timeScale = 1;
                pausePage.SetActive(false);
            }
        }
    }
   
}
