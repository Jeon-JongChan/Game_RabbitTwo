using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIntro : MonoBehaviour {
	[SerializeField] GameManager _gameManager;
	[SerializeField] GameObject _introPanel;
	[SerializeField] GameObject _settingPanel;
	
	public void OnStartGame()
	{
		//_gameManager.ChaneStage();
	}

	public void OnSetting()
	{
		_introPanel.SetActive(false);
		_settingPanel.SetActive(true);
	}

	public void OnExitButton()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

}
