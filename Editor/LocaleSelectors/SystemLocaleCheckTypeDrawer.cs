using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a property drawer for a system locale check type
  [CustomPropertyDrawer(typeof(SystemLocaleCheckType))]
  public sealed class SystemLocaleCheckTypeDrawer : PropertyDrawer
  {
    // Draw the property
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      EditorGUIExtensions.EnumFlagsDropdown<SystemLocaleCheckType>(rect, label, property);
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIExtensions.GetEnumFlagsDropdownHeight<SystemLocaleCheckType>(property);
    }
  }
}