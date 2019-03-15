using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 开始按钮的功能
/// </summary>
public class LoadGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(LoadGameScene);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
