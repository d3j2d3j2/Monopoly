// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Data class that defines how a specific dice works
    /// </summary>
    public class DiceDefinition : ScriptableObject
    {
        public int numberOfFaces = 6;

        public float defaultPipsSize = 1f;              // Some dice have smaller pips
        public float defaultPipsStretch = 1f;          // Some dice have taller pips

        public FaceData[] faces;

        public float faceDistance = 1f;             // Set for dynamic, 3D, and quad faces 

        // Default special properties
        public bool sixAndNineBars = false;         // Use bars for 6 and 9, by default
        public bool zeroInsteadOfTen = false;       // Use 0 instead of 10, by default
        public bool pipsAtVertices = false;         // Place Pips at vertices instead of in the middle
        public int verticesNumber = 3;              // Used by pipsAtVertices
        public int pipsRadius = 170;                // Used by pipsAtVertices

    }
}

