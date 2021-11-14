using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Monopoly : MonoBehaviour
{

	// 게임 진행 상황.
	public enum GameProgress
	{
		None = 0,       // 시합 시작 전.
		Ready,          // 시합 시작 신호 표시.
		Turn,           // 시합 중.
		Move,
		FreePass,
		Sell,
		Pay,           // 땅값을 지불
		Acquisit,       //땅을 인수
		Buy,            //땅을 삼
		Home,
		Isolated,
		Olympic,
		Airport,
		GoldKey,
		GoldKeyResult,
		ForcedSell,
		Result,         // 결과 표시.
		GameOver,       // 게임 종료.
		Disconnect,     // 연결 끊기.
	};

	// 턴 종류.
	public enum Turn
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
	public enum Winner
	{
		None = 0,       // 시합 중.
		Player1,         // ○승리.
		Player2,          // ×승리.
		Tie,            // 무승부.
	};

	// 칸의 수.
	public const int rowNum = 3;

	// 시합 시작 전의 신호표시 시간.
	public const float waitTime = 1.0f;

	// 대기 시간.
	public const float turnTime = 10.0f;

	// 배치된 기호를 보존.
	public int[] spaces = new int[rowNum * rowNum];

	// 진행 상황.
	public GameProgress progress;

	// 현재의 턴.
	public PlayerType turn;

	// 로컬 기호.
	public PlayerType localPlayer;

	// 리모트 기호.
	public PlayerType remotePlayer;
	public GameObject turnPlayer;
	public Player turnPlayerScript;

	// 남은 시간.
	public float timer;

	// 승자.
	public Winner winner;

	// 게임 종료 플래그.
	public bool isGameOver;

	// 대기 시간.
	public float currentTime;

	// 네트워크.
	public TransportTCP m_transport = null;

	// 카운터.
	public float step_count = 0.0f;

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

	public static float SPACES_WIDTH = 400.0f;
	public static float SPACES_HEIGHT = 400.0f;

	public static float WINDOW_WIDTH = 640.0f;
	public static float WINDOW_HEIGHT = 480.0f;

	public bool isDiceRolled;
	public int diceValue;

	public float moveTime;
	public const float OneMoveTime = 0.5f;
	public int moveCount;

	public TextAsset defaultMap;

	public GameObject map;
	public Map mapScript;
	public GameObject player1;
	public GameObject player2;

	public bool isConfirmed;
	public bool[] sellingLands;
	public bool isBankrupt;
	public int infoTransCount;

	public bool willAcquisit;

	public bool[] buyingBuild;

	public float delayTime;

	public const int SALARY = 1000;
	public bool[] isLandChosenArray;
	public const int BuildSize = 4;
	
	public static int olympicScaler = 1;
	public static int olympicLand = -1;
	public const int MaxOlympicScaler = 5;

	public bool usedFreePass = false;
	public int forcedSellLand = -1;

	public enum GoldKeyType
    {
		Isolated,
		Olympic,
		Airport,
		Lotto,
		FreePass,
		ForcedSell,
		Size
	}
	GoldKeyType goldKey;
	public bool isGoldKeyOpen;
	public const int LOTTO_MONEY = 100000;

	public Map.MonopolyType WinCondition(PlayerType player)
    {
		for(int i=0; i<4; i++)
        {
			if (player == mapScript.WhoMonopolySeason(i))
			{
				return Map.MonopolyType.Season;
			}
		}
		if (player == mapScript.WhoMonopoly3Region()) return Map.MonopolyType.Region3;
		if (player == mapScript.WhoMonopolyFestival()) return Map.MonopolyType.Festival;
		return Map.MonopolyType.None;
	}

	public Map.MonopolyType winCondition;

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
			case GameProgress.FreePass:
				UpdateFreePass();
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
			case GameProgress.GoldKey:
				UpdateGoldKey();
				break;
			case GameProgress.GoldKeyResult:
				UpdateGoldKeyResult();
				break;

			case GameProgress.ForcedSell:
				UpdateForcedSell();
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

			case GameProgress.FreePass:
				GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200, 20), "보유 면제권: " + turnPlayerScript.freepass);
				int notEnoughMoney = Map.landArray[turnPlayerScript.position].GetFee() - turnPlayerScript.currentMoney;
				if (notEnoughMoney < 0) notEnoughMoney = 0;
				GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 20, 200, 20), "통행료 부족액: " + notEnoughMoney);
				if(turn == localPlayer)
                {
					usedFreePass = GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 2 + 20 * 2, 200, 20), usedFreePass, "면제권 사용시 체크");
					isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 20 * 3, 200, 20), "승인");
                }
                else
                {
					GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 20 * 2, 200, 20),"면제권사용 결정중");
				}

				break;

			case GameProgress.Sell:
				Display();
				mapScript = map.GetComponent<Map>();
				Player seller = turnPlayerScript;
				if (turn == localPlayer)
				{
					for (int i = 0; i < Map.mapSize; i++)
					{
						if (Map.landArray[i].owner == turn)
						{
							sellingLands[i] = GUI.Toggle(new Rect(Screen.width / 2, 20 + i * 20, 100, 30), sellingLands[i], " " + i + ":" + Map.landArray[i].GetCurTotalPrice());
						}
					}
					int needMoney = Map.landArray[seller.position].GetFee() - seller.currentMoney;
					if (needMoney > 0)
						GUI.Label(new Rect(Screen.width / 2, 20 + Map.mapSize * 20, 100, 30), "부족금액 = " + needMoney);
					isConfirmed = GUI.Button(new Rect(Screen.width / 2, 20 + Map.mapSize * 20 + 20, 100, 30), "매각 승인");
				}
				
				break;

			case GameProgress.Pay:
				GUI.Label(new Rect(Screen.width / 2, Screen.height/2, 100, 30),
					"지불금액 = " +
					(Map.landArray[turnPlayerScript.position].owner == turnPlayerScript.playerType ? 0:Map.landArray[turnPlayerScript.position].GetFee())
					);
				if (turn == localPlayer)
				{
					isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 30, 100, 30), "지불 승인");
				}
				Display();
				break;

			case GameProgress.Acquisit:
				Display();
				Player acquisiter = turnPlayerScript;
				if (turn == localPlayer)
				{
					if (Map.landArray[turnPlayerScript.position].owner != PlayerType.None)
					{
						if (turn != Map.landArray[turnPlayerScript.position].owner)
						{
							if (acquisiter.currentMoney >= Map.landArray[acquisiter.position].GetCurTotalPrice() * 2)
							{
								GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 - 40, 100, 20), "인수비용: " + (Map.landArray[turnPlayerScript.position].GetCurTotalPrice() * 2));
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
					Map.Land land = Map.landArray[turnPlayerScript.position];
					for(int i=0; i<land.build.Length; i++)
                    {
                        if (!land.build[i])
                        {
							buyingBuild[i] = GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 2 + i*20, 100, 20), buyingBuild[i], "건물 " + i);
                        }
                    }
                    if (BuyingMoney(turnPlayerScript.position) <= turnPlayerScript.currentMoney)
                    {
						isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + land.build.Length * 20, 100, 20),"구매 승인");
                    }
                }
				
				break;
			case GameProgress.Home:
				if(turn == localPlayer)
                {
                    if (turnPlayerScript.haveLands())
                    {
						if (NumChosenLands() != 1)
						{
							for (int i = 0; i < Map.mapSize; i++)
							{
								if (Map.landArray[i].owner == turnPlayerScript.playerType)
								{
									isLandChosenArray[i] = GUI.Toggle(new Rect(Screen.width / 2, Screen.height/2 + i * 20, 100, 20), isLandChosenArray[i], "Land " + i);
								}
							}
						}
                        else
                        {
							
							if (Map.landArray[ChosenLand()].NumBuild() == Map.landArray[ChosenLand()].build.Length)
							{
				
								GUI.Label(new Rect(Screen.width / 2, (Map.mapSize) * 20, 200, 20), "더이상 구매불가");
								isConfirmed = GUI.Button(new Rect(Screen.width / 2, (Map.mapSize + 1) * 20, 100, 20), "승인");
							}
                            else
                            {
								Debug.Log("DP");
								Map.Land land = Map.landArray[ChosenLand()];
								for (int i = 0; i < land.build.Length; i++)
								{
									if (!land.build[i])
									{
										buyingBuild[i] = GUI.Toggle(new Rect(Screen.width / 2, Screen.height / 2 + i * 20, 100, 20), buyingBuild[i], "건물" + i+": "+ Map.landArray[ChosenLand()].price[i]);
									}
								}
								GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + land.build.Length * 20, 100, 20), "구매비용: " + BuyingMoney(ChosenLand()));
								if (BuyingMoney(ChosenLand()) <= turnPlayerScript.currentMoney)
								{
									isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + (land.build.Length+1) * 20, 100, 20), "구매 승인");
								}
							}
						}
					}
				}
				Display();
				break;

			case GameProgress.Isolated:
				Display();
				break;
			case GameProgress.Olympic:
				if (turn == localPlayer)
				{
					if (turnPlayerScript.haveLands())
					{
						if (NumChosenLands() != 1)
						{
							for (int i = 0; i < Map.mapSize; i++)
							{
								if (Map.landArray[i].owner == turnPlayerScript.playerType)
								{
									isLandChosenArray[i] = GUI.Toggle(new Rect(Screen.width / 2, i * 20, 100, 20), isLandChosenArray[i], "Land " + i);
								}
							}
							if(NumChosenLands() == 1)
                            {
								isConfirmed = true;
                            }
						}
					}
				}
                else
                {
					GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 20), "올림픽 개최지 결정중");
                }
				Display();
				break;
			case GameProgress.Airport:
				if (turn == localPlayer)
				{
					if (NumChosenLands() != 1)
					{
						for (int i = 0; i < Map.mapSize; i++)
						{	
							if(i != turnPlayerScript.position)
								if(Map.landArray[i].type != Map.LandType.Airport)
									isLandChosenArray[i] = GUI.Toggle(new Rect(Screen.width / 2, i * 20, 100, 20), isLandChosenArray[i], "Land " + i);
						}
						if (NumChosenLands() == 1)
						{
							isConfirmed = true;
						}
					}	
				}
				Display();
				break;
			case GameProgress.GoldKey:
				GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 30), "황금열쇠 대기중");
				if(turn == localPlayer)
					isGoldKeyOpen = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 30, 100, 30), "황금열쇠 뽑기");
				break;
			case GameProgress.GoldKeyResult:
				GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200, 30), "황금열쇠 결과: " + goldKey);
				if(turn == localPlayer)
					isConfirmed = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 30, 100, 30), "황금열쇠 승인");
				break;

			case GameProgress.ForcedSell:
				if (turn == localPlayer)
				{
					if (turnPlayerScript.haveLands())
					{
						if (NumChosenLands() != 1)
						{
							for (int i = 0; i < Map.mapSize; i++)
							{
								if (Map.landArray[i].owner == turnPlayerScript.playerType)
								{
									isLandChosenArray[i] = GUI.Toggle(new Rect(Screen.width / 2, i * 20, 100, 20), isLandChosenArray[i], "Land " + i);
								}
							}
							if (NumChosenLands() == 1)
							{
								isConfirmed = true;
							}
						}
					}
				}
				else
				{
					GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 20), "강제 매각지 결정중");
				}
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
				ResetMove();
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
        {
			ResetMove();
			progress = GameProgress.Move;
		}
			
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

	void ResetMove()
    {
		timer = turnTime;
		moveTime = 0;
		moveCount = 0;
	}

	void UpdateMove()
	{
		if (moveCount < diceValue)
		{
			moveTime += Time.deltaTime;
			if (moveTime >= OneMoveTime)
			{
				moveCount++;
				turnPlayerScript.position = (turnPlayerScript.position + 1) % Map.mapSize;
                if (turnPlayerScript.position == 0)
                {
					turnPlayerScript.currentMoney += SALARY;
                }
				moveTime = 0f;
			}
		}
		else
		{
			int pos;
			if (turn == PlayerType.Player1)
				pos = player1.GetComponent<Player>().position;
			else
				pos = player2.GetComponent<Player>().position;
			Map.LandType landType = Map.landArray[pos].type;

			if (landType == Map.LandType.Usual || landType == Map.LandType.Festival)
			{
                if (turnPlayerScript.freepass > 0
					&& Map.landArray[turnPlayerScript.position].owner != PlayerType.None
						&& turn != Map.landArray[turnPlayerScript.position].owner)
                {
					ResetFreePass();
					progress = GameProgress.FreePass;
                }
                else
                {
					ResetSell();
					progress = GameProgress.Sell;
				}
			}
			if (landType == Map.LandType.Home)
			{
				ResetHome();
				progress = GameProgress.Home;
			}
			if (landType == Map.LandType.Isolated) progress = GameProgress.Isolated;
			if (landType == Map.LandType.Olympic)
			{
				ResetOlympic();
				progress = GameProgress.Olympic;
			}
			if (landType == Map.LandType.Airport)
			{
				ResetAirport();
				progress = GameProgress.Airport;
			}
			if(landType == Map.LandType.GoldKey)
            {
				ResetGoldKey();
				progress = GameProgress.GoldKey;
            }
		}
	}

	void ResetFreePass()
    {
		isConfirmed = false;
		usedFreePass = false;
    }

	void UpdateFreePass()
	{
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnFreePass();
		}
		else
		{
			setMark = DoOpponentFreePass();
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
            if (usedFreePass)
            {
				turnPlayerScript.freepass--;
				ResetAcuisit();
				progress = GameProgress.Acquisit;
            }
            else
            {
				ResetSell();
				progress = GameProgress.Sell;
			}
		}
	}

	bool DoOwnFreePass()
    {
		if (isConfirmed)
		{
			byte[] buffer = new byte[sizeof(bool)];
			buffer = BitConverter.GetBytes(usedFreePass);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}

	bool DoOpponentFreePass()
    {
		byte[] buffer = new byte[sizeof(bool)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize > 0)
		{
			usedFreePass = BitConverter.ToBoolean(buffer, 0);
			return true;
		}
		return false;
	}

	void ResetSell()
	{
		isBankrupt = false;
		isConfirmed = false;
		infoTransCount = 0;
		int mapSize = Map.mapSize;
		sellingLands = new bool[mapSize];
		for (int i = 0; i < mapSize; i++)
		{
			sellingLands[i] = false;
		}
	}

	void UpdateSell()
	{
		if(turnPlayerScript.currentMoney >= Map.landArray[turnPlayerScript.position].GetFee())
        {
			ResetPay();
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
        {
			ResetPay();
			progress = GameProgress.Pay;
		}
			
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
			for (int i = 0; i < Map.mapSize; i++)
			{
				if (sellingLands[i] == true)
				{
					sum += Map.landArray[i].GetCurTotalPrice();
					for (int j = 0; j < Map.landArray[i].build.Length; j++)
						Map.landArray[i].build[j] = false;
					Map.landArray[i].owner = PlayerType.None;
				}
			}
			seller.GetComponent<Player>().currentMoney += sum;

			byte[] buffer = new byte[sizeof(bool) * Map.mapSize];
			Buffer.BlockCopy(sellingLands, 0, buffer, 0, buffer.Length);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		else
			return false;
	}

	bool DoOpponentSell()
	{
		int mapSize = Map.mapSize;
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
					sum += Map.landArray[i].GetCurTotalPrice();
					for (int j = 0; j < Map.landArray[i].build.Length; j++)
						Map.landArray[i].build[j] = false;
					Map.landArray[i].owner = PlayerType.None;
				}
			}
			Debug.Log("sum=" + sum);
			seller.GetComponent<Player>().currentMoney += sum;
			return true;
		}
	}

	void UpdatePay()
    {
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnPay();
		}
		else
		{
			setMark = DoOpponentPay();
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
			if (isBankrupt)
				progress = GameProgress.Result;
            else
            {
				ResetAcuisit();
				progress = GameProgress.Acquisit;
			}
		}
	}

	void ResetPay()
    {
		isConfirmed = false;
		isBankrupt = false;
    }

	bool DoOwnPay()
    {
		if (isConfirmed)
        {
			byte[] buffer = new byte[sizeof(bool)];
			buffer = BitConverter.GetBytes(isConfirmed);
			m_transport.Send(buffer, buffer.Length);

			Map.Land curLand = Map.landArray[turnPlayerScript.position];
			if (curLand.type == Map.LandType.Usual || curLand.type == Map.LandType.Festival)
			{
				if (curLand.owner != PlayerType.None)
				{
					if (curLand.owner != turn)
					{
						if (turnPlayerScript.currentMoney < curLand.GetFee())
						{
							if (turn == PlayerType.Player1)
								winner = Winner.Player2;
							else
								winner = Winner.Player1;
							isBankrupt = true;
						}
						else
						{
							turnPlayerScript.currentMoney -= curLand.GetFee();
							Player taker;
							if (turn == player1.GetComponent<Player>().playerType)
								taker = player2.GetComponent<Player>();
							else
								taker = player1.GetComponent<Player>();
							taker.currentMoney += curLand.GetFee();
							Debug.Log("" + turnPlayerScript.playerType);
						}
					}
				}
			}
			return true;
		}
		return false;
		
	}

	bool DoOpponentPay()
    {
		byte[] buffer = new byte[sizeof(bool)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize > 0)
		{
			isConfirmed = BitConverter.ToBoolean(buffer, 0);
		}
		if (isConfirmed)
		{
			Map.Land curLand = Map.landArray[turnPlayerScript.position];
			if (curLand.type == Map.LandType.Usual || curLand.type == Map.LandType.Festival)
			{
				if (curLand.owner != PlayerType.None)
				{
					if (curLand.owner != turn)
					{
						if (turnPlayerScript.currentMoney < curLand.GetFee())
						{
							if (turn == PlayerType.Player1)
								winner = Winner.Player2;
							else
								winner = Winner.Player1;
							isBankrupt = true;
						}
						else
						{
							turnPlayerScript.currentMoney -= curLand.GetFee();
							Player taker;
							if (turn == player1.GetComponent<Player>().playerType)
								taker = player1.GetComponent<Player>();
							else
								taker = player2.GetComponent<Player>();
							taker.currentMoney += curLand.GetFee();
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	void ResetAcuisit()
	{
		isConfirmed = false;
		willAcquisit = false;
	}

	void UpdateAcquisit()
	{
		Map.Land land = Map.landArray[turnPlayerScript.position];
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
			winCondition = WinCondition(turn);
			if(winCondition == Map.MonopolyType.Season
				|| winCondition == Map.MonopolyType.Region3
				|| winCondition == Map.MonopolyType.Festival)
            {
				if (turn == PlayerType.Player1) winner = Winner.Player1;
				else winner = Winner.Player2;
				progress = GameProgress.Result;
            }
            else
            {
				ResetBuy();
				progress = GameProgress.Buy;
			}
		}
	}

	void TransactLand()
	{
		if (willAcquisit)
		{
			int transactionMoney = Map.landArray[turnPlayerScript.position].GetCurTotalPrice() * 2;
			Player landOwner;
			if (turn == PlayerType.Player1)
				landOwner = player2.GetComponent<Player>();
			else
				landOwner = player1.GetComponent<Player>();

			turnPlayerScript.currentMoney -= transactionMoney;
			landOwner.currentMoney += transactionMoney;
			Map.landArray[turnPlayerScript.position].owner = turn;
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
		if (Map.landArray[turnPlayerScript.position].type == Map.LandType.Usual)
			buyingBuild = new bool[4];
		else if(Map.landArray[turnPlayerScript.position].type == Map.LandType.Festival)
			buyingBuild = new bool[1];
	}

	int BuyingMoney(int position)
    {
		int sum = 0;
		for(int i=0; i<buyingBuild.Length; i++)
        {
            if (buyingBuild[i])
            {
				sum += Map.landArray[position].price[i];
            }
        }
		return sum;
    }

	bool BuiltAll()
    {
		Map.Land land = Map.landArray[turnPlayerScript.position];
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
		if(Map.landArray[turnPlayerScript.position].owner != PlayerType.None &&
			turn != Map.landArray[turnPlayerScript.position].owner)
        {
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
		if(turnPlayerScript.currentMoney < Map.landArray[turnPlayerScript.position].price[0])
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
			winCondition = WinCondition(turn);
			if (winCondition == Map.MonopolyType.Season
				|| winCondition == Map.MonopolyType.Region3
				|| winCondition == Map.MonopolyType.Festival)
			{
				if (turn == PlayerType.Player1) winner = Winner.Player1;
				else winner = Winner.Player2;
				progress = GameProgress.Result;
			}
			else
			{
				// 턴을 갱신합니다.
				ChangeTurn();
				progress = GameProgress.Turn;
			}
		}
	}

	void TransactBuild(int position)
    {
		Debug.Log("TransactBuild");
		Map.Land land = Map.landArray[position];
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
		Map.Land land = Map.landArray[turnPlayerScript.position];
		if (isConfirmed)
		{
			TransactBuild(turnPlayerScript.position);
			byte[] buffer = new byte[sizeof(bool) * land.build.Length];
			Buffer.BlockCopy(buyingBuild, 0, buffer, 0, buffer.Length);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}
	bool DoOpponentBuy()
	{
		Map.Land land = Map.landArray[turnPlayerScript.position];
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
			TransactBuild(turnPlayerScript.position);
			return true;
		}
	}
	void ResetHome()
    {
		isLandChosenArray = new bool[Map.mapSize];
		ResetBuy();
    }
	public int NumChosenLands()
	{
		int sum = 0;
		for(int i=0; i<Map.mapSize; i++)
        {
			if (isLandChosenArray[i])
            {
				sum++;
			}
        }
		return sum;
	}
	public int ChosenLand()
    {
		for(int i=0; i<Map.mapSize; i++)
        {
			if (isLandChosenArray[i]) return i;
        }
		return -1;
    }

	public bool DoOwnHome()
    {
        if (isConfirmed)
        {
			TransactBuild(ChosenLand());
			byte[] buffer = new byte[sizeof(int)+sizeof(bool)*buyingBuild.Length];
			Buffer.BlockCopy(BitConverter.GetBytes(ChosenLand()), 0, buffer, 0, sizeof(int));
			Buffer.BlockCopy(buyingBuild, 0, buffer, sizeof(int), sizeof(bool)*buyingBuild.Length);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
    }
	public bool DoOpponentHome()
    {
		byte[] buffer = new byte[sizeof(int) + sizeof(bool) * buyingBuild.Length];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize <= 0)
		{
			// 아직 수신이 완료 되지 않았습니다.
			return false;
		}
		else
		{
			int chosenLand = BitConverter.ToInt32(buffer, 0);
			Map.Land land = Map.landArray[chosenLand];
			for (int i = 0; i < buyingBuild.Length; i++)
			{
				buyingBuild[i] = BitConverter.ToBoolean(buffer, sizeof(int)+i);
			}
			TransactBuild(chosenLand);
			return true;
		}
	}

	void UpdateHome()
	{
		if (turnPlayerScript.haveLands()==false)
		{
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnHome();
		}
		else
		{
			setMark = DoOpponentHome();
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
	void UpdateIsolated()
	{
		turnPlayerScript.isolatedCount = 2;
		delayTime = 0;
		// 턴을 갱신합니다.
		ChangeTurn();
		progress = GameProgress.Turn;
		return;
	}

	void ResetOlympic()
    {
		isLandChosenArray = new bool[Map.mapSize];
		isConfirmed = false;
	}

	bool DoOwnOlympic()
    {
		if (isConfirmed)
		{
			olympicLand = ChosenLand();
			olympicScaler = Math.Min(olympicScaler + 1, MaxOlympicScaler);
			byte[] buffer = new byte[sizeof(int)];
			Buffer.BlockCopy(BitConverter.GetBytes(olympicLand), 0, buffer, 0, sizeof(int));
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}
	bool DoOpponentOlympic()
    {
		byte[] buffer = new byte[sizeof(int)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize <= 0)
		{
			// 아직 수신이 완료 되지 않았습니다.
			return false;
		}
		else
		{
			olympicLand = BitConverter.ToInt32(buffer, 0);
			olympicScaler = Math.Min(olympicScaler + 1, MaxOlympicScaler);
			return true;
		}
	}

	void UpdateOlympic()
	{
		if (turnPlayerScript.haveLands() == false)
		{
			ChangeTurn();
			progress = GameProgress.Turn;
			return;
		}
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnOlympic();
		}
		else
		{
			setMark = DoOpponentOlympic();
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

	void ResetAirport()
    {
		isLandChosenArray = new bool[Map.mapSize];
		isConfirmed = false;
    }

	bool DoOwnAirport()
    {
		if (isConfirmed)
		{
			turnPlayerScript.position = ChosenLand();
			byte[] buffer = new byte[sizeof(int)];
			Buffer.BlockCopy(BitConverter.GetBytes(turnPlayerScript.position), 0, buffer, 0, sizeof(int));
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}

	bool DoOpponentAirport()
    {
		byte[] buffer = new byte[sizeof(int)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize <= 0)
		{
			// 아직 수신이 완료 되지 않았습니다.
			return false;
		}
		else
		{
			turnPlayerScript.position = BitConverter.ToInt32(buffer, 0);
			return true;
		}
	}

	void UpdateAirport()
	{
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnAirport();
		}
		else
		{
			setMark = DoOpponentAirport();
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

		if(setMark == true)
        {
			Map.LandType landType = Map.landArray[turnPlayerScript.position].type;

			if (landType == Map.LandType.Usual || landType == Map.LandType.Festival)
			{
				if (turnPlayerScript.freepass > 0
					&& Map.landArray[turnPlayerScript.position].owner != PlayerType.None
					&& turn != Map.landArray[turnPlayerScript.position].owner)
				{
					ResetFreePass();
					progress = GameProgress.FreePass;
				}
				else
				{
					ResetSell();
					progress = GameProgress.Sell;
				}
			}
			if (landType == Map.LandType.Home)
			{
				ResetHome();
				progress = GameProgress.Home;
			}
			if (landType == Map.LandType.Isolated) progress = GameProgress.Isolated;
			if (landType == Map.LandType.Olympic)
			{
				ResetOlympic();
				progress = GameProgress.Olympic;
			}
			if (landType == Map.LandType.GoldKey)
			{
				ResetGoldKey();
				progress = GameProgress.GoldKey;
			}
		}

	}

	void ResetGoldKey()
	{
		goldKey = (GoldKeyType)UnityEngine.Random.Range(0, (int)GoldKeyType.Size - 1);
		isGoldKeyOpen = false;
	}

	void UpdateGoldKey()
	{
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnGoldKey();
		}
		else
		{
			setMark = DoOpponentGoldKey();
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
			ResetGoldKeyResult();
			progress = GameProgress.GoldKeyResult;
		}
	}

	bool DoOwnGoldKey()
    {
		if (isGoldKeyOpen)
		{
			byte[] buffer = new byte[sizeof(int)];
			buffer = BitConverter.GetBytes((int)goldKey);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}

	bool DoOpponentGoldKey()
    {
		byte[] buffer = new byte[sizeof(int)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize > 0)
		{
			isGoldKeyOpen = true;
			goldKey = (GoldKeyType)BitConverter.ToInt32(buffer, 0);
			return true;
		}
		return false;
	}

	void ResetGoldKeyResult()
    {
		isConfirmed = false;
    }

	void UpdateGoldKeyResult()
    {
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnGoldKeyResult();
		}
		else
		{
			setMark = DoOpponentGoldKeyResult();
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
			if (goldKey == GoldKeyType.Isolated)
			{
				progress = GameProgress.Isolated;
			}
			else if (goldKey == GoldKeyType.Olympic)
			{
				ResetOlympic();
				progress = GameProgress.Olympic;
			}
			else if (goldKey == GoldKeyType.Airport)
			{
				ResetAirport();
				progress = GameProgress.Airport;
			}
			else if (goldKey == GoldKeyType.Lotto)
			{
				turnPlayerScript.currentMoney += LOTTO_MONEY;
				ChangeTurn();
				progress = GameProgress.Turn;
			}
			else if(goldKey == GoldKeyType.FreePass)
            {
				turnPlayerScript.freepass += 1;
				ChangeTurn();
				progress = GameProgress.Turn;
			}
			else if(goldKey == GoldKeyType.ForcedSell)
            {
                if (turnPlayerScript.haveLands())
                {
					ResetForcedSell();
					progress = GameProgress.ForcedSell;
				}
                else
                {
					ChangeTurn();
					progress = GameProgress.Turn;
				}
            }
		}
	}

	bool DoOwnGoldKeyResult()
    {
		if (isConfirmed)
		{
			byte[] buffer = new byte[sizeof(bool)];
			buffer = BitConverter.GetBytes(isConfirmed);
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
	}

	bool DoOpponentGoldKeyResult()
    {
		byte[] buffer = new byte[sizeof(bool)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize > 0)
		{
			isConfirmed = BitConverter.ToBoolean(buffer, 0);
			return true;
		}
		return false;
	}

	void ResetForcedSell()
    {
		isConfirmed = false;
		isLandChosenArray = new bool[Map.mapSize];
	}

	void UpdateForcedSell()
    {
		bool setMark = false;

		if (turn == localPlayer)
		{
			setMark = DoOwnForcedSell();
		}
		else
		{
			setMark = DoOpponentForcedSell();
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
			if(ChosenLand() != -1)
            {
				for(int i=0; i<Map.landArray[ChosenLand()].build.Length; i++)
                {
					Map.landArray[ChosenLand()].build[i] = false;
				}
				Map.landArray[ChosenLand()].owner = PlayerType.None;
			}
			ChangeTurn();
			progress = GameProgress.Turn;
		}
	}

	bool DoOwnForcedSell()
    {
        if (isConfirmed)
        {
			byte[] buffer = new byte[sizeof(int)];
			buffer = BitConverter.GetBytes(ChosenLand());
			m_transport.Send(buffer, buffer.Length);
			return true;
		}
		return false;
    }

	bool DoOpponentForcedSell()
    {
		byte[] buffer = new byte[sizeof(int)];
		int recvSize = m_transport.Receive(ref buffer, buffer.Length);
		if (recvSize > 0)
		{
			forcedSellLand = BitConverter.ToInt32(buffer, 0);
			isLandChosenArray[forcedSellLand] = true;
			return true;
		}
		return false;
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
		olympicScaler = 1;
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
		//GUI.Label(new Rect(100, 100, 100, 100), "YOU");

		// 결과 표시.
		rect.y += 140.0f;

		GUI.Label(new Rect(100, 200, 100, 100), winner+" 승리 "+winCondition);
		/*
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
		*/
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
		PlayerDisplay();
		OlympicDisplay();
		MapDisplay();
		
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
		for (int i = 0; i < Map.mapSize; i++)
		{
			Map.Land land = Map.landArray[i];
			string landInfo = i + "th land:" + land.type + "/" + land.owner;
			if(land.build != null)
            {
				for (int j = 0; j < land.build.Length; j++)
				{
					landInfo += "/" + land.build[j];
				}
			}
			GUI.Label(new Rect(30, 80 + (i+3) * 20, 300, 20), landInfo, style);
		}
	}

	public void PlayerDisplay()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		string lands = "";
		for(int i=0; i<Map.landArray.Length; i++)
        {
            if (Map.landArray[i].owner == PlayerType.Player1)
            {
				lands += i + ",";
            }
        }
		GUI.Label(new Rect(30, 80, 300, 20), "Player1:" + lands + "/" + player1.GetComponent<Player>().currentMoney
			+"/FreePass:"+player1.GetComponent<Player>().freepass, style);
		lands = "";
		for (int i = 0; i < Map.landArray.Length; i++)
		{
			if (Map.landArray[i].owner == PlayerType.Player2)
			{
				lands += i + ",";
			}
		}
		GUI.Label(new Rect(30, 80 + 20, 300, 20), "Player2:" + lands + "/" + player2.GetComponent<Player>().currentMoney
			+ "/FreePass:" + player2.GetComponent<Player>().freepass, style);
	}

	public void OlympicDisplay()
    {
		GUI.Label(new Rect(30, 80 + 2 * 20, 300, 20), "Olympic:" + olympicLand + "/" + "Olympic Scaler:" + olympicScaler);
    }
}