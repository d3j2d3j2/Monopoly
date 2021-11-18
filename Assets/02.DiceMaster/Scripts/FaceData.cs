// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Data class for faces 
    /// </summary>
    [System.Serializable]
    public class FaceData 
    {
        public int value = 0;                       // The value of this face (number)
        public Vector3 direction;                   // The UP direction for this face
        public Vector2 uvFaceCenter;                // The center UV coordinates of this face
        public float defaultOrientation = 0f;       // Some dice have pips rotated around
        public float pipSize = 1f;                  // Some pips are smaller
        public float pipStretch = 1f;               // Some pips are taller
        public float weight = 0f;                   // Increase this to add weight to the dice
    }

}

