using Audune.Localization.Messages;
using Audune.Utils.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines an editor for locales
  [CustomEditor(typeof(Locale))]
  public sealed class LocaleEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _name;
    private SerializedProperty _code;
    private SerializedProperty _altCodes;


    // Return the target object of the editor
    public Locale Target => serializedObject.targetObject as Locale;


    // Constructor
    public void OnEnable()
    {
      // Initialize the properties
      _name = serializedObject.FindProperty("_name");
      _code = serializedObject.FindProperty("_code");
      _altCodes = serializedObject.FindProperty("_altCodes");
    }


    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      Rect rect;

      serializedObject.Update();

      EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

      EditorGUILayout.PropertyField(_name);
      EditorGUILayout.PropertyField(_code);
      EditorGUILayout.PropertyField(_altCodes, new GUIContent("Alternative Codes", _altCodes.tooltip));


      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Tables", EditorStyles.boldLabel); 

      EditorGUILayout.PropertyField(_strings);

      if (GUI.changed)
        serializedObject.ApplyModifiedProperties();
    }
  }
}