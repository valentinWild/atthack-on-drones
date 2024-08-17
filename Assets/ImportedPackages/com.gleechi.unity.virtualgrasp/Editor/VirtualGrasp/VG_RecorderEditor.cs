// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/*
namespace VirtualGrasp.Scripts
{
    [CustomEditor(typeof(VG_Recorder))]
    [CanEditMultipleObjects]
    public class VG_RecorderEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

            SerializedProperty replay = serializedObject.FindProperty("m_replayFromMemory");
            EditorGUILayout.PropertyField(replay);

            EditorGUI.BeginDisabledGroup(replay.boolValue);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_newRecordingPath"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_replayRecording"));

            EditorGUI.EndDisabledGroup();

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script", "m_replayFromMemory", "m_newRecordingPath", "m_replayRecording" });

            serializedObject.ApplyModifiedProperties();
        }
    }
}
*/

#endif