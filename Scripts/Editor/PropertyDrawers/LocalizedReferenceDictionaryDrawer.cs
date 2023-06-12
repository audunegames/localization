using Audune.Utils.Unity;
using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a property drawer for localized string reference diictionaries
  [CustomPropertyDrawer(typeof(LocalizedReferenceDictionary<,>))]
  public class LocalizedReferenceDictionaryDrawer : PropertyDrawer
  {
    // Reorderable list for the entries
    private ReorderableList _entriesList;


    // Initialize the reorderable list for the entries
    public void InitializeEntriesList(SerializedProperty property, SerializedProperty elements)
    {
      if (_entriesList == null)
      {
        _entriesList = new ReorderableList(elements.serializedObject, elements);
        _entriesList.drawHeaderCallback = ReorderableListUtils.HeaderWithDisplayName(property);
        _entriesList.drawElementCallback = ReorderableListUtils.CustomElement(_entriesList, ((rect, element, index) => {
          EditorGUI.PropertyField(rect.AlignLeft(EditorGUIUtility.labelWidth, 0.0f, out rect), element.FindPropertyRelative("key"), GUIContent.none);
          EditorGUI.PropertyField(rect, element.FindPropertyRelative("reference"), GUIContent.none);
        }));
        _entriesList.elementHeightCallback = ReorderableListUtils.CustomElementHeight(_entriesList, (element, Index) => EditorGUI.GetPropertyHeight(element.FindPropertyRelative("reference")));
      }
      else
      {
        _entriesList.serializedProperty = elements;
      }
    }


    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      var entries = property.FindPropertyRelative("_entries");

      if (_entriesList == null || _entriesList.serializedProperty != entries)
        InitializeEntriesList(property, entries);

      rect = EditorGUI.IndentedRect(rect);
      _entriesList.DoList(rect);
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var entries = property.FindPropertyRelative("_entries");

      if (_entriesList == null || _entriesList.serializedProperty != entries)
        InitializeEntriesList(property, entries);

      return _entriesList.GetHeight();
    }
  }
}