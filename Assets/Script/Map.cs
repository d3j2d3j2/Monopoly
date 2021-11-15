using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public enum LandType
	{
		Usual,
		GoldKey,
		Festival,
		Home,
		Isolated,
		Olympic,
		Airport
	}

	public enum MonopolyType
    {
		None,
		Season,
		Region3,
		Festival,
		Turnover
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
		public Monopoly.PlayerType owner = Monopoly.PlayerType.None;
		public bool[] build = { false, false, false, false };
		public int[] price = { 0, 0, 0, 0 };
		
		public int landNum = -1;
		public int GetFee()
		{
			int sum = 0;
			for(int i=0; i<build.Length; i++)
            {
				if (build[i]) sum += price[i];
            }
			if (type == LandType.Usual && IsRegionMonopoly(Region())) sum *= 2;
			if(landNum == Monopoly.olympicLand) sum *= Monopoly.olympicScaler;
			return sum;
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

		public int NumBuild()
        {
			int sum = 0;
			for(int i=0; i<build.Length; i++)
            {
				if(build[i])
                {
					sum += 1;
                }
            }
			return sum;
        }

		public int Season()
		{
			return landNum / (Map.mapSize/4);
		}
		
		public int Region()
        {
			if (type != LandType.Usual) return -1;
			else return landNum / (Map.mapSize / 8);
        }
	}
	

	public static Land[] landArray;
	public static int mapSize;
	public TextAsset mapAsset;

	public int NumLandSeason(int season)
	{
		int sum = 0;
		for (int i = 0; i < Map.mapSize; i++)
		{
			if ( (Map.landArray[i].type == LandType.Usual || Map.landArray[i].type == LandType.Festival)
				&& Map.landArray[i].Season() == season)
			{
				sum++;
			}
		}
		return sum;
	}

	public Monopoly.PlayerType WhoMonopolySeason(int season)
    {
		int sum1 = 0;
		int sum2 = 0;
		for(int i= season * (Map.mapSize / 4); i<season*(Map.mapSize/4) + Map.mapSize/4; i++)
        {
			if (Map.landArray[i].owner == Monopoly.PlayerType.Player1) sum1++;
			else if (Map.landArray[i].owner == Monopoly.PlayerType.Player2) sum2++;
		}
		if (sum1 == NumLandSeason(season)) return Monopoly.PlayerType.Player1;
		else if (sum2 == NumLandSeason(season)) return Monopoly.PlayerType.Player2;
		else return Monopoly.PlayerType.None;
    }
	public Monopoly.PlayerType AnySeasonMonopoly()
    {
		for(int i=0; i<4; i++)
        {
			if (WhoMonopolySeason(i) == Monopoly.PlayerType.Player1) return Monopoly.PlayerType.Player1;
			else if (WhoMonopolySeason(i) == Monopoly.PlayerType.Player2) return Monopoly.PlayerType.Player2;
		}
		return Monopoly.PlayerType.None;
    }

	public static int NumLandRegion(int region)
    {
		int sum = 0;
		for (int i = 0; i < mapSize; i++)
		{
			if ((Map.landArray[i].type == LandType.Usual)
				&& Map.landArray[i].Region() == region)
			{
				sum++;
			}
		}
		return sum;
	}

	public static bool IsRegionMonopoly(int region)
    {
		if (WhoMonopolyRegion(region) != Monopoly.PlayerType.None) return true;
		else return false;
    }

	public static Monopoly.PlayerType WhoMonopolyRegion(int region)
    {
		int sum1 = 0;
		int sum2 = 0;
		for(int i= region*(mapSize/8); i< region*(mapSize/8)+mapSize/8; i++)
        {
			if (Map.landArray[i].owner == Monopoly.PlayerType.Player1) sum1++;
			else if (Map.landArray[i].owner == Monopoly.PlayerType.Player2) sum2++;
		}
		if (sum1 == NumLandRegion(region)) return Monopoly.PlayerType.Player1;
		else if (sum2 == NumLandRegion(region)) return Monopoly.PlayerType.Player2;
		else return Monopoly.PlayerType.None;
	}

	public Monopoly.PlayerType WhoMonopoly3Region()
    {
		int sum1 = 0;
		int sum2 = 0;
		for(int i=0; i<8; i++)
        {
			if (WhoMonopolyRegion(i) == Monopoly.PlayerType.Player1) sum1++;
			else if (WhoMonopolyRegion(i) == Monopoly.PlayerType.Player2) sum2++;
		}
		if (sum1 >= 3) return Monopoly.PlayerType.Player1;
		else if (sum2 >= 3) return Monopoly.PlayerType.Player2;
		else return Monopoly.PlayerType.None;
	}

	public int NumFestival()
    {
		int sum = 0;
		for(int i=0; i<mapSize; i++)
        {
			if (landArray[i].type == LandType.Festival) sum++;
        }
		return sum;
    }

	public Monopoly.PlayerType WhoMonopolyFestival()
    {
		int sum1 = 0;
		int sum2 = 0;
		for(int i=0; i<mapSize; i++)
        {
			if(landArray[i].type == LandType.Festival)
            {
				if (landArray[i].owner == Monopoly.PlayerType.Player1) sum1++;
				else if (landArray[i].owner == Monopoly.PlayerType.Player2) sum2++;
			}
        }
		if (sum1 == NumFestival()) return Monopoly.PlayerType.Player1;
		else if (sum2 == NumFestival()) return Monopoly.PlayerType.Player2;
		else return Monopoly.PlayerType.None;
	}

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

		Map.landArray = new Land[mapSize];

		for (int i = 0; i < mapSize; i++)
		{
			Map.landArray[i] = new Land();
			Map.landArray[i].landNum = i;
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
				case 'f':
					landType = LandType.Festival;
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
			Map.landArray[i].type = landType;

			if (landType == LandType.Usual)
			{
				Map.landArray[i].price[0] = int.Parse(data[1]);
				Map.landArray[i].price[1] = int.Parse(data[2]);
				Map.landArray[i].price[2] = int.Parse(data[3]);
				Map.landArray[i].price[3] = int.Parse(data[4]);
			}
			else if(landType == LandType.Festival)
            {
				Map.landArray[i].build = new bool[1];
				Map.landArray[i].build[0] = false;
				Map.landArray[i].price = new int[1];
				Map.landArray[i].price[0] = int.Parse(data[1]);
            }
            else
            {
				Map.landArray[i].build = null;
				Map.landArray[i].price = null;
            }
		}
	}
}