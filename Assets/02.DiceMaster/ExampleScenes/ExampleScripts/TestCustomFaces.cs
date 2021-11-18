// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using System.Collections;
using DiceMaster;

/// <summary>
/// This script randomly activates the shown face of a custom dice.
/// </summary>
public class TestCustomFaces : MonoBehaviour
{
    public Dice targetDice;

    void Start()
    {
        Debug.Assert(targetDice != null, "Make sure a target dice is set.");
        Debug.Assert(targetDice.dynamicFaceGos != null, "Make sure you are using a dice with dynamic faces.");
        targetDice.onShowNumber.AddListener(HandleShownFace);
    }

    public void HandleShownFace(int number)
    {
        var pipAnimation = targetDice.dynamicFaceGos[number - 1].GetComponent<PipAnimation>();
        if (pipAnimation != null)
        {
            pipAnimation.Trigger();
        }
    }

}
