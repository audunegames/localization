using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a property drawer for an execution mode
  [CustomPropertyDrawer(typeof(ExecutionMode))]
  public sealed class ExecutionModeDrawer : PropertyDrawer
  {
    // Draw the property
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      EditorGUIExtensions.EnumFlagsDropdown<ExecutionMode>(rect, label, property);
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIExtensions.GetEnumFlagsDropdownHeight<ExecutionMode>(property);
    }
  }
}