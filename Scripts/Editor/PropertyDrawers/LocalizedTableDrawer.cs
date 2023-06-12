using Audune.Utils.Unity.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a property drawer for localized tables
  [CustomPropertyDrawer(typeof(LocalizedTable<>), true)]
  public sealed class LocalizedTableDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      var keys = property.FindPropertyRelative("_keys");
      var values = property.FindPropertyRelative("_values");

      property.isExpanded = EditorGUI.Foldout(rect.AlignTop(EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing, out rect), property.isExpanded, new GUIContent($"{label.text} ({keys.arraySize} entries)", label.image, label.tooltip), true);

      if (property.isExpanded)
      {
        EditorGUI.indentLevel++;

        var keysProperties = keys.GetArrayElements().ToList();
        var valuesProperties = values.GetArrayElements().ToList();

        EditorGUIExtensions.ArrayElementPropertyFields(rect, values, (value, index) => new GUIContent(keys.GetArrayElementAtIndex(index).stringValue));

        EditorGUI.indentLevel--;
      }
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var keys = property.FindPropertyRelative("_keys");
      var values = property.FindPropertyRelative("_values");

      var height = EditorGUIUtility.singleLineHeight;

      if (property.isExpanded)
        height += EditorGUIUtility.standardVerticalSpacing + EditorGUIExtensions.GetArrayElementPropertyFieldsHeight(values, (value, index) => new GUIContent(keys.GetArrayElementAtIndex(index).stringValue));

      return height;
    }
  }
}
