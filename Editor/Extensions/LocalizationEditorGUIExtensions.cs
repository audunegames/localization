using Audune.Utils.UnityEditor.Editor;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines extensions for the EditorGUI class
  public static class LocalizationEditorGUIExtensions
  {
    // Draw a search dropdown for the path of a localized string
    public static void LocalizedStringSearchDropdown(Rect rect, GUIContent label, SerializedProperty property)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var path = property.FindPropertyRelative("_path");
      var isLocalized = !string.IsNullOrEmpty(path.stringValue);
      var dropdownLabel = new GUIContent(isLocalized ? path.stringValue : "<Non-Localized Value>");

      var propertyColor = Color.white;
      if (isLocalized)
      {
        var localizationSystem = Object.FindObjectOfType<LocalizationSystem>();
        if (localizationSystem != null)
        {
          localizationSystem.InitializeIfNoLocaleSelected();

          if (localizationSystem.loadedLocales.ContainsUndefinedString(path.stringValue))
            propertyColor = Color.Lerp(Color.red, Color.white, 0.75f);
          else if (localizationSystem.loadedLocales.ContainsMissingString(path.stringValue))
            propertyColor = Color.Lerp(Color.yellow, Color.white, 0.75f);
        }
      }

      using (new EditorGUIUtilityExtensions.ColorScope(propertyColor))
        EditorGUIExtensions.SearchDropdown<string, LocalizedStringSearchWindow>(rect, label, dropdownLabel, path);
    }

    // Draw a text field for the value of a localized string
    public static void LocalizedStringValueField(Rect rect, GUIContent label, SerializedProperty property)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var value = property.FindPropertyRelative("_value");

      EditorGUI.PropertyField(rect, value, label);
    }
  }
}