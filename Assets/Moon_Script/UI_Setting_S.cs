using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting_S : MonoBehaviour {

	int UI_number;
	bool UI_appear;
	Sprite ui_image;
	void Start () {
		
	}
	
	void Update () {
		Buy_UI();
	}


	void Buy_UI()
	{
		UI_number = GameObject.Find("Player_1").GetComponent<Player_S>().stop_land_number;
		UI_appear = GameObject.Find("Player_1").GetComponent<Player_S>().UI_appear;
		Debug.Log("In_Buy");
		if (UI_appear)
        {
			Debug.Log("In_Buy_if");
			ui_image = Resources.Load<Sprite>("2");
			this.gameObject.SetActive(UI_appear);
        }

    }
}
