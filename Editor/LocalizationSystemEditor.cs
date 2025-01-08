using Audune.Utils.Types;
using Audune.Utils.Types.Editor;
using Audune.Utils.UnityEditor.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines an editor for the persistence system
  [CustomEditor(typeof(LocalizationSystem))]
  public class LocalizationSystemEditor : UnityEditor.Editor
  {
    // Flodout state of the editor
    private bool _localeLoadersFoldout = true;
    private bool _localeSelectorsFoldout = true;
    private bool _componentsFoldout = true;

    // Generic menus for types
    private GenericMenu _localeLoadersTypesMenu;
    private GenericMenu _localeSelectorsTypesMenu;


    // Return the target object of the editor
    public new LocalizationSystem target => serializedObject.targetObject as LocalizationSystem;


    // OnEnable is called when the component becomes enabled
    protected void OnEnable()
    {
      // Initialize the generic menus for types
      _localeLoadersTypesMenu = typeof(LocaleLoader).CreateGenericMenuForChildTypes(TypeDisplayOptions.DontShowNamespace, null, type => target.gameObject.AddComponent(type));
      _localeSelectorsTypesMenu = typeof(LocaleSelector).CreateGenericMenuForChildTypes(TypeDisplayOptions.DontShowNamespace, null, type => target.gameObject.AddComponent(type));
    }

    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUI.BeginChangeCheck();

      _localeLoadersFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_localeLoadersFoldout, "Registered Locale Loaders");
      if (_localeLoadersFoldout)
      {
        var localeLoaders = target.loaders.ToList();
        if (localeLoaders.Count > 0)
          EditorGUILayout.HelpBox(string.Join("\n", localeLoaders.Select(l => $"• {l.GetType().ToDisplayString(TypeDisplayOptions.DontShowNamespace)} [Priority {l.priority}, {ObjectNames.NicifyVariableName(l.executionMode.ToString())}]")), MessageType.None);
        else
          EditorGUILayout.HelpBox("None", MessageType.None);

        EditorGUILayout.Space();
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      _localeSelectorsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_localeSelectorsFoldout, "Registered Locale Selectors");
      if (_localeSelectorsFoldout)
      {
        var localeSelectors = target.selectors.ToList();
        if (localeSelectors.Count > 0)
          EditorGUILayout.HelpBox(string.Join("\n", localeSelectors.Select(s => $"• {s.GetType().ToDisplayString(TypeDisplayOptions.DontShowNamespace)} [Priority {s.priority}, {ObjectNames.NicifyVariableName(s.executionMode.ToString())}]")), MessageType.None);
        else
          EditorGUILayout.HelpBox("None", MessageType.None);

        EditorGUILayout.Space();
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      _componentsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_componentsFoldout, "Components");
      if (_componentsFoldout)
      {
        var addLoaderPosition = EditorGUILayout.GetControlRect(true);
        addLoaderPosition = EditorGUI.PrefixLabel(addLoaderPosition, new GUIContent("Add Locale Loader"));
        EditorGUIExtensions.GenericMenuDropdown(addLoaderPosition, new GUIContent("(select)"), _localeLoadersTypesMenu);

        var addSelectorPosition = EditorGUILayout.GetControlRect(true);
        addSelectorPosition = EditorGUI.PrefixLabel(addSelectorPosition, new GUIContent("Add Locale Selector"));
        EditorGUIExtensions.GenericMenuDropdown(addSelectorPosition, new GUIContent("(select)"), _localeSelectorsTypesMenu);
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      if (EditorGUI.EndChangeCheck())
        serializedObject.ApplyModifiedProperties();
    }
  }
}