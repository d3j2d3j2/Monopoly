// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using System.Collections;
using DiceMaster;

/// <summary>
/// Controls a spawner by defining what prefabs to spawn based on a string that is passed to it.
/// </summary>
public class SpawnFormat : MonoBehaviour
{
    Spawner spawner;

    public GameObject d2Prefab;
    public GameObject d4Prefab;
    public GameObject d6Prefab;
    public GameObject d8Prefab;
    public GameObject d10Prefab;
    public GameObject d12Prefab;
    public GameObject d20Prefab;

    private string diceString = "2D6";
    private float spawnDelay = 0.2f;

    void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    void OnGUI()
    {
        var spanX = 200;
        GUILayout.BeginArea(new Rect(spanX, 0, Screen.width - spanX * 2, Screen.height));
        GUILayout.BeginHorizontal();

        //GUILayout.BeginVertical();

        diceString = GUILayout.TextField(diceString);
        if (GUILayout.Button("Spawn"))
            Interpret(diceString);
        // GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void Interpret(string diceString)
    {
        int nThrown = 0;
        int nFaces = 0;

        var numberStrings = diceString.Split('D');

        Debug.Assert(numberStrings.Length == 2, "Dice string is not in the correct format!");

        nThrown = int.Parse(numberStrings[0]);
        nFaces = int.Parse(numberStrings[1]);

        StartCoroutine(SpawnCO(nThrown, nFaces));
    }

    IEnumerator SpawnCO(int nThrown, int nFaces)
    {
        GameObject dicePrefab = null;
        switch (nFaces)
        {
            case 2: dicePrefab = d2Prefab; break;
            case 4: dicePrefab = d4Prefab; break;
            case 6: dicePrefab = d6Prefab; break;
            case 8: dicePrefab = d8Prefab; break;
            case 10: dicePrefab = d10Prefab; break;
            case 12: dicePrefab = d12Prefab; break;
            case 20: dicePrefab = d20Prefab; break;
        }
        spawner.dicePrefab = dicePrefab;

        for (int i = 0; i < nThrown; i++)
        {
            spawner.Trigger();
            yield return new WaitForSeconds(spawnDelay);
        }

    }

}
