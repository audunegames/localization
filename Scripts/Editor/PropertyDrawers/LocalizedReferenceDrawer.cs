using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a drawer for localized references
  [CustomPropertyDrawer(typeof(LocalizedReference<>))]
  public class LocalizedReferenceDrawer : PropertyDrawer
  {
    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      var path = property.FindPropertyRelative("_path");
      var value = property.FindPropertyRelative("_value");

      // Draw the label
      if (label != null)
        rect = EditorGUI.PrefixLabel(rect, label);

      // Draw the path
      if (EditorGUI.DropdownButton(rect, new GUIContent(path.stringValue), FocusType.Keyboard))
        LocalizedReferenceSearchWindow.Open(value, new GUIContent("Select localized key"));

      /*_path.ValueEntry.WeakSmartValue = new ValueSelectorFactory<string>()
        .WithItems(_textSystem != null && _textSystem.CurrentLocale != null ? _textSystem.CurrentLocale.strings.RecursiveEntries.Keys : Enumerable.Empty<string>())
        .WithSearchToolbarDrawn()
        .WithSearchInKeys(path => path)
        .WithMenuStyle(ValueSelector<string>.CompactMenuStyle)
        .WithSelection(!string.IsNullOrEmpty((string)_path.ValueEntry.WeakSmartValue) ? (string)_path.ValueEntry.WeakSmartValue : null)
        .WithPathMapper(path => path?.Replace(".", "/") ?? "")
        .WithLabelMapper(path => path)
        .WithNoneValueAdded("<Non-Localized Value>", "")
        .DrawSingleSelectDropdown(rect);*/

      // Draw the value
      if (path.stringValue == null)
      {
        EditorGUI.indentLevel += label != null ? 1 : 0;
        EditorGUI.PropertyField(rect, value, new GUIContent(label != null ? new GUIContent(value.displayName, value.tooltip) : GUIContent.none));
        EditorGUI.indentLevel -= label != null ? 1 : 0;
      }
    }
  }
}