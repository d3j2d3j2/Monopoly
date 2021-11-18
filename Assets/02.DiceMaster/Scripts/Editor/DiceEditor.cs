// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using UnityEditor;

namespace DiceMaster
{
    /// <summary>
    /// Editor that defines how the Dice class appears.
    /// </summary>
    [CustomEditor(typeof(Dice))]
    [CanEditMultipleObjects]
    public class DiceEditor : Editor
    {
        void OnSceneGUI()
        {
            Dice dice = (Dice)target;
            DiceDefinition diceDefinition = dice.definition;

            Handles.color = Color.white;
            float labelDistance = 0.5f;

            if (dice.definition == null) return; // Should NEVER be null!
            if (dice.definition.faces == null) return;  // Should NEVER be null!

            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            for (int i = 0; i < diceDefinition.faces.Length; i++)
            {
                var face = diceDefinition.faces[i];
                var localDir = dice.transform.rotation * face.direction;
                var to = dice.transform.position + localDir * labelDistance;
                Handles.DrawDottedLine(dice.transform.position, to, 3f);
                Handles.Label(to, face.value.ToString(), style);
            }

            if (GUI.changed) 
                EditorUtility.SetDirty(target);
        }
    }
}