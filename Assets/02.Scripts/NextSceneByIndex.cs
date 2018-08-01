using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneByIndex : MonoBehaviour {

    public void NextSecenOnClick(int sceneNumber) {
        SceneManager.LoadScene(sceneNumber);
    }
}
