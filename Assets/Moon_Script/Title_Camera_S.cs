using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Title_Camera_S : MonoBehaviour {

	float time,time2;
	public GameObject text;

	void Start () {
		this.transform.position = new Vector3(0,0,-7);
		Debug.Log(this.transform.position);
		
	}
	
	
	void Update () {
		if (-7 + time + time2 * 1.5f <= 3.2)
		{
			this.transform.position = new Vector3(0, 0, -7 + time + time2*1.5f);
			time2 += Time.deltaTime;
		}

		if (this.transform.position.z >= 3)
		{
			Debug.Log("In");
			text.SetActive(true);
			if (Input.GetMouseButtonDown(0))
			{
				SceneManager.LoadScene("Lobby");
			}
		}

		time += Time.deltaTime;
	}
}
