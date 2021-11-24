using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buy_image_S : MonoBehaviour {

	private Image image;
	int UI_number;
	bool UI_appear;

	void Start () {
	
	}
	
	
	void Update () {
		
	}

	void Buy_UI()
	{
		UI_number = GameObject.Find("Player_1").GetComponent<Player_S>().stop_land_number;
		UI_appear = GameObject.Find("Player_1").GetComponent<Player_S>().UI_appear;

		if (UI_appear)
		{
			
			this.gameObject.SetActive(UI_appear);

		}

	}
}
