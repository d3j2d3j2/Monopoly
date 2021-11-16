using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_S : MonoBehaviour {

	float time1, time2;
	public GameObject Main_Camera;
	public GameObject Title_Image;
	public GameObject next_text;

	void Start () {
		Main_Camera.transform.position = new Vector3(0, 1, -10);
	}
	
	
	void Update () {

		Camera_Move();

	}

	void Camera_Move()
    {
		if (-10 + time1 + time2 * 1.5f <= 3.2)
		{
			this.transform.position = new Vector3(0, 1, -10 + time1 + time2 * 1.5f);
			time2 += Time.deltaTime;
		}

		if (this.transform.position.z >= 3)
		{
			Debug.Log("In");
			next_text.SetActive(true);
			if (Input.GetMouseButtonDown(0))
			{
				SceneManager.LoadScene("Lobby");
			}
		}

		time1 += Time.deltaTime;
	}
}
