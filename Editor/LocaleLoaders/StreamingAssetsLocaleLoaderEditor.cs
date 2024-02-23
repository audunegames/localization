using Audune.Utils.UnityEditor;
using Audune.Utils.UnityEditor.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines an editor for a StreamingAssets locale loader
  [CustomEditor(typeof(StreamingAssetsLocaleLoader))]
  public class StreamingAssetsLocaleLoaderEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _priority;
    private SerializedProperty _executionMode;
    private SerializedProperty _directory;
    private SerializedProperty _pattern;
    private SerializedProperty _parser;

    // Foldout state of the editor
    private bool _loaderSettingsFoldout = true;
    private bool _executionSettingsFoldout = false;

    // Return the target object of the editor
    public new StreamingAssetsLocaleLoader target => serializedObject.targetObject as StreamingAssetsLocaleLoader;


    // OnEnable is called when the component becomes enabled
    protected void OnEnable()
    {
      // Initialize the properties
      _priority = serializedObject.FindProperty("_priority");
      _executionMode = serializedObject.FindProperty("_executionMode");
      _directory = serializedObject.FindProperty("_directory");
      _pattern = serializedObject.FindProperty("_pattern");
      _parser = serializedObject.FindProperty("_parser");
    }

    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUI.BeginChangeCheck();

      _loaderSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_loaderSettingsFoldout, "Loader Settings");
      if (_loaderSettingsFoldout)
      {
        EditorGUILayout.PropertyField(_directory);
        EditorGUILayout.PropertyField(_pattern);
        EditorGUILayout.PropertyField(_parser);

        EditorGUILayout.Space();
      }
      EditorGUI.EndFoldoutHeaderGroup();

      _executionSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_executionSettingsFoldout, "Execution Settings");
      if (_executionSettingsFoldout)
      {
        EditorGUILayout.PropertyField(_priority);
        EditorGUILayout.PropertyField(_executionMode);
      }
      EditorGUI.EndFoldoutHeaderGroup();

      if (EditorGUI.EndChangeCheck())
        serializedObject.ApplyModifiedProperties();
    }
  }
}