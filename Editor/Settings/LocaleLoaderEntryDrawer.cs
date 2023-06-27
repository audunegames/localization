using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Settings.Editor
{
  // Class that defines a property drawer for locale loader entries
  [CustomPropertyDrawer(typeof(LocaleLoaderEntry))]
  public sealed class LocaleLoaderEntryDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      var executionMode = property.FindPropertyRelative("executionMode");
      var loader = property.FindPropertyRelative("loader");

      EditorGUIExtensions.MultiplePropertyFields(rect, new[] { executionMode, loader });
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var executionMode = property.FindPropertyRelative("executionMode");
      var loader = property.FindPropertyRelative("loader");

      return EditorGUIExtensions.GetMultiplePropertyFieldsHeight(new[] { executionMode, loader });
    }
  }
}
