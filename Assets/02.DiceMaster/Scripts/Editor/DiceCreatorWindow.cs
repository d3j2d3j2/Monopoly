// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using UnityEditor;

namespace DiceMaster
{
    /// <summary>
    /// Editor window for the DiceCreator
    /// </summary>
    public class DiceCreatorWindow : EditorWindow
    {
        string diceName = "New Dice";

        // Dice Parameters
        public DiceDefinition diceDefinition;
        public Mesh usedMesh;
        public Mesh collisionMesh;
        public Material usedMaterial;
        public Texture2D patternTexture;
        public PhysicMaterial physicMaterial;
        public float faceSizeMultiplier = 1;
        public int targetTextureSize = 1024;    // Default: just as the UVs

        // Face parameters
        public float[] facesPipOrientation;
        public float[] facesPipSize;
        public float[] facesPipStretch;
        public Sprite[] facesSprite;

        // Dynamic faces
        public FacesType facesType = FacesType.Texture;
        public GameObject[] dynamicFacesObjects;

        // Show parameters
        bool showAdditionalFaceOptions = true; // This is always true for now
        bool showFacesOptions = false;
        float facePreviewSize = 50f;
        float facesAreaMinHeight = 200f;
        float texturePreviewSize = 100f;

        // Generated assets
        public Texture2D generatedTexture = null;
        public GameObject generatedPrefab = null;

        // Internal state
        private DiceDefinition lastDiceDefinition = null;

        [MenuItem("Window/Dice Creator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(DiceCreatorWindow), false, "Dice Creator");
        }

        Vector2 scrollPos_main;
        Vector2 scrollPos_faces;
        void OnGUI()
        {

            // Show properties
            diceDefinition = (DiceDefinition)EditorGUILayout.ObjectField("Definition", diceDefinition, typeof(DiceDefinition), false);
            if (diceDefinition == null)
                return;

            scrollPos_main = EditorGUILayout.BeginScrollView(scrollPos_main, false, false);

            // Load defaults if something changed
            // or if our items got out of sync
            if (diceDefinition != lastDiceDefinition
                || diceDefinition.numberOfFaces != facesSprite.Length)
            {
                diceName = "New D" + diceDefinition.numberOfFaces;

                // Load high poly mesh as visual mesh
                var tmpPath = DM.Config.DefaultMeshesPathComplete + "dice_d" + diceDefinition.numberOfFaces + "_high.fbx";
                var tmpMesh = AssetDatabase.LoadAssetAtPath<Mesh>(tmpPath);
                if (tmpMesh != null) usedMesh = tmpMesh;
                else Debug.LogWarning("Trying to load default mesh at " + tmpPath + " but could not find it!");

                // Load low poly mesh as collision mesh
                tmpPath = DM.Config.DefaultMeshesPathComplete + "dice_d" + diceDefinition.numberOfFaces + "_low.fbx";
                tmpMesh = AssetDatabase.LoadAssetAtPath<Mesh>(tmpPath);
                if (tmpMesh != null) collisionMesh = tmpMesh;
                else Debug.LogWarning("Trying to load default collision mesh at " + tmpPath + " but could not find it!");

                // Default material
                if (usedMaterial == null)
                {
                    tmpPath = DM.Config.DefaultMaterialsPathComplete + "default_material.mat";
                    var tmpMat = AssetDatabase.LoadAssetAtPath<Material>(tmpPath);
                    if (tmpMat != null) usedMaterial = tmpMat;
                    else Debug.LogWarning("Trying to load default material at " + tmpPath + " but could not find it!");
                }

                // Get all default values
                facesPipOrientation = new float[diceDefinition.numberOfFaces];
                facesPipSize = new float[diceDefinition.numberOfFaces];
                facesPipStretch = new float[diceDefinition.numberOfFaces];
                dynamicFacesObjects = new GameObject[diceDefinition.numberOfFaces];
                for (int i = 0; i < diceDefinition.numberOfFaces; i++)
                {
                    facesPipOrientation[i] = diceDefinition.faces[i].defaultOrientation;
                    facesPipSize[i] = diceDefinition.faces[i].pipSize;
                    facesPipStretch[i] = diceDefinition.faces[i].pipStretch;
                    dynamicFacesObjects[i] = Resources.Load<GameObject>("DefaultDynamicFace");
                }

                SetPipsToDefaultNumbers();
                // Reset textures and prefab
                generatedPrefab = null;
                generatedTexture = null;

                lastDiceDefinition = diceDefinition;
            }

            diceName = EditorGUILayout.TextField("Name", diceName);
            patternTexture = (Texture2D)EditorGUILayout.ObjectField("Pattern", patternTexture, typeof(Texture2D), false);
            usedMesh = (Mesh)EditorGUILayout.ObjectField("Mesh", usedMesh, typeof(Mesh), false);
            collisionMesh = (Mesh)EditorGUILayout.ObjectField("Collision", collisionMesh, typeof(Mesh), false);
            usedMaterial = (Material)EditorGUILayout.ObjectField("Material", usedMaterial, typeof(Material), false);
            physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physic Material", physicMaterial, typeof(PhysicMaterial), false);

            targetTextureSize = EditorGUILayout.IntField("Target texture size", targetTextureSize);
            if (targetTextureSize > 2048) targetTextureSize = 2048;
            if (targetTextureSize < 256) targetTextureSize = 256;

            // Actions
            if (GUILayout.Button("Generate Dice Texture"))
                generatedTexture = DiceCreator.GenerateFinalTexture(
                    diceName, diceDefinition, facesSprite, patternTexture,
                    targetTextureSize,
                    facesPipOrientation,
                    facesPipSize,
                    facesPipStretch,
                    facesType);

            EditorGUI.BeginDisabledGroup(generatedTexture == null);
            if (GUILayout.Button("Generate Dice Prefab"))
                generatedPrefab = DiceCreator.GenerateFinalPrefab(
                    diceName,
                    diceDefinition,
                    generatedTexture,
                    usedMesh, collisionMesh,
                    usedMaterial, physicMaterial,
                    facesType,
                    dynamicFacesObjects,
                    facesSprite,
                    facesPipOrientation,
                    facesPipSize,
                    facesPipStretch);
            EditorGUI.EndDisabledGroup();

            // Show number sprites
            showFacesOptions = EditorGUILayout.Toggle("Show faces options", showFacesOptions);

            if (diceDefinition != null)
            {
                if (showFacesOptions)
                {
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                    this.facesType = (FacesType)EditorGUILayout.EnumPopup("Faces type", (System.Enum)this.facesType);

                    if (facesType == FacesType.Dynamic || facesType == FacesType.Texture)
                    {
                        DrawChangePipsButtons();
                    }

                    scrollPos_faces = EditorGUILayout.BeginScrollView(scrollPos_faces, false, false, GUILayout.MinHeight(facesAreaMinHeight));

                    for (int i = 0; i < diceDefinition.numberOfFaces; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (facesType == FacesType.Dynamic || facesType == FacesType.Texture)
                        {
                            facesSprite[i] = (Sprite)EditorGUILayout.ObjectField(facesSprite[i], typeof(Sprite), false,
                                GUILayout.Height(facePreviewSize),
                                GUILayout.Width(facePreviewSize));
                        }
                        else if (facesType == FacesType.Custom)
                        {

                            dynamicFacesObjects[i] = (GameObject)EditorGUILayout.ObjectField(
                                dynamicFacesObjects[i], typeof(GameObject), false);
                        }

                        if (showAdditionalFaceOptions)
                        {
                            int labelWidth = 120;
                            int valueWidth = 30;

                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Orientation", GUILayout.MaxWidth(labelWidth));
                            facesPipOrientation[i] = EditorGUILayout.FloatField(facesPipOrientation[i], GUILayout.MaxWidth(valueWidth));
                            facesPipOrientation[i] = Mathf.Clamp(facesPipOrientation[i], -360, 360);
                            EditorGUILayout.EndHorizontal();

                            labelWidth = 50;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Size", GUILayout.MaxWidth(labelWidth));
                            facesPipSize[i] = EditorGUILayout.FloatField(facesPipSize[i], GUILayout.MaxWidth(valueWidth));
                            facesPipSize[i] = Mathf.Clamp(facesPipSize[i], 0.1f, 3f);

                            labelWidth = 50;
                            EditorGUILayout.LabelField("Stretch", GUILayout.MaxWidth(labelWidth));
                            facesPipStretch[i] = EditorGUILayout.FloatField(facesPipStretch[i], GUILayout.MaxWidth(valueWidth));
                            facesPipStretch[i] = Mathf.Clamp(facesPipStretch[i], 0.1f, 3f);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndVertical();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

            // Preview
            EditorGUILayout.BeginHorizontal();
            if (generatedPrefab != null)
                GUILayout.Label(AssetPreview.GetAssetPreview(generatedPrefab));
            if (generatedTexture != null)
                GUILayout.Label(generatedTexture, GUILayout.Width(texturePreviewSize));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        #region Pip Faces Switch
        void DrawChangePipsButtons()
        {
            var content = new GUIContent("Quick faces buttons", "These buttons will set all pip sprites to pre-defined sprites.");
            GUILayout.Label(content);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Numbers"))
                SetPipsToDefaultNumbers();

            if (GUILayout.Button("Pips"))
                SetPipsToDefaultPips();

            if (GUILayout.Button("Dot"))
                SetAllPipsTo("dot green");

            if (GUILayout.Button("Cross"))
                SetAllPipsTo("cross red");
            GUILayout.EndHorizontal();
        }

        void SetAllPipsTo(string spriteName)
        {
            facesSprite = new Sprite[diceDefinition.numberOfFaces];
            for (int i = 0; i < facesSprite.Length; i++)
                SetSprite(i, spriteName);
        }

        void SetPipsToDefaultNumbers()
        {
            facesSprite = new Sprite[diceDefinition.numberOfFaces];
            for (int i = 0; i < facesSprite.Length; i++)
            {
                string spriteName = (i + 1).ToString();
                if (i == 9 && diceDefinition.zeroInsteadOfTen) spriteName = "0";
                else if ((i == 5 || i == 8) && diceDefinition.sixAndNineBars) spriteName += "_";
                spriteName += " black";
                SetSprite(i, spriteName);
            }
        }

        void SetPipsToDefaultPips()
        {
            facesSprite = new Sprite[diceDefinition.numberOfFaces];
            for (int i = 0; i < facesSprite.Length; i++)
            {
                string spriteName = (i + 1).ToString() + " pip black";
                SetSprite(i, spriteName);
            }
        }

        void SetSprite(int index, string spriteName)
        {
            var path = DM.Config.DefaultPipsPathComplete + spriteName + ".png";
            facesSprite[index] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (facesSprite[index] == null)
                Debug.LogWarning("Trying to load default face sprite at " + path + " but could not find it!");
        }
        #endregion
    }
}
