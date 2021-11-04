using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public enum LandType
	{
		Usual,
		GoldKey,
		Casino,
		Home,
		Isolated,
		Olympic,
		Airport
	}

	private const char USUAL = 'u';
	private const char GOLDKEY = 'g';
	private const char CASINO = 'c';
	private const char HOME = 'h';
	private const char ISOLATED = 'i';
	private const char OLYMPIC = 'o';
	private const char AIRPORT = 'a';

	public class Land
	{
		public LandType type = LandType.Usual;
		public Monopoly.PlayerType owner = Monopoly.PlayerType.Player2;
		public bool[] build = { true, false, false, false };
		public int[] price = { 0, 0, 0, 0 };
		public int Getfee()
		{
			int sum = 0;
			if (build[0]) sum += price[0];
			if (build[1]) sum += price[1];
			if (build[2]) sum += price[2];
			if (build[3]) sum += price[3];
			return sum / 2;
		}
		public int GetCurTotalPrice()
		{
			int sum = 0;
			for (int i = 0; i < price.Length; i++)
			{
				if (build[i])
				{
					sum += price[i];
				}
			}
			return sum;
		}
	}

	public Land[] landArray;
	public int mapSize;
	public TextAsset mapAsset;

	public void LoadFromAsset()
	{
		if (mapAsset == null)
		{
			Debug.LogError("No Map Data Asset.");
			return;
		}

		string textData = mapAsset.text;
		var option = System.StringSplitOptions.RemoveEmptyEntries;
		var lines = textData.Split(new char[] { '\r', '\n' }, option);

		var spliter = new char[] { ',' };

		mapSize = lines.Length;

		landArray = new Land[mapSize];

		for (int i = 0; i < mapSize; i++)
		{
			landArray[i] = new Land();
			var data = lines[i].Split(spliter, option);
			LandType landType;
			switch (data[0][0])
			{
				case 'u':
					landType = LandType.Usual;
					break;
				case 'g':
					landType = LandType.GoldKey;
					break;
				case 'c':
					landType = LandType.Casino;
					break;
				case 'h':
					landType = LandType.Home;
					break;
				case 'i':
					landType = LandType.Isolated;
					break;
				case 'o':
					landType = LandType.Olympic;
					break;
				case 'a':
					landType = LandType.Airport;
					break;
				default:
					landType = LandType.Usual;
					break;
			}
			landArray[i].type = landType;

			if (landType == LandType.Usual)
			{
				landArray[i].price[0] = int.Parse(data[1]);
				landArray[i].price[1] = int.Parse(data[2]);
				landArray[i].price[2] = int.Parse(data[3]);
				landArray[i].price[3] = int.Parse(data[4]);
			}
		}
		landArray[1].owner = Monopoly.PlayerType.Player1;
	}
}