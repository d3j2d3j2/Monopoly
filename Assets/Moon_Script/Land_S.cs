using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land_S : MonoBehaviour {

	float time1, time2;
	public GameObject ob1,ob2,ob3, ob4,ob5,ob6, ob7, ob8, ob9, ob10, ob11, ob12, ob13, ob14, ob15, ob16, ob17, ob18, ob19, ob20, ob21, ob22, ob23, ob24, ob25, ob26, ob27, ob28, ob29, ob30, ob31, ob32;

	void Start () {
		time1 = 0f;
		time2 = 0f;
	}
	
	void Update () {
		Up_land();
	}
	void Up_land()
    {
		time1 += Time.deltaTime;
        if (time1 > 1.5f && ob1.transform.position.y< -0.5f)
        {
			ob1.transform.position = new Vector3(ob1.transform.position.x, ob1.transform.position.y, ob1.transform.position.z);

        }
    }


}
