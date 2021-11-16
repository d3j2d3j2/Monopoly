using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land_S : MonoBehaviour {

	float time1, time2;
	public GameObject event_land;
	//public GameObject normal_land;

	void Start () {
		time1 = 0f;
		time2 = 0f;
	}
	
	void Update () {
		Event_land();
	}

	void Event_land()
    {
		if (time1 <= 1.5f)
		{
			Debug.Log(-1.5f + time1);
			event_land.transform.position = new Vector3(event_land.transform.position.x, -1.5f + time1+3f, event_land.transform.position.z);
		}

		time1 += Time.deltaTime;
	}
}
