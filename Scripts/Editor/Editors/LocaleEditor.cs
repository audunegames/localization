using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Property drawer for locale 
  [CustomEditor(typeof(Locale))]
  public sealed class LocaleEditor : UnityEditor.Editor
  {
    // Properties of the object
    private SerializedProperty _code;
    private SerializedProperty _steamCode;
    private SerializedProperty _name;
    private SerializedProperty _strings;


    // Return the target object of the editor
    public Locale Target => serializedObject.targetObject as Locale;


    // Constructor
    public void OnEnable()
    {
      _code = serializedObject.FindProperty("_code");
      //_steamCode = serializedObject.FindProperty("_steamCode");
      _name = serializedObject.FindProperty("_name");
      _strings = serializedObject.FindProperty("_strings");
    }


    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

      EditorGUILayout.PropertyField(_code);
      //EditorGUILayout.PropertyField(_steamCode);
      EditorGUILayout.PropertyField(_name);

      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Tables", EditorStyles.boldLabel);

      EditorGUILayout.PropertyField(_strings);

      if (GUI.changed)
        serializedObject.ApplyModifiedProperties();
    }
  }
}