// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster { 

    /// <summary>
    /// Utility class with actions useful to the tool's development
    /// </summary>
    public class DiceTester : MonoBehaviour {

        [ContextMenu("Check Normals")]
	    void CheckNormals () {
            MeshCollider mc = GetComponent<MeshCollider>();
            Vector3[] normals = mc.sharedMesh.normals;
            foreach(var n in normals)
                Debug.Log(n);
	    }
	
    }

}
