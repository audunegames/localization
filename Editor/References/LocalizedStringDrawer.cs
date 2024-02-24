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
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");

      if (!property.isExpanded)
      {
        var fieldRect = string.IsNullOrEmpty(path.stringValue) ? rect.AlignLeft(rect.width - 24, EditorGUIUtility.standardVerticalSpacing, out rect) : rect;
        var pathDropdownLabel = new GUIContent(!string.IsNullOrEmpty(path.stringValue) ? path.stringValue : "<Non-Localized Value>");
        EditorGUIExtensions.SearchDropdown<string, LocalizedStringSearchWindow>(fieldRect, label, pathDropdownLabel, path);

        if (string.IsNullOrEmpty(path.stringValue))
        {
          var valueButtonIcon = Resources.Load<Texture>("Icons/Buttons/ValueButton");
          if (GUI.Button(rect, new GUIContent(valueButtonIcon, "Enter the non-localized value for the localized string")))
            property.isExpanded = true;
        }
      }
      else
      {
        var fieldRect = rect.AlignLeft(rect.width - 24, EditorGUIUtility.standardVerticalSpacing, out rect);
        EditorGUI.PropertyField(fieldRect, value, new GUIContent($"{label.text} (Value)", label.tooltip));

        var pathButtonIcon = Resources.Load<Texture>("Icons/Buttons/PathButton");
        if (GUI.Button(rect, new GUIContent(pathButtonIcon, "Select the path for the localized string")))
          property.isExpanded = false;
      }
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");

      if (!property.isExpanded)
        return EditorGUI.GetPropertyHeight(path);
      else
        return EditorGUI.GetPropertyHeight(value);
    }
  }
}