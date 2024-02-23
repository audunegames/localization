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
      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");
      var arguments = property.FindPropertyRelative("_arguments");

      var pathRect = rect.AlignTop(EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing, out rect);
      property.isExpanded = EditorGUI.Foldout(pathRect.AlignLeft(EditorGUIUtility.labelWidth, EditorGUIUtility.standardVerticalSpacing, out pathRect), property.isExpanded, label, true);

      var pathDropdownLabel = new GUIContent(!string.IsNullOrEmpty(path.stringValue) ? path.stringValue : "<Non-Localized Value>");
      EditorGUIExtensions.SearchDropdown<string, LocalizedStringWindow>(pathRect, pathDropdownLabel, path);

      if (property.isExpanded)
      {
        EditorGUI.indentLevel++;

        if (string.IsNullOrEmpty(path.stringValue))
          EditorGUI.PropertyField(rect.AlignTop(EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing, out rect), value);

        EditorGUI.PropertyField(rect, arguments);

        EditorGUI.indentLevel--;
      }
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");
      var arguments = property.FindPropertyRelative("_arguments");

      var height = EditorGUI.GetPropertyHeight(path);
      if (property.isExpanded)
      {
        if (string.IsNullOrEmpty(path.stringValue))
          height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(value);
        height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(arguments);
      }

      return height;
    }
  }
}