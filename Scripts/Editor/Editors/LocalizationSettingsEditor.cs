using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Property drawer for localization settings assets
  [CustomEditor(typeof(LocalizationSettings))]
  public sealed class LocalizationSettingsEditor : UnityEditor.Editor
  {
    // Properties of the object
    private SerializedProperty _localeLoaders;
    private SerializedProperty _localeSelectors;

    // Reorderable lists of the object
    private ReorderableList _localeLoadersList;
    private ReorderableList _localeSelectorsList;


    // Return the target object of the editor
    public LocalizationSettings Target => serializedObject.targetObject as LocalizationSettings;


    // Constructor
    public void OnEnable()
    {
      _localeLoaders = serializedObject.FindProperty("_localeLoaders");
      _localeSelectors = serializedObject.FindProperty("_localeSelectors");

      _localeLoadersList = new ReorderableList(serializedObject, _localeLoaders);
      _localeLoadersList.drawHeaderCallback = ReorderableListUtils.HeaderWithDisplayName(_localeLoadersList);
      _localeLoadersList.drawElementCallback = ReorderableListUtils.ElementWithHeaderLabel(_localeLoadersList, (element, index) => new GUIContent(element.FindPropertyRelative("loader").objectReferenceValue.name));
      _localeLoadersList.elementHeightCallback = ReorderableListUtils.ElementHeightWithLabel(_localeLoadersList);
      _localeLoadersList.onAddDropdownCallback = ReorderableListUtils.AddScriptableObjectDropdown(typeof(LocaleLoader), loader => new LocalizationSettings.LocaleLoaderEntry(loader as LocaleLoader, ExecutionMode.Always), element => Target.AddChildAsset(element.FindPropertyRelative("loader")));
      _localeLoadersList.onRemoveCallback = ReorderableListUtils.Remove(element => Target.RemoveChildAsset(element.FindPropertyRelative("loader")));

      _localeSelectorsList = new ReorderableList(serializedObject, _localeSelectors);
      _localeSelectorsList.drawHeaderCallback = ReorderableListUtils.HeaderWithDisplayName(_localeSelectorsList);
      _localeSelectorsList.drawElementCallback = ReorderableListUtils.ElementWithHeaderLabel(_localeSelectorsList, (element, index) => new GUIContent(element.FindPropertyRelative("selector").objectReferenceValue.name));
      _localeSelectorsList.elementHeightCallback = ReorderableListUtils.ElementHeightWithLabel(_localeSelectorsList);
      _localeSelectorsList.onAddDropdownCallback = ReorderableListUtils.AddScriptableObjectDropdown(typeof(LocaleSelector), selector => new LocalizationSettings.LocaleSelectorEntry(selector as LocaleSelector, ExecutionMode.Always), element => Target.AddChildAsset(element.FindPropertyRelative("selector")));
      _localeSelectorsList.onRemoveCallback = ReorderableListUtils.Remove(element => Target.RemoveChildAsset(element.FindPropertyRelative("selector")));
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