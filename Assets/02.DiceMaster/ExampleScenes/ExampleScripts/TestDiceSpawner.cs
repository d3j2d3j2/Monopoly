// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using DiceMaster;

/// <summary>
/// Triggers a spawner with the S key
/// </summary>
public class TestDiceSpawner : MonoBehaviour
{

    public Spawner spawner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            spawner.Trigger();
    }

}
