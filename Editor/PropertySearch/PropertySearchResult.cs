using Audune.Localization.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a result in a property search
  public sealed class PropertySearchResult
  {
    // Asset of the result
    public EditorAsset asset;

    // Component of the result
    public EditorComponent component;

    // Property name of the result
    public readonly string propertyName;

    // Property display name of the result
    public readonly string propertyDisplayName;

    // Key of the property when the result is a dictionary
    public readonly int propertyDictionaryKeyIndex;


    // Return the target object of the component
    public Object targetObject => component.targetObject;

    // Return the target serialized object of the component
    public SerializedObject targetSerializedObject => component.targetSerializedObject;

    // Return the target serialized property of the result
    public SerializedProperty targetSerializedProperty {
      get {
        var targetSerializedObject = this.targetSerializedObject;
        var serializedProperty = targetSerializedObject != null ? targetSerializedObject.FindProperty(propertyName) : null;
        if (serializedProperty != null && propertyDictionaryKeyIndex >= 0)
          return serializedProperty.FindPropertyRelative("_entries._entries").GetArrayElementAtIndex(propertyDictionaryKeyIndex)?.FindPropertyRelative("value");
        else
          return serializedProperty;
      }
    }

    // Return the property value of the result
    public object propertyValue {
      get {
        var targetSerializedProperty = this.targetSerializedProperty;
        if (targetSerializedProperty != null)
        {
          return targetSerializedProperty.boxedValue;
        }
        else
        {
          var propertyField = component.componentType.GetField(propertyName);
          var targetObject = this.targetObject;
          dynamic value = targetObject != null && propertyField != null ? propertyField.GetValue(targetObject) : null;
          if (value != null && propertyDictionaryKeyIndex >= 0 && propertyDictionaryKeyIndex < value.keys.Count)
            return value.Get(value.keys[propertyDictionaryKeyIndex]);
          else
            return value;
        }
      }
    }


    // Constructor
    public PropertySearchResult(EditorAsset asset, EditorComponent component, string propertyName, string propertyDisplayName = null, int propertyDictionaryKeyIndex = -1)
    {
      this.asset = asset;
      this.component = component;
      this.propertyName = propertyName;
      this.propertyDisplayName = propertyDisplayName ?? ObjectNames.NicifyVariableName(propertyName);
      this.propertyDictionaryKeyIndex = propertyDictionaryKeyIndex;
    }
  }
}