using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Camera_S : MonoBehaviour {

	Vector3 initial_pos;
	public float time;
	public float power;

	void Start () {
		initial_pos = this.transform.position;
	}
	
	void Update () {
		Camera_Shake();
	}

	void Camera_Shake()
    {
        if (time > 0f)
        {
			this.transform.position = Random.insideUnitSphere * power + initial_pos;
			time -= Time.deltaTime;
        }
        else
        {
			time = 0f;
			this.transform.position = initial_pos;
        }
    }

}
