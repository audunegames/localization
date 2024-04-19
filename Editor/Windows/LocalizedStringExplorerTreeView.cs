using Audune.Utils.UnityEditor.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for a localized string explorer editor window
  public sealed class LocalizedStringExplorerTreeView : SerializedPropertySearchTreeView
  {    
    // Locales of the tree view
    private IReadOnlyList<Locale> _locales = Locale.GetAllLocaleAssets().ToList();


    // Return the item matcher for the property value
    protected override ItemMatcher<SerializedPropertySearchResult> _propertyValueMatcher => ItemMatcher.String<SerializedPropertySearchResult>(data => data.propertyValue is LocalizedString localizedString && localizedString.isLocalized ? localizedString.path : null);


    // Constructor
    public LocalizedStringExplorerTreeView(TreeViewState treeViewState) : base(treeViewState, p => p.type == typeof(LocalizedString).Name, "Assets")
    {
      // Set the tree view columns
      columns = new[] {
        new Column(new GUIContent("Object"), OnObjectColumnGUI, width: 250),
        new Column(new GUIContent("Component"), OnComponentColumnGUI, width: 150, isHideable: true),
        new Column(new GUIContent("Property Name"), OnPropertyNameColumnGUI, width: 150, isHideable: true),
        new Column(new GUIContent("Property Value"), OnPropertyValueColumnGUI, width: 200),
        new Column(new GUIContent("Non-Localized Value"), OnNonLocalizedValueColumnGUI, width: 200),
      };
    }

    // Return the icon for a data item
    protected override Texture2D SelectDataIcon(SerializedPropertySearchResult data)
    {
      // Check if the data has missing values
      var hasMissingValues = data.propertyValue is not  LocalizedString localizedString || (localizedString.isLocalized && _locales.ContainsMissingString(localizedString.path));
      return hasMissingValues ? EditorIcons.errorMark : base.SelectDataIcon(data);
    }


    // Draw the property value field GUI
    protected override void OnPropertyValueFieldGUI(Rect rect, SerializedProperty serializedProperty)
    {
      // Draw the localized string search dropdown
      LocalizationEditorGUIExtensions.LocalizedStringSearchDropdown(rect, GUIContent.none, serializedProperty, path => HighlightSearchString(path, searchString, _keys, _valueKey));
    }

    // Draw the non-localized value column GUI
    private void OnNonLocalizedValueColumnGUI(Rect rect, DataItem item)
    {
      // Get the serialized property
      var serializedProperty = item.data.targetSerializedProperty;
      if (serializedProperty != null && string.IsNullOrEmpty(serializedProperty.FindPropertyRelative("_path").stringValue))
      {
        // Get the rect for drawing the property field
        var propertyRect = rect.ContractTop(EditorGUIUtility.standardVerticalSpacing * 0.5f).ContractBottom(EditorGUIUtility.standardVerticalSpacing * 0.5f);

        // Draw the localized string value field
        serializedProperty.serializedObject.Update();
        LocalizationEditorGUIExtensions.LocalizedStringValueField(propertyRect, GUIContent.none, serializedProperty);
        serializedProperty.serializedObject.ApplyModifiedProperties();
      }
    }

    // Fill a context menu for the specified data item
    protected override void FillDataItemContextMenu(DataItem item, GenericMenu menu)
    {
      // Items to find the navigate to the definition of the item
      if (item.data.propertyValue is LocalizedString localizedString && localizedString.isLocalized)
        menu.AddItem(new GUIContent("Find Definition"), false, () => NavigateToDefinition(localizedString.path));
      else
        menu.AddDisabledItem(new GUIContent("Find Definition"), false);

      // Fill the base menu items
      menu.AddSeparator("");
      base.FillDataItemContextMenu(item, menu);
    }


    // Navigate to the definition of the specified localized string
    private void NavigateToDefinition(string path)
    {
      // Open the locale explorer window if the localized string is set
      LocaleExplorerWindow.Open<LocaleExplorerWindow>(selected: path);
    }
  }
}