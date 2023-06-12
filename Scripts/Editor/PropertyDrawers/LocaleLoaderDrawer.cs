using Audune.Localization.Loaders;
using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a property drawer for locale loaders
  [CustomPropertyDrawer(typeof(LocaleLoader), true)]
  public sealed class LocaleLoaderDrawer : PropertyDrawer
  {
    // Reorderable list for the locales of a persistent locale loader
    private ReorderableList _localesList;


    // Initialize the reorderable list for the locales of a persistent locale loader 
    public void InitializeLocalesList(SerializedProperty property)
    {
      if (_localesList == null)
      {
        _localesList = new ReorderableList(property.serializedObject, property);
        _localesList.headerHeight = 0;
        _localesList.drawElementCallback = ReorderableListUtils.Element(_localesList, (element, index) => GUIContent.none);
        _localesList.elementHeightCallback = ReorderableListUtils.ElementHeight(_localesList);
      }
      else
      {
        _localesList.serializedProperty = property;
      }
    }


    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      EditorGUIExtensions.InlineObject(rect, property, (rect, serializedObject) => {
        if (serializedObject.targetObject is PersistentLocaleLoader)
        {
          var locales = serializedObject.FindProperty("_locales");

          if (_localesList == null || _localesList.serializedProperty != locales)
            InitializeLocalesList(locales);

          rect = EditorGUI.IndentedRect(rect);
          _localesList.DoList(rect);
        }
        else
        {
          EditorGUIExtensions.ChildPropertyFields(rect, serializedObject, p => p.name != "m_Script");
        }
      });
    }

    // Return the property height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIExtensions.GetInlineObjectHeight(property, (serializedObject) => {
        if (serializedObject.targetObject is PersistentLocaleLoader)
        {
          var locales = serializedObject.FindProperty("_locales");

          if (_localesList == null || _localesList.serializedProperty != locales)
            InitializeLocalesList(locales);

          return _localesList.GetHeight();
        }
        else
        {
          return EditorGUIExtensions.GetChildPropertyFieldsHeight(serializedObject, p => p.name != "m_Script");
        }
      });
    }
  }
}
