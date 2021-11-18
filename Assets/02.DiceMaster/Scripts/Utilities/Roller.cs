// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using DiceMaster;

/// <summary>
/// Rolls a dice by triggering a Thrower and/or a Spinner when the R key, a mouse button, or a touch is detected.
/// </summary>
public class Roller : MonoBehaviour
{

    Spinner spinner;
    Thrower thrower;

    void Start()
    {
        spinner = GetComponent<Spinner>();
        thrower = GetComponent<Thrower>();

        if (spinner)
            spinner.autoDestroy = false;
        if (thrower)
            thrower.autoDestroy = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
        {
            if (thrower) thrower.Trigger();
            if (spinner) spinner.Trigger();
        }
    }
}
