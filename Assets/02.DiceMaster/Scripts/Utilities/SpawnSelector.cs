// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using DiceMaster;

/// <summary>
/// Controls a spawner by defining what prefab to spawn based on user input.
/// </summary>
public class SpawnSelector : MonoBehaviour
{
    Spawner spawner;

    public GameObject d2Prefab;
    public GameObject d4Prefab;
    public GameObject d6Prefab;
    public GameObject d8Prefab;
    public GameObject d10Prefab;
    public GameObject d12Prefab;
    public GameObject d20Prefab;

    void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("D2")) Spawn(d2Prefab);
        if (GUILayout.Button("D4")) Spawn(d4Prefab);
        if (GUILayout.Button("D6")) Spawn(d6Prefab);
        if (GUILayout.Button("D8")) Spawn(d8Prefab);
        if (GUILayout.Button("D10")) Spawn(d10Prefab);
        if (GUILayout.Button("D12")) Spawn(d12Prefab);
        if (GUILayout.Button("D20")) Spawn(d20Prefab);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void Spawn(GameObject prefab)
    {
        spawner.dicePrefab = prefab;
        spawner.Trigger();
    }

}
