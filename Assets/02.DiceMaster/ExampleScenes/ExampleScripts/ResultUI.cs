// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tie this to a Dice event to see the result in the GUI
/// </summary>
public class ResultUI : MonoBehaviour
{

    public Text textUI;

    public void Awake()
    {
        if (textUI == null)
        {
            Debug.LogError("Please assign the textUI property of ResultUI");
            return;
        }

        textUI.text = "";
    }

    public void HandleResult(int v)
    {
        if (textUI == null)
        {
            Debug.LogError("Please assign the textUI property of ResultUI");
            return;
        }

        textUI.text = "You rolled a " + v.ToString() + "!";
    }
}
