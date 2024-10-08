﻿using Audune.Utils.UnityEditor.Editor;
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

      var isLocalized = !string.IsNullOrEmpty(path.stringValue);

      var pathRect = rect.AlignTop(EditorGUI.GetPropertyHeight(path), EditorGUIUtility.standardVerticalSpacing, out rect);
      LocalizationEditorGUIExtensions.LocalizedStringSearchDropdown(pathRect, label, property);

      if (!isLocalized)
      {
        var valueRect = rect.AlignTop(EditorGUI.GetPropertyHeight(value), EditorGUIUtility.standardVerticalSpacing, out rect);
        if (label != GUIContent.none)
        {
          using (new EditorGUI.IndentLevelScope())
            LocalizationEditorGUIExtensions.LocalizedStringValueField(valueRect, new GUIContent($"Non-Localized Value", label.tooltip), property);
        }
        else
        {
          LocalizationEditorGUIExtensions.LocalizedStringValueField(valueRect, GUIContent.none, property);
        }
      }
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");

      var isLocalized = string.IsNullOrEmpty(path.stringValue);

      var height = EditorGUI.GetPropertyHeight(path);
      if (isLocalized)
        height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(value);
      return height;
    }
  }
}