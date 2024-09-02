using UnityEditor;

namespace Audune.Localization
{
  // Class that defines an editor for a command line locale loader
  [CustomEditor(typeof(CommandLineLocaleSelector))]
  public class CommandLineLocaleSelectorEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _priority;
    private SerializedProperty _executionMode;
    private SerializedProperty _argument;

    // Foldout state of the editor
    private bool _selectorSettingsFoldout = true;
    private bool _executionSettingsFoldout = false;


    // Return the target object of the editor
    public new CommandLineLocaleSelector target => serializedObject.targetObject as CommandLineLocaleSelector;


    // OnEnable is called when the component becomes enabled
    protected void OnEnable()
    {
      // Initialize the properties
      _priority = serializedObject.FindProperty("_priority");
      _executionMode = serializedObject.FindProperty("_executionMode");
      _argument = serializedObject.FindProperty("_argument");
    }

    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUI.BeginChangeCheck();

      _selectorSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_selectorSettingsFoldout, "Selector Settings");
      if (_selectorSettingsFoldout)
      {
        EditorGUILayout.PropertyField(_argument);

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