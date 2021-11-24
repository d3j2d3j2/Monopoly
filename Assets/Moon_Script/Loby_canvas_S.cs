using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Loby_canvas_S : MonoBehaviour {

	public GameObject img;

	void Start () {
		Debug.Log("0");
		img.SetActive(false);
	}
	
	void Update () {
		
	}

	public void Game_Start()
    {
		SceneManager.LoadScene("Monopoly");
	}

	public void Setting_UI()
	{
		Debug.Log("1");
		img.SetActive(true);
	}
	public void Setting_UI_X()
	{
		Debug.Log("2");
		img.SetActive(false);
	}
}


