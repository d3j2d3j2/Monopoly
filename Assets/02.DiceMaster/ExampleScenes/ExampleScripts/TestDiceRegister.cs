// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using DiceMaster;

/// <summary>
/// This script attaches itself to the given dice and waits for their numbers to be shown, then adds them and logs the result.
/// </summary>
public class TestDiceRegister : MonoBehaviour{

    public Dice[] dice;
    public int total = 0;

    void Start()
    {
        foreach (var d in dice)
            d.onShowNumber.AddListener(RegisterNumber);
    }

    public void RegisterNumber(int number)
    {
        Debug.Log("Got " + number);

        total += number;
        Debug.Log("Total: " + total);
    }
}
