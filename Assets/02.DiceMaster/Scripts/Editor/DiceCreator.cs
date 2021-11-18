// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using UnityEditor;
using System.IO;

namespace DiceMaster
{
    /// <summary>
    /// Static editor class responsible for creating new dice prefabs and their textures
    /// </summary>
    public class DiceCreator
    {
        // Parameters
        static int original_uv_size = 1024;

        /// <summary>
        /// Generate a prefab of the dice and save it among the assets
        /// </summary>
        /// <param name="diceName">Name of the dice (and asset) to generate.</param>
        /// <param name="diceDefinition">Definition to use.</param>
        /// <param name="finalTexture">Texture of the dice.</param>
        /// <param name="finalMesh">Mesh of the dice.</param>
        /// <param name="collisionMesh">Collision mesh of the dice.</param>
        /// <param name="finalMaterial">Material of the dice.</param>
        /// <param name="physicMaterial">Physics material of the dice.</param>
        /// <param name="facesType">The type determining how to handle faces.</param>
        /// <param name="dynamicFacesObjects">Dynamic objects to place on faces.</param>
        /// <returns>The new dice prefab GameObject.</returns>
        public static GameObject GenerateFinalPrefab(
            string diceName,
            DiceDefinition diceDefinition,
            Texture2D finalTexture,
            Mesh finalMesh, Mesh collisionMesh,
            Material finalMaterial, PhysicMaterial physicMaterial,
            FacesType facesType, GameObject[] dynamicFacesObjects,
            Sprite[] facesSprite,
            float[] facesPipOrientation,
            float[] facesPipSize,
            float[] facesPipStretch)
        {
            Debug.Log("Generating Dice prefab...");

            // Get the base prefab
            GameObject dicePrefabBase = Resources.Load("DicePrefabBase") as GameObject;
            if (dicePrefabBase == null)
            {
                Debug.LogError("No example prefab 'DicePrefabBase' found in the resources folder! Make sure not to delete or move it!");
                return null;
            }

            // Create the new dice GameObject
            GameObject newDiceGo = GameObject.Instantiate(dicePrefabBase) as GameObject;
            var newDice = newDiceGo.GetComponent<Dice>();
            newDice.definition = diceDefinition;

            // Set meshes
            if (finalMesh == null)
            {
                Debug.LogError("No mesh was selected in the Dice Creator! Make sure you choose one.");
                return null;
            }
            if (collisionMesh == null)
            {
                Debug.LogWarning("No collision mesh was selected in the Dice Creator! Your new dice prefab won't be able to react to collisions.");
            }
            if (physicMaterial == null)
            {
                Debug.LogWarning("No physical material was selected in the Dice Creator! Your new dice prefab will behave with default values.");
            }
            GameObject meshGo = newDiceGo.transform.Find("Mesh").gameObject;
            var meshFilter = meshGo.GetComponent<MeshFilter>();
            var meshCollider = newDiceGo.GetComponent<MeshCollider>();
            meshFilter.mesh = finalMesh;
            meshCollider.sharedMesh = collisionMesh;
            meshCollider.material = physicMaterial;

            // Instantiate the new material and save it in the output folder
            if (finalMaterial == null)
            {
                Debug.LogError("No material was assigned to the dice creator! Cannot generate prefab!");
                return null;
            }
            Material mat = new Material(finalMaterial);
            mat.SetTexture("_MainTex", finalTexture);
            meshGo.GetComponent<MeshRenderer>().material = mat;
            string matPath = DM.Config.OutputPathComplete + "Materials";
            if (!Directory.Exists(matPath))
            {
                Debug.LogError("Cannot find directory at " + matPath + ".\nDid you move the folders? Make sure to update the DiceConfig script if you did!");
                return null;
            }
            matPath += "/" + diceName + ".mat";
            AssetDatabase.CreateAsset(mat, matPath);
            EditorUtility.SetDirty(mat);

            // Check if we need to add something to the different faces
            if (facesType == FacesType.Dynamic || facesType == FacesType.Custom)
            {
                newDice.dynamicFaceGos = new GameObject[diceDefinition.numberOfFaces];
            }
            for (int faceIndex = 0; faceIndex < diceDefinition.faces.Length; faceIndex++)
            {
                var vectorToFace = diceDefinition.faces[faceIndex].direction * diceDefinition.faceDistance;

                // Check if we need to add dynamic faces
                if (facesType == FacesType.Dynamic || facesType == FacesType.Custom)
                {
                    var dynamicFaceGo = GameObject.Instantiate(dynamicFacesObjects[faceIndex]) as GameObject;
                    if (facesType == FacesType.Dynamic)
                        dynamicFaceGo.name = "DynamicFace_" + diceDefinition.faces[faceIndex].value;
                    else
                        dynamicFaceGo.name = "CustomFace_" + diceDefinition.faces[faceIndex].value;

                    var dynamicFaceTr = dynamicFaceGo.transform;

                    dynamicFaceTr.SetParent(newDiceGo.transform);

                    dynamicFaceTr.localPosition = vectorToFace;

                    if (facesType == FacesType.Dynamic)
                        dynamicFaceTr.forward = -vectorToFace;
                    else
                        dynamicFaceTr.forward = vectorToFace;

                    dynamicFaceTr.Rotate(Vector3.forward, facesPipOrientation[faceIndex], Space.Self);

                    dynamicFaceTr.localScale = new Vector3(1, facesPipStretch[faceIndex], 1);
                    //dynamicFaceTr.localScale *= diceDefinition.defaultPipsSize;
                    dynamicFaceTr.localScale *= facesPipSize[faceIndex];
                    dynamicFaceTr.localScale *= DiceConfig.Instance.dynamicFaceSizeMultiplier;

                    if (facesType == FacesType.Dynamic)
                    {
                        var dynamicFace = dynamicFaceGo.GetComponent<DynamicFace>();
                        if (dynamicFace != null)
                            dynamicFace.SetSprite(facesSprite[faceIndex]);
                    }

                    newDice.dynamicFaceGos[faceIndex] = dynamicFaceGo;
                }

                // Check if we need to add weights
                if (diceDefinition.faces[faceIndex].weight > 0)
                {
                    Debug.Log("Adding weight to face " + (faceIndex + 1) + " at axis " + (-diceDefinition.faces[faceIndex].direction));
                    var weightGo = new GameObject("Weight " + (faceIndex + 1).ToString());
                    weightGo.transform.parent = newDiceGo.transform;
                    var weightRb = weightGo.AddComponent<Rigidbody>();
                    weightRb.mass = diceDefinition.faces[faceIndex].weight;

                    // Note that the weight is at the other side fo the face
                    weightRb.transform.localPosition = -vectorToFace;

                    var weightJoint = weightGo.AddComponent<FixedJoint>();
                    weightJoint.connectedBody = newDiceGo.GetComponent<Rigidbody>();
                }
            }

            string targetPath = DM.Config.OutputPathComplete + "Prefabs";
            if (!Directory.Exists(targetPath))
            {
                Debug.LogError("Cannot find directory at " + targetPath + ".\nDid you move the folders? Make sure to update DiceConfig if you did!");
                return null;
            }
            targetPath += "/" + diceName + ".prefab";

            GameObject newDiePrefab = null;
            bool createIt = false;
            if (AssetDatabase.LoadAssetAtPath(targetPath, typeof(GameObject)))
            {
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "The prefab already exists. Do you want to overwrite it?",
                    "Yes",
                    "No"))
                    createIt = true;
            }
            else
                createIt = true;

            if (createIt)
            {
                var prefab = PrefabUtility.CreateEmptyPrefab(targetPath);
                newDiePrefab = PrefabUtility.ReplacePrefab(newDiceGo, prefab, ReplacePrefabOptions.ReplaceNameBased);
                Debug.Log("Final dice prefab " + diceName + " generated! \n " + targetPath);
            }
            else
            {
                Debug.LogWarning("No dice prefab generated.");
            }


            // Remove temporary objects
            GameObject.DestroyImmediate(newDiceGo);

            // At the end, save and refresh
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newDiePrefab;
        }

        /// <summary>
        /// Generates a complete texture for the dice and save it
        /// </summary>
        /// <param name="diceName">Name of the dice to generate.</param>
        /// <param name="diceDefinition">Definition to use.</param>
        /// <param name="pipSprites">Sprites to use as pips for the faces of the dice.</param>
        /// <param name="patternTexture">Base pattern to use.</param>
        /// <param name="targetTextureSize">Size of the target (final) texture</param>
        /// <param name="facesPipOrientation">Orientation of each pip.</param>
        /// <param name="facesPipSize">Multiplier for the size of each pip.</param>
        /// <param name="facesPipStretch">Multiplier for the stretch of each pip.</param>
        /// <returns>The final generated texture.</returns>
        public static Texture2D GenerateFinalTexture(
                string diceName,
                 DiceDefinition diceDefinition,
                 Sprite[] pipSprites,
                 Texture2D patternTexture,
                int targetTextureSize,
                float[] facesPipOrientation,
                float[] facesPipSize,
                float[] facesPipStretch,
            FacesType facesType)
        {
            if (facesType == FacesType.Dynamic || facesType == FacesType.Custom)
            {
                Debug.Log("When using non-texture faces, the final texture will just be the pattern.");
                if (patternTexture == null)
                {
                    Debug.LogError("Make sure you select a pattern!");
                    return null;
                }
                return patternTexture;
            }

            Debug.Log("Generating texture...");
            // Check readability
            if (!ReadabilityIsOk(pipSprites, patternTexture))
                return null;

            // Define the size of the final texture
            // Changed if we need to have a different size for the target texture
            int target_width = targetTextureSize;
            int target_height = target_width;

            // The UVs are written on texture of fixed size, so take the ratio into account
            float uv_to_target_ratio_x = target_width * 1f / original_uv_size;
            float uv_to_target_ratio_y = target_height * 1f / original_uv_size;

            // Define input and output
            Color[] sourcePixels;
            var newTexture = new Texture2D(target_width, target_height, TextureFormat.ARGB32, false, true);
            newTexture.name = "New Texture";
            var targetPixels = newTexture.GetPixels();

            // First write the pattern below
            if (patternTexture != null)
            {
                int source_pattern_width = patternTexture.width;
                int source_pattern_height = patternTexture.height;

                float pattern_to_target_x = target_width / source_pattern_width;
                float pattern_to_target_y = target_height / source_pattern_height;

                sourcePixels = patternTexture.GetPixels();
                for (int target_x = 0; target_x < target_width; target_x++)
                    for (int target_y = 0; target_y < target_height; target_y++)
                    {
                        // Target to source
                        int source_x = Mathf.FloorToInt(target_x / pattern_to_target_x);
                        int source_y = Mathf.FloorToInt(target_y / pattern_to_target_y);

                        // Clamp if out of range
                        source_x = Mathf.Clamp(source_x, 0, source_pattern_width - 1);
                        source_y = Mathf.Clamp(source_y, 0, source_pattern_height - 1);

                        // Copy
                        var p = GetPixel(sourcePixels, source_pattern_width, source_pattern_height, source_x, source_y);
                        SetPixel(p, targetPixels, target_width, target_height, target_x, target_y);
                    }
            }

            // Then add all the face pips
            for (int faceIndex = 0; faceIndex < diceDefinition.numberOfFaces; faceIndex++)
            {
                // Define the size of each face texture
                // This does not change the size of the final pip, but it changes the size of the available space
                // This is set according to how big the pips are needed (if the size gets out of scope)
                float faceSizeMultiplier = target_width / 256; // FIXED, to make sure it works for different sizes
                int target_face_width = 256;    // FIXED
                int target_face_height = target_face_width;

                // Size multiplier is taken into account, since we need to have more space
                target_face_width = Mathf.FloorToInt(target_face_width * faceSizeMultiplier);
                target_face_height = Mathf.FloorToInt(target_face_height * faceSizeMultiplier);

                // If we need to place them at vertices, do a special method
                if (diceDefinition.pipsAtVertices)
                {
                    // Compute UV for each vertex
                    // We will place them in a radial manner, starting from the top (we add 90 degrees for that)
                    float angle_delta = 360 / diceDefinition.verticesNumber;
                    float angle_zero_offset = 90;
                    float radius = diceDefinition.pipsRadius;
                    for (int v = 0; v < diceDefinition.verticesNumber; v++)
                    {
                        // We get the other faces
                        int other_face_index = faceIndex + 1 + v;
                        if (other_face_index >= diceDefinition.numberOfFaces)
                            other_face_index -= diceDefinition.numberOfFaces;

                        var pipSprite = pipSprites[other_face_index];
                        if (pipSprite == null) continue;    // No pip for this face
                        var faceData = diceDefinition.faces[faceIndex];

                        int even_correction = ((faceIndex + 1) % 2 == 0 ? 1 : 0); // Even degrees need a correction of 1 cycle
                        int clockwise = facesPipOrientation[faceIndex] == 180 ? -1 : 1; // Change direction based on the orientation
                        float angle_degrees = angle_zero_offset + (angle_delta * clockwise * (v + even_correction)) + facesPipOrientation[faceIndex];
                        float angle_radians = angle_degrees / 180 * Mathf.PI;
                        float uv_x = faceData.uvFaceCenter.x + radius * Mathf.Cos(angle_radians);
                        float uv_y = faceData.uvFaceCenter.y + radius * Mathf.Sin(angle_radians);

                        var tmpPipTexture = PrepareTextureFromSprite(pipSprite, -(angle_delta * clockwise * (v + even_correction)) + facesPipOrientation[faceIndex]);
                        tmpPipTexture.name = "Pip " + faceIndex;
                        sourcePixels = tmpPipTexture.GetPixels();
                        int source_width = tmpPipTexture.width;
                        int source_height = tmpPipTexture.height;

                        float size = diceDefinition.defaultPipsSize * facesPipSize[faceIndex] / faceSizeMultiplier;
                        int offset_x = GetOffset(size, target_face_width, uv_x, uv_to_target_ratio_x);
                        float multiplier_x = GetMultiplier(size, target_face_width, source_width, uv_to_target_ratio_x);

                        size = size * diceDefinition.defaultPipsStretch * facesPipStretch[faceIndex];
                        int offset_y = GetOffset(size, target_face_height, uv_y, uv_to_target_ratio_y);
                        float multiplier_y = GetMultiplier(size, target_face_height, source_height, uv_to_target_ratio_y);

                        CopyPixels(sourcePixels, targetPixels,
                            source_width, source_height,
                            target_face_width, target_face_height,
                            target_width, target_height,
                            multiplier_x, multiplier_y,
                            offset_x, offset_y
                            );

                        // Cleanup
                        GameObject.DestroyImmediate(tmpPipTexture);
                    }

                }
                // Normal method: place at UV points
                else
                {
                    var pipSprite = pipSprites[faceIndex];
                    if (pipSprite == null) continue;    // No pip for this face
                    var faceData = diceDefinition.faces[faceIndex];

                    // Get the pip texture
                    int previous_width = pipSprite.texture.width;
                    int previous_height = pipSprite.texture.height;

                    var tmpPipTexture = PrepareTextureFromSprite(pipSprite, facesPipOrientation[faceIndex]);
                    tmpPipTexture.name = "Pip " + faceIndex;
                    sourcePixels = tmpPipTexture.GetPixels();
                    int source_width = tmpPipTexture.width;
                    int source_height = tmpPipTexture.height;

                    // Take into account that the size may have changed
                    float new_ratio_x = source_width * 1f / previous_width;
                    float new_ratio_y = source_height * 1f / previous_height;
                    target_face_width = (int)(target_face_width * new_ratio_x);
                    target_face_height = (int)(target_face_height * new_ratio_y);

                    float size = diceDefinition.defaultPipsSize * facesPipSize[faceIndex] / faceSizeMultiplier;
                    int offset_x = GetOffset(size, target_face_width, faceData.uvFaceCenter.x, uv_to_target_ratio_x);
                    float multiplier_x = GetMultiplier(size, target_face_width, source_width, uv_to_target_ratio_x);

                    size = size * diceDefinition.defaultPipsStretch * facesPipStretch[faceIndex];
                    int offset_y = GetOffset(size, target_face_height, faceData.uvFaceCenter.y, uv_to_target_ratio_y);
                    float multiplier_y = GetMultiplier(size, target_face_height, source_height, uv_to_target_ratio_y);

                    CopyPixels(sourcePixels, targetPixels,
                        source_width, source_height,
                        target_face_width, target_face_height,
                        target_width, target_height,
                        multiplier_x, multiplier_y,
                        offset_x, offset_y
                        );

                    // Cleanup
                    GameObject.DestroyImmediate(tmpPipTexture);
                }
            }
            newTexture.SetPixels(targetPixels);
            newTexture.Apply(false);

            // At the end, save and reload
            newTexture.name = diceName + "_Texture";
            bool canSave = SaveTextureToFile(newTexture, diceName);
            if (canSave)
            {
                //EditorUtility.SetDirty(newTexture);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GameObject.DestroyImmediate(newTexture);  // Cleaning-up the now saved texture for reload
                newTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(
                    DM.Config.OutputPathComplete + "Textures/" + diceName + ".png", typeof(Texture2D));
                newTexture.name = diceName + "_Texture RELOAD";
            }

            return newTexture;
        }

        private static int GetOffset(float pip_size, int target_face_length, float uv_offset, float uv_to_target_ratio)
        {
            float zero_offset = target_face_length * (-pip_size / 2f);
            float total_offset = uv_offset + zero_offset;
            return (int)(total_offset * uv_to_target_ratio);
        }

        private static float GetMultiplier(float pip_size, int target_length, float source_length, float uv_to_target_ratio)
        {
            float ratio = target_length * 1f / source_length;
            return ratio * pip_size * uv_to_target_ratio;
        }

        private static void CopyPixels(Color[] sourcePixels, Color[] targetPixels,
            int source_width, int source_height,
            int target_width, int target_height,
            int target_total_width, int target_total_height,
            float ratio_x, float ratio_y,
            int offset_x, int offset_y)
        {
            for (int target_x = 0; target_x < target_width; target_x++)
            {
                for (int target_y = 0; target_y < target_height; target_y++)
                {
                    int source_x = Mathf.FloorToInt(target_x / ratio_x);
                    int source_y = Mathf.FloorToInt(target_y / ratio_y);

                    // Skip if out of range
                    if (source_x >= source_width
                         || source_y >= source_height)
                        continue;

                    Color pix = GetPixel(sourcePixels, source_width, source_height, source_x, source_y);
                    if (pix.a == 0) continue;  // Only visible stuff!

                    int target_total_x = target_x + offset_x;
                    int target_total_y = target_y + offset_y;

                    // Clamp if out of range
                    target_total_x = Mathf.Clamp(target_total_x, 0, target_total_width - 1);
                    target_total_y = Mathf.Clamp(target_total_y, 0, target_total_height - 1);

                    SetPixel(pix, targetPixels, target_total_width, target_total_height, target_total_x, target_total_y);
                }
            }
        }

        private static Texture2D PrepareTextureFromSprite(Sprite sprite, float orientation = 0)
        {
            // Get pixels and create a texture
            var sourcePixels = sprite.texture.GetPixels((int)sprite.rect.x,
                                                        (int)sprite.rect.y,
                                                        (int)sprite.rect.width,
                                                        (int)sprite.rect.height);

            // Transform the texture (use a temporary texture for that)
            int source_w = (int)sprite.rect.width;
            int source_h = (int)sprite.rect.height;

            Texture2D tmpTexture = null;
            if (orientation != 0)
            {
                var rotationTextureMultiplier = DM.Config.rotationTextureMultiplier;

                int target_w = (int)(source_w * rotationTextureMultiplier);
                int target_h = (int)(source_h * rotationTextureMultiplier);

                tmpTexture = new Texture2D(target_w, target_h, TextureFormat.ARGB32, false, true);
                tmpTexture.name = "Tmp tex";

                // Initialise to empty
                var targetPixels = new Color[target_w * target_h];
                var emptyColor = new Color(0, 0, 0, 0);
                for (int i = 0; i < targetPixels.Length; i++)
                    targetPixels[i] = emptyColor;

                // Copy all pixels to the new texture
                CopyPixels(sourcePixels, targetPixels, source_w, source_h,
                        target_w, target_h,
                        target_w, target_h, 1, 1,
                      (int)(0.5f * source_w * (rotationTextureMultiplier - 1f)),
                      (int)(0.5f * source_h * (rotationTextureMultiplier - 1f)));

                tmpTexture.SetPixels(targetPixels);
                tmpTexture = RotateTexture(tmpTexture, orientation);
            }
            else
            {
                tmpTexture = new Texture2D(source_w, source_h, TextureFormat.ARGB32, false, true);
                tmpTexture.name = "Tmp tex";
                tmpTexture.SetPixels(sourcePixels);
            }
            tmpTexture.Apply();
            return tmpTexture;
        }

        private static bool ReadabilityIsOk(Sprite[] numbersSprites, Texture2D patternTexture)
        {
            for (int i = 0; i < numbersSprites.Length; i++)
            {
                if (numbersSprites[i] == null) continue;
                try
                {
                    numbersSprites[i].texture.GetPixel(0, 0);
                }
                catch (UnityException)
                {
                    Debug.LogError("Face texture " + i + " is not read/write enabled! Make sure it is!");
                    return false;
                }
            }

            try
            {
                if (patternTexture != null)
                    patternTexture.GetPixel(0, 0);
            }
            catch (UnityException)
            {
                Debug.LogError("Pattern texture is not read/write enabled! Make sure it is!");
                return false;
            }
            return true;
        }

        private static Color GetPixel(Color[] pixels, int width, int height, float x, float y)
        {
            Color pix;
            int x1 = (int)Mathf.Floor(x);
            int y1 = (int)Mathf.Floor(y);

            if (x1 > width || x1 < 0 ||
               y1 > height || y1 < 0)
                pix = Color.clear;
            else
                pix = pixels[x1 + y1 * width];
            return pix;
        }

        private static Color GetPixel(Texture2D tex, float x, float y, bool bilinear = false)
        {
            Color pix;
            int x1 = (int)Mathf.Floor(x);
            int y1 = (int)Mathf.Floor(y);

            if (x1 > tex.width || x1 < 0 ||
               y1 > tex.height || y1 < 0)
                pix = Color.clear;
            else
                pix = bilinear
                    ? tex.GetPixelBilinear(x1 * 1f / tex.width, y1 * 1f / tex.height)
                    : tex.GetPixel(x1, y1);

            return pix;
        }


        private static void SetPixel(Color pix, Color[] pixels, int width, int height, float x, float y)
        {
            int x1 = (int)Mathf.Floor(x);
            int y1 = (int)Mathf.Floor(y);
            if (!(x1 > width || x1 < 0 ||
               y1 > height || y1 < 0))
                pixels[x1 + y1 * width] = pix;
        }

        private static Texture2D RotateTexture(Texture2D tex, float angle)
        {
            Texture2D rotTex = new Texture2D(tex.width, tex.height);
            rotTex.name = "Rot tex";
            float x1, y1;

            int w = tex.width;
            int h = tex.height;
            float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
            float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);

            float x0 = w / 2;
            float y0 = h / 2;

            for (x1 = 0; x1 < w; x1++)
            {
                for (y1 = 0; y1 < h; y1++)
                {
                    var x2 = cos * (x1 - x0) - sin * (y1 - y0) + x0;
                    var y2 = sin * (x1 - x0) + cos * (y1 - y0) + y0;
                    rotTex.SetPixel(
                        Mathf.RoundToInt(x1),
                        Mathf.RoundToInt(y1),
                        GetPixel(tex, x2, y2, bilinear: true));
                }
            }

            GameObject.DestroyImmediate(tex);   // CLEANUP - the original texture is destroyed!
            rotTex.Apply();
            return rotTex;
        }

        private static float rot_x(float cos, float sin, float x, float y)
        {
            return x * cos + y * (-sin);
        }
        private static float rot_y(float cos, float sin, float x, float y)
        {
            return x * sin + y * cos;
        }

        /// <summary>
        /// Save the given dice texture to the output folder.
        /// </summary>
        /// <param name="texture">Texture to save.</param>
        /// <param name="diceName">Name of the dice this texture is for.</param>
        public static bool SaveTextureToFile(Texture2D texture, string diceName)
        {
            string targetPath = DM.Config.OutputDataPathComplete + "Textures";
            if (!Directory.Exists(targetPath))
            {
                Debug.LogError("Cannot save texture to path " + targetPath + ".\nDid you move the folders? Make sure to update DiceConfig if you did!");
                return false;
            }
            targetPath += "/" + diceName + ".png";

            var bytes = texture.EncodeToPNG();
            var file = File.Open(targetPath, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
            Debug.Log("Final dice texture " + diceName + " saved! \n " + targetPath);
            return true;
        }

    }
}
