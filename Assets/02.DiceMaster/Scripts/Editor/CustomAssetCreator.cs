// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using UnityEditor;

namespace DiceMaster
{
    /// <summary>
    /// Utility class that uses the CustomAssetUtility to create specific custom assets
    /// </summary>
    public class CustomAssetCreator : MonoBehaviour
    {
        [MenuItem("Assets/Create/DiceDefinition")]
        public static void CreateDiceDefinition()
        {
            CustomAssetUtility.CreateAsset<DiceDefinition>();
        }

        [MenuItem("Assets/Create/DiceConfig")]
        public static void CreateDiceConfig()
        {
            CustomAssetUtility.CreateAsset<DiceConfig>();
        }
    }
}