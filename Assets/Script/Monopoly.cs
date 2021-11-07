using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Monopoly : MonoBehaviour
{

	// 게임 진행 상황.
	private enum GameProgress
	{
		None = 0,       // 시합 시작 전.
		Ready,          // 시합 시작 신호 표시.
		Turn,           // 시합 중.
		Move,
		Sell,
		Pay,           // 땅값을 지불
		Acquisit,       //땅을 인수
		Buy,            //땅을 삼
		Home,
		Isolated,
		Olympic,
		Airport,
		Result,         // 결과 표시.
		GameOver,       // 게임 종료.
		Disconnect,     // 연결 끊기.
	};

	// 턴 종류.
	private enum Turn
	{
		Own = 0,        // 자신의 턴.
		Opponent,       // 상대의 턴.
	};

	// 마크.
	public enum PlayerType
	{
		Player1 = 0,     // ○.
		Player2,          // ×.
		None
	};

	// 시합 결과.
	private enum Winner
	{
		None = 0,       // 시합 중.
		Player1,         // ○승리.
		Player2,          // ×승리.
		Tie,            // 무승부.
	};

	// 칸의 수.
	private const int rowNum = 3;

	// 시합 시작 전의 신호표시 시간.
	private const float waitTime = 1.0f;

	// 대기 시간.
	private const float turnTime = 10.0f;

	// 배치된 기호를 보존.
	private int[] spaces = new int[rowNum * rowNum];

	// 진행 상황.
	private GameProgress progress;

	// 현재의 턴.
	private PlayerType turn;

	// 로컬 기호.
	private PlayerType localPlayer;

	// 리모트 기호.
	private PlayerType remotePlayer;
	public GameObject turnPlayer;
	public Player turnPlayerScript;

	// 남은 시간.
	private float timer;

	// 승자.
	private Winner winner;

	// 게임 종료 플래그.
	private bool isGameOver;

	// 대기 시간.
	private float currentTime;

	// 네트워크.
	private TransportTCP m_transport = null;

	// 카운터.
	private float step_count = 0.0f;

	//
	// 텍스처 관련.
	//

	// 동그라미 텍스처.
	public GUITexture circleTexture;

	// .
	public GUITexture crossTexture;

	// .
	public GUITexture fieldTexture;

	public GUITexture youTexture;

	public GUITexture winTexture;

	public GUITexture loseTexture;

	// 사운드.
	public AudioClip se_click;
	public AudioClip se_setMark;
	public AudioClip se_win;

	private static float SPACES_WIDTH = 400.0f;
	private static float SPACES_HEIGHT = 400.0f;

	private static float WINDOW_WIDTH = 640.0f;
	private static float WINDOW_HEIGHT = 480.0f;

	public bool isDiceRolled;
	public int diceValue;

	public float moveTime;
	public const float OneMoveTime = 0.5f;
	public int moveCount;

	public TextAsset defaultMap;

	public GameObject map;
	public Map mapScript;
	public int mapSize;
	public GameObject player1;
	public GameObject player2;

	public bool isConfirmed;
	public bool[] sellingLands;
	public bool isBankrupt;
	public int infoTransCount;

	public bool willAcquisit;

	public bool[] buyingBuild;

	public float delayTime;

	// Use this for initialization
	void Start()
	{
		// Network 클래스의 컴포넌트 가져오기.
		GameObject obj = GameObject.Find("Network");
		m_transport = obj.GetComponent<TransportTCP>();
		if (m_transport != null)
		{
			m_transport.RegisterEventHandler(EventCallback);
		}

		// 게임을 초기화합니다.
		Reset();
		isGameOver = false;
		timer = turnTime;
	}

	// Update is called once per frame
	void Update()
	{
		switch (progress)
		{
			case GameProgress.Ready:
				UpdateReady();
				break;
			case GameProgress.Turn:
				UpdateTurn();
				break;
			case GameProgress.Move:
				UpdateMove();
				break;
			case GameProgress.Sell:
				UpdateSell();
				break;
			case GameProgress.Pay:
				UpdatePay();
				break;
			case GameProgress.Acquisit:
				UpdateAcquisit();
				break;
			case GameProgress.Buy:
				UpdateBuy();
				break;
			case GameProgress.Home:
				UpdateHome();
				break;
			case GameProgress.Isolated:
				UpdateIsolated();
				break;
			case GameProgress.Olympic:
				UpdateOlympic();
				break;
			case GameProgress.Airport:
				UpdateAirport();
				break;
			case GameProgress.GameOver:
				UpdateGameOver();
				break;
		}
	}

	// 
	void OnGUI()
	{
		switch (progress)
		{
			case GameProgress.Ready:
				// 필드와 기호를 그립니다.

				break;
			case GameProgress.Turn:
				Display();
                // 필드와 기호를 그립니다.
				// 남은 시간을 그립니다.
				if (turn == localPlayer)
				{
					if (turnPlayerScript.isolatedCount > 0)
					{
						GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 300, 20), "자동차 사고로 쉽니다. 남은 턴: " + (turnPlayerScript.isolatedCount - 1));
					}
                    else
                    {
						DrawTime();
						RollDice();
					}
				}
				
				break;

			case GameProgress.Move:
				Display();
				break;

			case GameProgress.Sell:
				Display();
				mapScript = map.GetComponent<Map>();
				Player seller = turnPlayerScript;
				if (turn == localPlayer)
				{
					for (int i = 0; i < mapSize; i++)
					{
						if (mapScript.landArray[i].owner == turn)
						{
							sellingLands[i] = GUI.Toggle(new Rect(Screen.width / 2, 20 + i * 20, 100, 30), sellingLands[i], " " + i + ":" + mapScript.landArray[i].GetCurTotalPrice());
						}
					}
					int needMoney = mapScript.landArray[seller.position].Getfee() - seller.currentMoney;
					if (needMoney > 0)
						GUI.Label(new Rect(Screen.width / 2, 20 + mapSize * 20, 100, 30), "부족금액 = " + needMoney);
					isConfirmed = GUI.Button(new Rect(Screen.width / 2, 20 + mapSize * 20 + 20, 100, 30), "매각 승인");
				}
				
				break;
			case GameProgress.Acquisit:
				Display();
				Player acquisiter = turnPlayerScript;
				if (turn == localPlayer)
				{
					if (mapScript.landArray[turnPlayerScript.position].owner != PlayerType.None)
					{
						if (turn != mapScript.landArray[turnPlayerScript.position].owner)
						{
							if (acquisiter.currentMoney >= mapScript.landArray[acquisiter.position].GetCurTotalPrice() * 2)
							{
								GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 - 40, 100, 20), "인수비용: " + (mapScript.landArray[turnPlayerScript.position].GetCurTotalPrice() * 2));
								willAcquisit = GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 2, 200, 20), willAcquisit, "인수하려면 체크");
							}
						}
					}
					isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 40, 100, 20), "승인");
				}
				
				break;

			case GameProgress.Buy:
				Display();
				if (turn == localPlayer)
                {
					Map.Land land = mapScript.landArray[turnPlayerScript.position];
					for(int i=0; i<land.build.Length; i++)
                    {
                        if (!land.build[i])
                        {
							buyingBuild[i] = GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 2 + i*20, 100, 20), buyingBuild[i], "건물 " + i);
                        }
                    }
                    if (BuyingMoney() <= turnPlayerScript.currentMoney)
                    {
						isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + land.build.Length * 20, 100, 20),"구매 승인");
                    }
                }
				
				break;
			case GameProgress.Home:
				Display();
				break;
			case GameProgress.Pay:
				Display();
				break;
			case GameProgress.Isolated:
				Display();
				break;
			case GameProgress.Olympic:
				Display();
				break;
			case GameProgress.Airport:
				Display();
				break;

			case GameProgress.Result:
				// 필드와 기호를 그립니다.

				// 승자를 표시합니다.
				DrawWinner();
				// 종료 버튼을 표시합니다.
				{
					GUISkin skin = GUI.skin;
					GUIStyle style = new GUIStyle(GUI.skin.GetStyle("button"));
					style.normal.textColor = Color.white;
					style.fontSize = 25;

					if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 100), "끝", style))
					{
						progress = GameProgress.GameOver;
						step_count = 0.0f;
					}
				}
				break;

			case GameProgress.GameOver:
				// 필드와 기호를 그립니다.

				// 승자를 표시합니다.
				DrawWinner();
				break;

			case GameProgress.Disconnect:
				// 필드와 기호를 그립니다.

				// 연결 끊김을 통지합니다.
				NotifyDisconnection();
				break;

			default:
				break;
		}

	}

	void UpdateReady()
	{
		// 시합 시작 신호 표시를 기다립니다.
		currentTime += Time.deltaTime;

		if (currentTime > waitTime)
		{
			//BGM 재생 시작.
			GameObject bgm = GameObject.Find("BGM");
			bgm.GetComponent<AudioSource>().Play();

			// 표시가 끝나면 게임 시작입니다.
			progress = GameProgress.Turn;
		}
	}

	void UpdateTurn()
	{
		if (turnPlayerScript.isolatedCount > 0)
		{
			delayTime += Time.deltaTime;
			if (delayTime > 2)
			{
				turnPlayerScript.isolatedCount--;
				delayTime = 0;
				ChangeTurn();
				progress = GameProgress.Turn;
			}
		}
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnTurn();
		}
		else
		{
			setMark = DoOpponentTurn();
		}

		if (setMark == false)
		{
			// 놓을 곳을 검토 중입니다.	
			return;
		}
		else
		{
			//기호가 놓이는 사운드 효과를 냅니다. 
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = se_setMark;
			audio.Play();
		}
		if (setMark)
			progress = GameProgress.Move;
	}

	// 자신의 턴일 때의 처리.
	bool DoOwnTurn()
	{
		//diceValue = 0;

		timer -= Time.deltaTime;
		if (timer <= 0.0f)
		{
			// 타임오버.
			timer = 0.0f;
			diceValue = UnityEngine.Random.Range(2, 12);
		}
		else
		{
			if (isDiceRolled == true)
			{
				diceValue = UnityEngine.Random.Range(2, 12);
			}
			else
				return false;
		}

		Debug.Log("Dice Value: " + diceValue);

		// 선택한 칸의 정보를 송신합니다.
		byte[] buffer = new byte[sizeof(int)];
		buffer = BitConverter.GetBytes(diceValue);
		m_transport.Send(buffer, buffer.Length);
		isDiceRolled = false;
		return true;
	}

	// 상대의 턴일 때의 처리.
	bool DoOpponentTurn()
	{
		// 상대의 정보를 수신합니다.
		byte[] buffer = new byte[sizeof(int)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);

		if (recvSize <= 0)
		{
			// 아직 수신되지 않았습니다.
			return false;
		}

		// 서버라면 ○ 클라이언트라면 ×를 지정합니다.
		//Mark mark = (m_network.IsServer() == true)? Mark.Cross : Mark.Circle;

		// 수신한 정보를 선택된 칸으로 변환합니다. 
		diceValue = BitConverter.ToInt32(buffer, 0);

		Debug.Log("Recv:" + diceValue + " [" + m_transport.IsServer() + "]");

		return true;
	}

	void UpdateMove()
	{
		if (moveCount < diceValue)
		{
			moveTime += Time.deltaTime;
			if (moveTime >= OneMoveTime)
			{
				moveCount++;
				if (turn == PlayerType.Player1)
					player1.GetComponent<Player>().position = (player1.GetComponent<Player>().position + 1) % map.GetComponent<Map>().mapSize;
				else if (turn == PlayerType.Player2)
					player2.GetComponent<Player>().position = (player2.GetComponent<Player>().position + 1) % map.GetComponent<Map>().mapSize;
				moveTime = 0f;
			}
		}
		else
		{
			timer = turnTime;
			moveTime = 0;
			moveCount = 0;

			int pos;
			if (turn == PlayerType.Player1)
				pos = player1.GetComponent<Player>().position;
			else
				pos = player2.GetComponent<Player>().position;
			Map.LandType landType = map.GetComponent<Map>().landArray[pos].type;

			if (landType == Map.LandType.Usual)
			{
				ResetSell();
				progress = GameProgress.Sell;
			}
			if (landType == Map.LandType.Home) progress = GameProgress.Home;
			if (landType == Map.LandType.Isolated) progress = GameProgress.Isolated;
			if (landType == Map.LandType.Olympic) progress = GameProgress.Olympic;
			if (landType == Map.LandType.Airport) progress = GameProgress.Airport;
		}
	}

	void ResetSell()
	{
		isBankrupt = false;
		isConfirmed = false;
		infoTransCount = 0;
		int mapSize = map.GetComponent<Map>().mapSize;
		sellingLands = new bool[mapSize];
		for (int i = 0; i < mapSize; i++)
		{
			sellingLands[i] = false;
		}
	}

	void UpdateSell()
	{
		if(turnPlayerScript.currentMoney >= mapScript.landArray[turnPlayerScript.position].Getfee())
        {
			progress = GameProgress.Pay;
        }
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnSell();
		}
		else
		{
			setMark = DoOpponentSell();
		}

		if (setMark == false)
		{
			// 놓을 곳을 검토 중입니다.	
			return;
		}
		else
		{
			//기호가 놓이는 사운드 효과를 냅니다. 
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = se_setMark;
			audio.Play();
		}
		if (setMark)
			progress = GameProgress.Pay;
	}

	bool DoOwnSell()
	{
		Player seller;

		if (turn == PlayerType.Player1)
		{
			seller = player1.GetComponent<Player>();
		}
		else
		{
			seller = player2.GetComponent<Player>();
		}
		if (isConfirmed)
		{
			int sum = 0;
			for (int i = 0; i < mapSize; i++)
			{
				if (sellingLands[i] == true)
				{
					sum += map.GetComponent<Map>().landArray[i].GetCurTotalPrice();
					for (int j = 0; j < map.GetComponent<Map>().landArray[i].build.Length; j++)
						map.GetComponent<Map>().landArray[i].build[j] = false;
					map.GetComponent<Map>().landArray[i].owner = PlayerType.None;
				}
			}
			seller.GetComponent<Player>().currentMoney += sum;

			byte[] buffer = new byte[sizeof(bool) * mapSize];
			Buffer.BlockCopy(sellingLands, 0, buffer, 0, buffer.Length);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		else
			return false;
	}

	bool DoOpponentSell()
	{
		int mapSize = map.GetComponent<Map>().mapSize;
		byte[] buffer = new byte[sizeof(bool) * mapSize];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);

		if (recvSize <= 0)
		{
			// 아직 수신이 완료 되지 않았습니다.
			return false;
		}
		else
		{
			for (int i = 0; i < sellingLands.Length; i++)
			{
				sellingLands[i] = BitConverter.ToBoolean(buffer, i);
			}
			Player seller;
			if (turn == PlayerType.Player1)
			{
				seller = player1.GetComponent<Player>();
			}
			else
			{
				seller = player2.GetComponent<Player>();
			}
			int sum = 0;
			for (int i = 0; i < mapSize; i++)
			{
				if (sellingLands[i] == true)
				{
					sum += map.GetComponent<Map>().landArray[i].GetCurTotalPrice();
					for (int j = 0; j < map.GetComponent<Map>().landArray[i].build.Length; j++)
						map.GetComponent<Map>().landArray[i].build[j] = false;
					map.GetComponent<Map>().landArray[i].owner = PlayerType.None;
				}
			}
			Debug.Log("sum=" + sum);
			seller.GetComponent<Player>().currentMoney += sum;
			return true;
		}
	}

	void UpdatePay()
	{
		Player payer;
		Player taker;
		if (turn == PlayerType.Player1)
		{
			payer = player1.GetComponent<Player>();
			taker = player2.GetComponent<Player>();
		}
		else
		{
			payer = player2.GetComponent<Player>();
			taker = player1.GetComponent<Player>();
		}
		Map.Land curLand = map.GetComponent<Map>().landArray[payer.position];
		if (curLand.type == Map.LandType.Usual)
		{
			if (curLand.owner != PlayerType.None)
			{
				if (curLand.owner != turn)
				{
					if (payer.currentMoney < curLand.Getfee())
					{
						if (turn == PlayerType.Player1)
							winner = Winner.Player2;
						else
							winner = Winner.Player1;
						progress = GameProgress.Result;
						return;
					}
					else
					{
						payer.currentMoney -= curLand.Getfee();
						taker.currentMoney += curLand.Getfee();
					}
				}
			}
		}
		ResetAcuisit();
		progress = GameProgress.Acquisit;
	}

	void ResetAcuisit()
	{
		isConfirmed = false;
		willAcquisit = false;
	}

	void UpdateAcquisit()
	{
		Map.Land land = mapScript.landArray[turnPlayerScript.position];
		if(land.owner == turn || land.owner == PlayerType.None)
        {
			ResetBuy();
			progress = GameProgress.Buy;
		}
        if (BuiltAll())
        {
			ResetBuy();
			progress = GameProgress.Buy;
		}
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnAcquisit();
		}
		else
		{
			setMark = DoOpponentAcquisit();
		}

		if (setMark == false)
		{
			// 놓을 곳을 검토 중입니다.	
			return;
		}
		else
		{
			//기호가 놓이는 사운드 효과를 냅니다. 
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = se_setMark;
			audio.Play();
		}
		if (setMark)
        {
			ResetBuy();
			progress = GameProgress.Buy;
		}
	}

	void TransactLand()
	{
		if (willAcquisit)
		{
			int transactionMoney = mapScript.landArray[turnPlayerScript.position].GetCurTotalPrice() * 2;
			Player landOwner;
			if (turn == PlayerType.Player1)
				landOwner = player2.GetComponent<Player>();
			else
				landOwner = player1.GetComponent<Player>();

			turnPlayerScript.currentMoney -= transactionMoney;
			landOwner.currentMoney += transactionMoney;
			mapScript.landArray[turnPlayerScript.position].owner = turn;
		}
	}

	bool DoOwnAcquisit()
	{
		if (isConfirmed)
		{
			TransactLand();
			byte[] buffer = new byte[sizeof(bool)];
			buffer = BitConverter.GetBytes(willAcquisit);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}
	bool DoOpponentAcquisit()
	{
		byte[] buffer = new byte[sizeof(bool)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize > 0)
		{
			willAcquisit = BitConverter.ToBoolean(buffer, 0);
			TransactLand();
			return true;
		}
		else
			return false;
	}

	void ChangeTurn()
	{
		if (turn == PlayerType.Player1)
		{
			turn = PlayerType.Player2;
			turnPlayer = player2;
			turnPlayerScript = player2.GetComponent<Player>();
		}
		else
		{
			turn = PlayerType.Player1;
			turnPlayer = player1;
			turnPlayerScript = player1.GetComponent<Player>();
		}
	}

	void ResetBuy()
    {
		isConfirmed = false;
		buyingBuild = new bool[mapScript.landArray[1].build.Length];
	}

	int BuyingMoney()
    {
		int sum = 0;
		for(int i=0; i<buyingBuild.Length; i++)
        {
            if (buyingBuild[i])
            {
				sum += mapScript.landArray[turnPlayerScript.position].price[i];
            }
        }
		return sum;
    }

	bool BuiltAll()
    {
		Map.Land land = mapScript.landArray[turnPlayerScript.position];
		int sum=0;
		for(int i=0; i<land.build.Length; i++)
        {
			if (land.build[i]) sum++;
        }
		if (sum == land.build.Length) return true;
		else return false;
    }

	void UpdateBuy()
	{
		if(mapScript.landArray[turnPlayerScript.position].owner != PlayerType.None &&
			turn != mapScript.landArray[turnPlayerScript.position].owner)
        {
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
		if(turnPlayerScript.currentMoney < mapScript.landArray[turnPlayerScript.position].price[0])
        {
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
        if (BuiltAll())
        {
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
		
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnBuy();
		}
		else
		{
			setMark = DoOpponentBuy();
		}

		if (setMark == false)
		{
			// 놓을 곳을 검토 중입니다.	
			return;
		}
		else
		{
			//기호가 놓이는 사운드 효과를 냅니다. 
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = se_setMark;
			audio.Play();
		}

        if (setMark)
        {
			// 턴을 갱신합니다.
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
	}

	void TransactBuild()
    {
		Debug.Log("TransactBuild");
		Map.Land land = mapScript.landArray[turnPlayerScript.position];
		for(int i=0; i<buyingBuild.Length; i++)
        {
            if (buyingBuild[i])
            {
				land.owner = turn;
				land.build[i] = true;
				turnPlayerScript.currentMoney -= land.price[i];
            }
        }
    }

	bool DoOwnBuy()
	{
		Map.Land land = mapScript.landArray[turnPlayerScript.position];
		if (isConfirmed)
		{
			TransactBuild();
			byte[] buffer = new byte[sizeof(bool) * land.build.Length];
			Buffer.BlockCopy(buyingBuild, 0, buffer, 0, buffer.Length);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}
	bool DoOpponentBuy()
	{
		Map.Land land = mapScript.landArray[turnPlayerScript.position];
		byte[] buffer = new byte[sizeof(bool) * land.build.Length];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);

		if (recvSize <= 0)
		{
			// 아직 수신이 완료 되지 않았습니다.
			return false;
		}
        else
        {
			for (int i = 0; i < buyingBuild.Length; i++)
			{
				buyingBuild[i] = BitConverter.ToBoolean(buffer, i);
			}
			TransactBuild();
			return true;
		}
	}

	void UpdateHome()
	{
		// 턴을 갱신합니다.
		ChangeTurn();
		progress = GameProgress.Turn;
		return;
	}
	void UpdateIsolated()
	{
		turnPlayerScript.isolatedCount = 2;
		delayTime = 0;
		// 턴을 갱신합니다.
		ChangeTurn();
		progress = GameProgress.Turn;
		return;
	}
	void UpdateOlympic()
	{
		// 턴을 갱신합니다.
		ChangeTurn();
		progress = GameProgress.Turn;
		return;
	}
	void UpdateAirport()
	{
		// 턴을 갱신합니다.
		ChangeTurn();
		progress = GameProgress.Turn;
		return;
	}

	// 게임 종료 처리
	void UpdateGameOver()
	{
		step_count += Time.deltaTime;
		if (step_count > 1.0f)
		{
			// 게임을 종료합니다.
			Reset();
			isGameOver = true;
		}
	}

	// 게임 리셋.
	void Reset()
	{
		//turn = Turn.Own;
		progress = GameProgress.None;
		map.GetComponent<Map>().LoadFromAsset();
		mapSize = map.GetComponent<Map>().mapSize;
		mapScript = map.GetComponent<Map>();
		player1 = GameObject.Find("Player1");
		player2 = GameObject.Find("Player2");
		player1.GetComponent<Player>().Reset();
		player2.GetComponent<Player>().Reset();
		player1.GetComponent<Player>().playerType = PlayerType.Player1;
		player2.GetComponent<Player>().playerType = PlayerType.Player2;
		turn = PlayerType.Player1;
		turnPlayer = player1;
		turnPlayerScript = player1.GetComponent<Player>();
	}

	// 남은 시간 표시.
	void DrawTime()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 35;
		style.fontStyle = FontStyle.Bold;

		string str = "Time : " + timer.ToString("F3");

		style.normal.textColor = (timer > 5.0f) ? Color.black : Color.white;
		GUI.Label(new Rect(222, 5, 200, 100), str, style);

		style.normal.textColor = (timer > 5.0f) ? Color.white : Color.red;
		GUI.Label(new Rect(220, 3, 200, 100), str, style);
	}

	// 결과 표시.
	void DrawWinner()
	{
		float sx = SPACES_WIDTH;
		float sy = SPACES_HEIGHT;
		float left = ((float)Screen.width - sx) * 0.5f;
		float top = ((float)Screen.height - sy) * 0.5f;

		// 순서 텍스처 표시.
		float offset = (localPlayer == PlayerType.Player1) ? -94.0f : sx + 36.0f;
		Rect rect = new Rect(left + offset, top + 5.0f, 68.0f, 136.0f);
		//Graphics.DrawTexture(rect, youTexture.texture);
		GUI.Label(new Rect(100, 100, 100, 100), "YOU");

		// 결과 표시.
		rect.y += 140.0f;

		if (localPlayer == PlayerType.Player1 && winner == Winner.Player1 ||
			localPlayer == PlayerType.Player2 && winner == Winner.Player2)
		{
			//Graphics.DrawTexture(rect, winTexture.texture);
			GUI.Label(new Rect(100, 200, 100, 100), "WIN");

		}

		if (localPlayer == PlayerType.Player1 && winner == Winner.Player2 ||
			localPlayer == PlayerType.Player2 && winner == Winner.Player1)
		{
			//Graphics.DrawTexture(rect, loseTexture.texture);
			GUI.Label(new Rect(100, 200, 100, 100), "LOSE");
		}
	}

	// 끊김 통지.
	void NotifyDisconnection()
	{
		GUISkin skin = GUI.skin;
		GUIStyle style = new GUIStyle(GUI.skin.GetStyle("button"));
		style.normal.textColor = Color.white;
		style.fontSize = 25;

		float sx = 450;
		float sy = 200;
		float px = Screen.width / 2 - sx * 0.5f;
		float py = Screen.height / 2 - sy * 0.5f;

		string message = "회선이 끊겼습니다.\n\n버튼을 누르세요.";
		if (GUI.Button(new Rect(px, py, sx, sy), message, style))
		{
			// 게임이 종료됐습니다.
			Reset();
			isGameOver = true;
		}
	}

	// 게임 시작.
	public void GameStart()
	{
		Debug.Log("GameStart");
		// 게임 시작 상태로 합니다.
		progress = GameProgress.Ready;

		// 서버가 먼저 하게 설정합니다.
		turn = PlayerType.Player1;

		// 자신과 상대의 기호를 설정합니다.
		if (m_transport.IsServer() == true)
		{
			localPlayer = PlayerType.Player1;
			remotePlayer = PlayerType.Player2;
		}
		else
		{
			localPlayer = PlayerType.Player2;
			remotePlayer = PlayerType.Player1;
		}

		// 이전 설정을 클리어합니다.
		isGameOver = false;
	}

	// 게임 종료 체크.
	public bool IsGameOver()
	{
		return isGameOver;
	}

	// 이벤트 발생 시의 콜백 함수.
	public void EventCallback(NetEventState state)
	{
		switch (state.type)
		{
			case NetEventType.Disconnect:
				if (progress < GameProgress.Result && isGameOver == false)
				{
					progress = GameProgress.Disconnect;
				}
				break;
		}
	}

	public void RollDice()
	{
		var oldColor = GUI.contentColor;
		GUI.contentColor = Color.yellow;
		if (GUI.Button(new Rect(Screen.width / 2, 20, 150, 60), "주사위\n던져!!"))
		{
			isDiceRolled = true;
		}

	}

	public void Display()
	{
		DiceValueDisplay();
		PositionDisplay();
		MapDisplay();
		PlayerDisplay();
	}

	public void DiceValueDisplay()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(30, 20, 150, 20), "Dice Value: " + diceValue, style);
	}

	public void PositionDisplay()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(30, 40, 150, 20), "Player1 Position: " + player1.GetComponent<Player>().position, style);
		GUI.Label(new Rect(30, 60, 150, 20), "Player2 Position: " + player2.GetComponent<Player>().position, style);
	}

	public void MapDisplay()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		for (int i = 0; i < map.GetComponent<Map>().mapSize; i++)
		{
			Map.Land land = map.GetComponent<Map>().landArray[i];
			GUI.Label(new Rect(30, 80 + (i+2) * 20, 300, 20), i + "th land:" + land.owner + "/" + land.type + "/" + land.build[0] + "/" + land.build[1] + "/" + land.build[2] + "/" + land.build[3], style);
		}
	}

	public void PlayerDisplay()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		player1.GetComponent<Player>().ownLand.Sort();
		string lands = "";
		foreach (int landNum in player1.GetComponent<Player>().ownLand)
		{
			lands += landNum + ",";
		}
		GUI.Label(new Rect(30, 80, 300, 20), "Player1:" + lands + "/" + player1.GetComponent<Player>().currentMoney, style);
		lands = "";
		foreach (int landNum in player2.GetComponent<Player>().ownLand)
		{
			lands += landNum + ",";
		}
		GUI.Label(new Rect(30, 80 + 20, 300, 20), "Player2:" + lands + "/" + player2.GetComponent<Player>().currentMoney, style);
	}
}