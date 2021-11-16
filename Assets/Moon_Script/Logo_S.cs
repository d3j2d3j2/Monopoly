using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Logo_S : MonoBehaviour {

	float time;
	float time2;
	
	void Start () {
		this.transform.localScale = Vector3.zero;
		time2 = 0f;
	}
	
	
	void Update () {
		time += Time.deltaTime;
		
		if(time> 4f && time2<=2f)
        {
			this.transform.localScale = Vector3.one;
			this.transform.localScale = Vector3.one * (1 * time2);
			time2 += Time.deltaTime;
		}
        if (time > 6.5f)
        {
			Debug.Log("넘어가라!");
			SceneManager.LoadScene("Title");
		}
	}
}
