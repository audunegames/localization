using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a property drawer for locale selectors
  [CustomPropertyDrawer(typeof(LocaleSelector), true)]
  public sealed class LocaleSelectorDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      EditorGUIExtensions.InlineObject(rect, property, (rect, serializedObject) => {
        EditorGUIExtensions.ChildPropertyFields(rect, serializedObject, p => p.name != "m_Script");
      });
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIExtensions.GetInlineObjectHeight(property, (serializedObject) => {
        return EditorGUIExtensions.GetChildPropertyFieldsHeight(serializedObject, p => p.name != "m_Script");
      });
    }
  }
}
