using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardCard))]
public class CardEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BoardCard script = (BoardCard)target;
        if (script.isDebug)
        {
            script.atkValue = EditorGUILayout.LongField("Attack", script.atkValue);
            script.hpValue = EditorGUILayout.LongField("HP", script.hpValue);
            script.hpMaxValue = EditorGUILayout.LongField("HPMax", script.hpMaxValue);
            script.cleaveValue = EditorGUILayout.LongField("Cleave", script.cleaveValue);
            script.pierceValue = EditorGUILayout.LongField("Pierce", script.pierceValue);
            script.breakerValue = EditorGUILayout.LongField("Breaker", script.breakerValue);
            script.comboValue = EditorGUILayout.LongField("Combo", script.comboValue);
            script.overrunValue = EditorGUILayout.LongField("Overrun", script.overrunValue);
            script.shieldValue = EditorGUILayout.LongField("Shield", script.shieldValue);
            script.godSlayerValue = EditorGUILayout.LongField("GodSlayer", script.godSlayerValue);
        }
    }
}
