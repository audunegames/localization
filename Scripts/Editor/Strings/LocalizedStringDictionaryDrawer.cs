using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Strings.Editor
{
  // Class that defines a property drawer for a localized string reference diictionary
  [CustomPropertyDrawer(typeof(LocalizedStringDictionary<>))]
  public class LocalizedStringDictionaryDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      EditorGUI.PropertyField(rect, property.FindPropertyRelative("_entries"), scope.content);
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_entries"), label);
    }
  }
}