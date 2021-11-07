using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public List<int> ownLand;
	public const int StartMoney = 1000000;
	public int currentMoney;
	public int position;
	public Map map;
	public Monopoly.PlayerType playerType;
	public int isolatedCount = 0;
	// Use this for initialization
	void Start()
	{
		map = GameObject.Find("Map").GetComponent<Map>();
		Reset();
	}

	// Update is called once per frame
	void Update()
	{

	}
	public void Reset()
	{
		currentMoney = StartMoney;
		ownLand = new List<int>();
		position = 0;
	}
	public int Property()
	{
		int sum = 0;

		foreach (var land in map.landArray)
		{
			if (land.owner == playerType)
			{
				for (int i = 0; i < land.build.Length; i++)
				{
					if (land.build[i] == true)
					{
						sum += land.price[i];
					}
				}
			}
		}

		return sum;
	}

	public int TotalWealth()
	{
		return currentMoney + Property();
	}
}
