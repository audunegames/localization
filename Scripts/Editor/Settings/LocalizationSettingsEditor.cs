using Audune.Utils.Collections.Editor;
using Audune.Utils.Types;
using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Audune.Localization.Settings.Editor
{
  // Class that defines an editor for localization settings
  [CustomEditor(typeof(LocalizationSettings))]
  public sealed class LocalizationSettingsEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _localeLoaders;
    private SerializedProperty _localeSelectors;

    // Reorderable lists of the editor
    private ReorderableList _localeLoadersList;
    private ReorderableList _localeSelectorsList;


    // Return the target object of the editor
    public LocalizationSettings Target => serializedObject.targetObject as LocalizationSettings;


    // Constructor
    public void OnEnable()
    {
      // Initialize the properties
      _localeLoaders = serializedObject.FindProperty("_localeLoaders");
      _localeSelectors = serializedObject.FindProperty("_localeSelectors");

      // Initialize the reorderable lists
      _localeLoadersList = ReorderableDropdownListBuilder.CreateForScriptableObjectTypes<LocaleLoader>(TypeDisplayStringOptions.DontShowNamespace)
        .UsePropertyFieldForElementDrawerWithHeader((element, index) => new GUIContent(element.FindPropertyRelative("loader").objectReferenceValue.name))
        .AddAfterAddedCallback((element, index) => Target.AddChildAsset(element.FindPropertyRelative("loader")))
        .AddBeforeRemovedCallback((element, index) => Target.RemoveChildAsset(element.FindPropertyRelative("loader")))
        .Create(serializedObject, _localeLoaders);

      _localeSelectorsList = ReorderableDropdownListBuilder.CreateForScriptableObjectTypes<LocaleSelector>(TypeDisplayStringOptions.DontShowNamespace)
        .UsePropertyFieldForElementDrawerWithHeader((element, index) => new GUIContent(element.FindPropertyRelative("selector").objectReferenceValue.name))
        .AddAfterAddedCallback((element, index) => Target.AddChildAsset(element.FindPropertyRelative("selector")))
        .AddBeforeRemovedCallback((element, index) => Target.RemoveChildAsset(element.FindPropertyRelative("selector")))
        .Create(serializedObject, _localeSelectors);
    }


    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUILayout.LabelField("Settings For Loading Locales", EditorStyles.boldLabel);

      _localeLoadersList.DoLayoutList();

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Settings For Selecting The Startup Locale", EditorStyles.boldLabel);

      _localeSelectorsList.DoLayoutList();

      if (GUI.changed)
        serializedObject.ApplyModifiedProperties();
    }
  }
}