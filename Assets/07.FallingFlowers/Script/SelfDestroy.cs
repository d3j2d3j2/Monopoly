using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {

	public float SelfDestroyTime = 10.0f;

	// Use this for initialization
	void Start () {
	
		StartCoroutine ("SelfDestroyGameobject");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SelfDestroyGameobject(){

		yield return new WaitForSeconds (SelfDestroyTime);
		Destroy (gameObject);
	}
}
