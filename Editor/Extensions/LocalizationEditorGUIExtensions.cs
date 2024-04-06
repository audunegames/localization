using Audune.Utils.UnityEditor.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines extensions for the EditorGUI class
  public static class LocalizationEditorGUIExtensions
  {
    private static readonly IReadOnlyList<Locale> _locales = Locale.GetAllLocaleAssets().ToList();


    // Draw a search dropdown for the path of a localized string
    public static void LocalizedStringSearchDropdown(Rect rect, GUIContent label, SerializedProperty property)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var path = property.FindPropertyRelative("_path");
      var isLocalized = !string.IsNullOrEmpty(path.stringValue);
      var hasUndefinedValues = isLocalized && _locales.ContainsUndefinedString(path.stringValue);
      var hasMissingValues = isLocalized && _locales.ContainsMissingString(path.stringValue);

      var color = hasUndefinedValues ? EditorIcons.errorColor : hasMissingValues ? EditorIcons.warningColor : Color.white;
      var dropdownLabel = new GUIContent(isLocalized ? path.stringValue : "<Non-Localized Value>");

      using (new EditorGUIUtilityExtensions.ColorScope(color))
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