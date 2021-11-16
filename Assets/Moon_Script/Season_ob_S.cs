using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Season_ob_S : MonoBehaviour {

	public GameObject season_ob;
	private float ob_up_time;
	Vector3 season_ob_pos;
	void Start () {
		ob_up_time = 0f;
	}
	
	
	void Update () {

		if (ob_up_time < 1.5)
		{
			season_ob.transform.position = new Vector3(season_ob.transform.position.x, season_ob.transform.position.y + ob_up_time, season_ob.transform.position.z);
		
			Debug.Log(ob_up_time);
		}
		ob_up_time += Time.deltaTime;
	}
}
