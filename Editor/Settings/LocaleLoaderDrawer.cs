using Audune.Localization.Settings.Loaders;
using Audune.Utils.Collections;
using Audune.Utils.Collections.Editor;
using Audune.Utils.Unity.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Audune.Localization.Settings.Editor
{
  // Class that defines a property drawer for locale loaders
  [CustomPropertyDrawer(typeof(LocaleLoader), true)]
  public sealed class LocaleLoaderDrawer : PropertyDrawer
  {
    // Reorderable list for the locales of a persistent locale loader
    private ReorderableList _localesList;


    // Draw the property GUI
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
      using var scope = new EditorGUI.PropertyScope(rect, label, property);

      EditorGUIExtensions.InlineObject(rect, property, (rect, serializedObject) => {
        if (serializedObject.targetObject is PersistentLocaleLoader)
        {
          var locales = serializedObject.FindProperty("_locales");

          if (_localesList == null || _localesList.serializedProperty != locales)
            InitializeLocalesList(locales);

          rect = EditorGUI.IndentedRect(rect);
          _localesList.DoList(rect, ReorderableListDrawOptions.DrawFoldout);
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

          return _localesList.GetHeight(ReorderableListDrawOptions.DrawFoldout);
        }
        else
        {
          return EditorGUIExtensions.GetChildPropertyFieldsHeight(serializedObject, p => p.name != "m_Script");
        }
      });
    }


    // Initialize the reorderable list for the locales of a persistent locale loader 
    private void InitializeLocalesList(SerializedProperty property)
    {
      if (_localesList == null)
      {
        _localesList = new ReorderableListBuilder()
          .UsePropertyFieldForElementDrawer(GUIContent.none)
          .Create(property.serializedObject, property);
      }
      else
      {
        _localesList.serializedProperty = property;
      }
    }
  }
}
