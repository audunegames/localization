using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines an editor for a persistent locale loader
  [CustomEditor(typeof(PersistentLocaleLoader))]
  public class PersistentLocaleLoaderEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _priority;
    private SerializedProperty _executionMode;
    private SerializedProperty _locales;

    private ReorderableList _localesList;

    // Foldout state of the editor
    private bool _loaderSettingsFoldout = true;
    private bool _executionSettingsFoldout = false;


    // Return the target object of the editor
    public new PersistentLocaleLoader target => serializedObject.targetObject as PersistentLocaleLoader;


    // OnEnable is called when the component becomes enabled
    protected void OnEnable()
    {
      // Initialize the properties
      _priority = serializedObject.FindProperty("_priority");
      _executionMode = serializedObject.FindProperty("_executionMode");
      _locales = serializedObject.FindProperty("_locales");

      // Initialize the list for the locales
      _localesList = new ReorderableListBuilder()
        .UsePropertyFieldForHeaderDrawer()
        .UsePropertyFieldForElementDrawer((property, index) => GUIContent.none)
        .Create(serializedObject, _locales);
    }

    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUI.BeginChangeCheck();

      _loaderSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_loaderSettingsFoldout, "Loader Settings");
      if (_loaderSettingsFoldout)
      {
        _localesList.DoLayoutList(Utils.UnityEditor.ReorderableListOptions.None);

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