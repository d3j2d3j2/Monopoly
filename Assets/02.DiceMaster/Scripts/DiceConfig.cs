// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// General configuration class, an instance should be placed in the Resources folder
    /// </summary>
    public class DiceConfig : ScriptableObject
    {

        static DiceConfig _instance;
        public static DiceConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<DiceConfig>("DiceConfig") as DiceConfig;
                    Debug.Assert(_instance != null, "Could not find the 'DiceConfig' custom asset in the Resources folder. Make sure not to delete it!");
                }
                return _instance;
            }
        }

        public string OutputPathComplete
        {
            get { return AssetsPath + ProjectPath + OutputPath; }
        }
        public string OutputDataPathComplete
        {
            get { return Application.dataPath + "/" + ProjectPath + OutputPath; }
        }
        public string DefaultPipsPathComplete
        {
            get { return AssetsPath + ProjectPath + DefaultPipsPath; }
        }
        public string DefaultMeshesPathComplete
        {
            get { return AssetsPath + ProjectPath + DefaultMeshesPath; }
        }
        public string DefaultMaterialsPathComplete
        {
            get { return AssetsPath + ProjectPath + DefaultMaterialsPath; }
        }

        public const string AssetsPath = "Assets/";
        public const string ProjectPath = "DiceMaster/";
        public const string OutputPath = "_Output/";
        public const string DefaultPipsPath = "Assets/GFX/Textures/Pips/";
        public const string DefaultMeshesPath = "Assets/GFX/Meshes/";
        public const string DefaultMaterialsPath = "Assets/GFX/Materials/";

        public float parallelTolerance = 0.1f;              // Tolerance on parallel axis checks used to determine what number is shown. This value works good with a D20 too.
        public float checkSpeedThreshold = 0.1f;            // Threshold under which a dice is supposed to have stopped moving and the shown number can be determined.

        public float overridenMaxAngularVelocity = 0;       // Increase max angular velocity, to enable spinning dice). Make sure it is no more than 100!
        public float dynamicFaceSizeMultiplier = 0.1f;     // This value is by default multiplied to the pip size value when using dynamic faces, to determine the final face size.

        public int wrongFaceOutputValue = -1;        // Value output when a dice cannot detect what face is currently shown

        [HideInInspector]
        public float rotationTextureMultiplier = 1.4f;
    }

}
