using UnityEngine;
using System.Collections;

public class DemoScript : MonoBehaviour {

	public GameObject[] ParticleSystems;
	public int value = 0;
	public int prev = 0;
	public GUISkin skin;

	// Use this for initialization
	void Start () {


		//Hide_True ();
		Instantiate (ParticleSystems[value],transform.position,transform.rotation);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Hide_True(){

		//Destroy (ParticleSystems[prev]);
		Instantiate (ParticleSystems[value],transform.position,transform.rotation);
	}

	void OnGUI()
	{
		// Puts some basic buttons onto the screen.
		GUI.skin = skin;
		GUI.skin.button.fontSize = (int) (0.04f * Screen.height);
		GUI.skin.label.fontSize = (int) (0.025f * Screen.height);
		//GUI.skin.font.fontNames

		float unitX = Screen.width/100.0f;
		float unitY = Screen.height/100.0f;


		//----------------
		Rect F1 = new Rect(0.01f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);
		Rect F2 = new Rect(0.14f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);
		Rect F3 = new Rect(0.27f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);
		Rect F4 = new Rect(0.40f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);
		Rect F5 = new Rect(0.53f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);
		Rect F6 = new Rect(0.66f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);
		Rect F7 = new Rect(0.79f * Screen.width, 0.80f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);

		Rect F8 = new Rect(0.01f * Screen.width, 0.90f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);

		Rect F9 = new Rect(0.14f * Screen.width, 0.90f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);

		Rect F10 = new Rect(0.27f * Screen.width, 0.90f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);

		Rect L1 = new Rect(0.40f * Screen.width, 0.90f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);

		Rect L2 = new Rect(0.53f * Screen.width, 0.90f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);

		Rect P1 = new Rect(0.66f * Screen.width, 0.90f * Screen.height,
			0.12f * Screen.width, 0.07f * Screen.height);




		if (GUI.Button(F1,"Flower 1"))
		{
			
			value = 0;
			Hide_True ();
		}

		if (GUI.Button(F2,"Flower 2"))
		{

			
			value = 1;
			Hide_True ();
		}

		if (GUI.Button(F3,"Flower 3"))
		{

			
			value = 2;
			Hide_True ();
		}

		if (GUI.Button(F4,"Flower 4"))
		{

			
			value = 3;
			Hide_True ();
		}

		if (GUI.Button(F5,"Flower 5"))
		{

			
			value = 4;
			Hide_True ();
		}

		if (GUI.Button(F6,"Flower 6"))
		{

			
			value = 5;
			Hide_True ();
		}

		if (GUI.Button(F7,"Flower 7"))
		{

			
			value = 6;
			Hide_True ();
		}

		if (GUI.Button(F8,"Flower 8"))
		{

			
			value = 7;
			Hide_True ();
		}

		if (GUI.Button(F9,"Flower 9"))
		{

			
			value = 8;
			Hide_True ();
		}

		if (GUI.Button(F10,"Flower 10"))
		{
			
			value =9;
			Hide_True ();

		}

		if (GUI.Button(L1,"Leaf 1"))
		{

			
			value = 10;
			Hide_True ();
		}

		if (GUI.Button(L2,"Leaf 2"))
		{

			
			value = 11;
			Hide_True ();
		}

		if (GUI.Button(P1,"Petal 1"))
		{

			
			value = 12;
			Hide_True ();
		}



	}
}
