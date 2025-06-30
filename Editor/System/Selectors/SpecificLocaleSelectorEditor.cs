using UnityEditor;

namespace Audune.Localization
{
  // Class that defines an editor for a specific locale selector
  [CustomEditor(typeof(SpecificLocaleSelector))]
  public class SpecificLocaleSelectorEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _priority;
    private SerializedProperty _executionMode;
    private SerializedProperty _locale;

    // Foldout state of the editor
    private bool _selectorSettingsFoldout = true;
    private bool _executionSettingsFoldout = false;


    // Return the target object of the editor
    public new SpecificLocaleSelector target => serializedObject.targetObject as SpecificLocaleSelector;


    // OnEnable is called when the component becomes enabled
    protected void OnEnable()
    {
      // Initialize the properties
      _priority = serializedObject.FindProperty("_priority");
      _executionMode = serializedObject.FindProperty("_executionMode");
      _locale = serializedObject.FindProperty("_locale");
    }

    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUI.BeginChangeCheck();

      _selectorSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_selectorSettingsFoldout, "Selector Settings");
      if (_selectorSettingsFoldout)
      {
        EditorGUILayout.PropertyField(_locale);

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