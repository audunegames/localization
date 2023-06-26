using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Settings.Editor
{
  // Class that defines a property drawer for executionlocale selector entries
  [CustomPropertyDrawer(typeof(LocaleSelectorEntry))]
  public sealed class LocaleSelectorEntryDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var executionMode = property.FindPropertyRelative("executionMode");
      var selector = property.FindPropertyRelative("selector");

      EditorGUIExtensions.MultiplePropertyFields(rect, new[] { executionMode, selector });
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var executionMode = property.FindPropertyRelative("executionMode");
      var selector = property.FindPropertyRelative("selector");

      return EditorGUIExtensions.GetMultiplePropertyFieldsHeight(new[] { executionMode, selector });
    }
  }
}
