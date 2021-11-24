using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_S : MonoBehaviour {

	// 시작할 때와 UI X 버튼 혹은 구매 버튼 (+ 인수버튼 등)을 누르면 Random으로 2~12만큼 숫자 생성(다이스 임의 생성).
	// 
	int dice_number, move_number;
	private GameObject next;
	int land_number;
	string land_name;
	bool t;
	public int stop_land_number;
	public bool UI_appear;

	void Start () {
		land_number = 0;
		//next.transform.position =
		this.transform.position =
			new Vector3(GameObject.Find("0").transform.position.x, 8.5f, GameObject.Find("0").transform.position.z);
		Debug.Log("시작");

		t = true;
		UI_appear = false;
	}
	
	void Update () {
		if (t)
		{
			Dice_RoLL();
			t = false;
			
		}
		Player_Move();
		Stop_Check();
		Debug.Log("트루?" + UI_appear);
	}

	//주사위 던지기
	void Dice_RoLL()
    {
		dice_number = Random.Range(17,19);
		Debug.Log("--던짐 주사위수---:" + dice_number);
		move_number = dice_number;
		stop_land_number += dice_number;
		if(stop_land_number > 31)
        {
			stop_land_number -= 32;
        }
		//Debug.Log("스탑랜드:" + stop_land_number);
	}

	//주사위 나온 수만큼 player 이동
	void Player_Move()
    {
		land_name = (land_number).ToString();
		next = GameObject.Find(land_name);
		if (move_number != -1)
        {
			if (this.transform.position == new Vector3(next.transform.position.x, 8.5f, next.transform.position.z))
			{
				move_number--;
				land_number++;
                if (land_number > 31)
                {
					land_number = land_number - 32;
                }
				Debug.Log("?:" + land_number);
			}
			else
			{
				this.transform.position =
						Vector3.MoveTowards(this.transform.position, new Vector3(next.transform.position.x, 8.5f, next.transform.position.z), Time.deltaTime * 5f);

			}
		}

	}

	// Player 0,8,16,24 지역에서 rotation
	void OnTriggerEnter(Collider col) 
    {
		if(col.tag == "0" || col.tag == "8"|| col.tag == "16"|| col.tag == "24")
        {
			//Debug.Log("회전해라");
			this.transform.Rotate(new Vector3(0,90,0));
        }
    }

	void Stop_Check()
    {
		
		if(this.transform.position.x == GameObject.Find(stop_land_number.ToString()).transform.position.x 
			&& this.transform.position.z == GameObject.Find(stop_land_number.ToString()).transform.position.z)
        {
			Debug.Log("멈춰!" + stop_land_number);
			UI_appear = true;
			//t = true;
			
		}
		
	}

}
