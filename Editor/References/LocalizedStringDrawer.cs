using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a drawer for a localized string reference
  [CustomPropertyDrawer(typeof(LocalizedString))]
  public class LocalizedStringDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      var localizationSystem = Object.FindObjectOfType<LocalizationSystem>();
      if (localizationSystem != null)
        localizationSystem.InitializeIfNoLocaleSelected();

      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");

      var isPathNull = string.IsNullOrEmpty(path.stringValue);

      var pathRect = rect.AlignTop(EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing, out rect);
      var pathDropdownLabel = new GUIContent(!string.IsNullOrEmpty(path.stringValue) ? path.stringValue : "<Non-Localized Value>");

      var propertyColor = Color.white;
      if (!isPathNull && localizationSystem.loadedLocales.ContainsUndefinedString(path.stringValue))
        propertyColor = Color.Lerp(Color.red, Color.white, 0.75f);
      else if (!isPathNull && localizationSystem.loadedLocales.ContainsMissingString(path.stringValue))
        propertyColor = Color.Lerp(Color.yellow, Color.white, 0.75f);

      using (new EditorGUIUtilityExtensions.ColorScope(propertyColor))
        EditorGUIExtensions.SearchDropdown<string, LocalizedStringSearchWindow>(pathRect, label, pathDropdownLabel, path);

      if (isPathNull)
      {
        var valueRect = rect.AlignTop(EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing, out rect);
        if (label != GUIContent.none)
        {
          EditorGUI.indentLevel++;
          EditorGUI.PropertyField(valueRect, value, new GUIContent($"Non-Localized Value", label.tooltip));
          EditorGUI.indentLevel--;
        }
        else
        {
          EditorGUI.PropertyField(valueRect, value, GUIContent.none);
        }
      }
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");

      var isPathNull = string.IsNullOrEmpty(path.stringValue);

      var height = EditorGUI.GetPropertyHeight(path);
      if (isPathNull)
        height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(value);
      return height;
    }
  }
}