// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using DiceMaster;

/// <summary>
/// Registers to spawned dice and logs their result
/// </summary>
public class TestSpawnRegister : MonoBehaviour {

    Spawner spawner;

	void Start () {
        spawner = GetComponent<Spawner>();

        spawner.onSpawnDice.AddListener(this.OnNewDiceSpawned);
	}

    void OnNewDiceSpawned(Dice dice)
    {
        dice.onShowNumber.AddListener(RegisterNumber);
	}

    public void RegisterNumber(int number)
    {
        Debug.Log("Got " + number);
    }
}
