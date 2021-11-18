// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DiceMaster
{
    /// <summary>
    /// Custom editor for the DiceDefinition class
    /// </summary>
    [CustomEditor(typeof(DiceDefinition))]
    public class DiceDefinitionEditor : Editor
    {
        private ReorderableList faceList;

        private void OnEnable()
        {
            faceList = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("faces"),
                    true, true, false, false);

            faceList.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, new GUIContent("Faces"));
                };

            faceList.elementHeight = 120;
            faceList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = faceList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    var dx = (int)rect.width;
                    var dy = EditorGUIUtility.singleLineHeight;

                    // Row: box
                    GUI.color = new Color(1, 0.7f, 0.7f);
                    var style = new GUIStyle(GUI.skin.GetStyle("box"));
                    style.padding = new RectOffset(0, 0, 0, 0);
                    GUI.Box(new Rect(rect.x, rect.y, dx, dy), "Face " + (index + 1), style);
                    GUI.color = Color.white;

                    // Row: value
                    rect.y += dy;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, dx, dy),
                        element.FindPropertyRelative("value"), new GUIContent("Value"));

                    // Row: direction
                    rect.y += dy;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, dx, dy),
                        element.FindPropertyRelative("direction"), new GUIContent("Direction"));

                    // Row: UVs
                    rect.y += dy;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, dx, dy),
                        element.FindPropertyRelative("uvFaceCenter"), GUIContent.none);

                    // Row: Orientation
                    rect.y += dy;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, dx, dy),
                        element.FindPropertyRelative("defaultOrientation"), new GUIContent("Orientation"));

                    // Row: Size & Stretch
                    rect.y += dy;
                    dx = (int)rect.width / 2;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, dx, dy),
                        element.FindPropertyRelative("pipSize"), new GUIContent("Size"));
                    EditorGUI.PropertyField(
                        new Rect(rect.x + dx, rect.y, dx, dy),
                        element.FindPropertyRelative("pipStretch"), new GUIContent("Stretch"));

                    // Row: Weight
                    rect.y += dy;
                    dx = (int)rect.width;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, dx, dy),
                        element.FindPropertyRelative("weight"), new GUIContent("Weight"));

                };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DiceDefinition diceDefinition = (DiceDefinition)target;

            // Update parameters
            diceDefinition.numberOfFaces = EditorGUILayout.IntSlider("Number of Faces", diceDefinition.numberOfFaces, 2, 20);
            diceDefinition.defaultPipsSize = EditorGUILayout.FloatField("Default Pip Size", diceDefinition.defaultPipsSize);
            diceDefinition.defaultPipsStretch = EditorGUILayout.FloatField("Default Pip Stretch", diceDefinition.defaultPipsStretch);
            diceDefinition.faceDistance = EditorGUILayout.FloatField("Face distance from center", diceDefinition.faceDistance);

            // Special cases
            diceDefinition.zeroInsteadOfTen = EditorGUILayout.Toggle("0 instead of 10", diceDefinition.zeroInsteadOfTen);
            diceDefinition.sixAndNineBars = EditorGUILayout.Toggle("Bars under 6 and 9", diceDefinition.sixAndNineBars);
            diceDefinition.pipsAtVertices = EditorGUILayout.Toggle("Pips placed at vertices", diceDefinition.pipsAtVertices);
            if (diceDefinition.pipsAtVertices)
            {
                diceDefinition.verticesNumber = EditorGUILayout.IntField("Number of vertices", diceDefinition.verticesNumber);
                diceDefinition.pipsRadius = EditorGUILayout.IntField("Radius of the vertex pips", diceDefinition.pipsRadius);
            }

            // Update number of faces
            if (diceDefinition.faces == null || diceDefinition.faces.Length != diceDefinition.numberOfFaces)
                diceDefinition.faces = new FaceData[diceDefinition.numberOfFaces];

            faceList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
