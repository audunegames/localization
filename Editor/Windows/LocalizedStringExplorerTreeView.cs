using Audune.Utils.UnityEditor.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for a localized string explorer editor window
  public class LocalizedStringExplorerTreeView : ItemsTreeView<PropertySearchResult>
  {
    // Default strings
    private const string _missingDisplayName = "Strings with missing values";
    private const string _prefabsDisplayName = "Prefabs";
    private const string _scriptableObjectsDisplayName = "Scriptable objects";
    private const string _scenesDisplayName = "Scene objects";


    // Default options for the tree view
    private static readonly Options _options = new Options {
      displayNameSelector = data => data.component.ToString(),
      iconSelector = data => data.asset.type.IsScene() ? EditorIcons.gameObject : data.asset.assetIcon,
      groupIconSelector = (path, expanded) => {
        if (path.Length > 1)
          return expanded ? EditorIcons.folderOpened : EditorIcons.folder;
        else if (path[0] == _missingDisplayName)
          return EditorIcons.errorMark;
        else if (path[0] == _prefabsDisplayName)
          return EditorIcons.prefab;
        else if (path[0] == _scriptableObjectsDisplayName)
          return EditorIcons.scriptableObject;
        else if (path[0] == _scenesDisplayName)
          return EditorIcons.gameObject;
        else
          return null;
      },
    };


    // The locales used in the tree view
    private List<Locale> _locales;


    // Constructor
    public LocalizedStringExplorerTreeView(IEnumerable<PropertySearchResult> items, IEnumerable<Locale> locales) : base(items, _options)
    {
      _locales = new List<Locale>(locales ?? Enumerable.Empty<Locale>());

      multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(new[] {
        new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Object"), width = 250, canSort = false, allowToggleVisibility = false },
        new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Component"), width = 150, canSort = false, allowToggleVisibility = true },
        new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Property"), width = 150, canSort = false, allowToggleVisibility = true },
        new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Localized String"), width = 200, canSort = false, allowToggleVisibility = false },
        new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Non-Localized Value"), width = 200, canSort = false, allowToggleVisibility = true },
      }));
    }

    // Build the items of the tree view
    protected override void Build(ref TreeViewItem rootItem, ref int id)
    {
      // Create root items for different types of items
      var missingRootItem = CreateGroupItem(id++, new[] { _missingDisplayName });
      var prefabsRootItem = CreateGroupItem(id++, new[] { _prefabsDisplayName });
      var scriptableObjectsRootItem = CreateGroupItem(id++, new[] { _scriptableObjectsDisplayName });
      var scenesRootItem = CreateGroupItem(id++, new[] { _scenesDisplayName });

      // Iterate over the items and create data items for them
      var prefabsPathItems = new Dictionary<string, GroupItem>();
      var scriptableObjectsPathItems = new Dictionary<string, GroupItem>();
      foreach (var item in items)
      {
        // Create the data item
        var dataItem = CreateDataItem(id++, item);
        var localizedString = item.propertyValue as LocalizedString;
        var hasMissingValues = localizedString != null && localizedString.isLocalized && _locales.ContainsMissingString(localizedString.path);
        if (hasMissingValues)
          dataItem.icon = EditorIcons.errorMark;

        // Create a separate data item if the localized string of the item contains a missing value
        if (hasMissingValues)
          missingRootItem.AddChild(CreateDataItem(id++, item, icon: EditorIcons.errorMark));

        // Check the asset type of the item
        if (item.asset.type.IsPrefab())
        {
          // Create the path items for the prefab item
          var path = item.asset.assetDirectoryName.Split('/', 2)[1];
          if (!prefabsPathItems.TryGetValue(path, out var pathItem))
          {
            pathItem = CreateGroupItem(id++, new[] { _prefabsDisplayName, path });
            prefabsRootItem.AddChild(pathItem);
            prefabsPathItems.Add(path, pathItem);
          }

          // Add the data item to the correct path item
          pathItem.AddChild(dataItem);
        }
        else if (item.asset.type.IsScriptableObject())
        {
          // Create the path items for the scriptable object item
          var path = item.asset.assetDirectoryName.Split('/', 2)[1];
          if (!scriptableObjectsPathItems.TryGetValue(path, out var pathItem))
          {
            pathItem = CreateGroupItem(id++, new[] { _scriptableObjectsDisplayName, path });
            scriptableObjectsRootItem.AddChild(pathItem);
            scriptableObjectsPathItems.Add(path, pathItem);
          }

          // Add the data item to the correct path item
          pathItem.AddChild(dataItem);
        }
        else if (item.asset.type.IsScene())
          scenesRootItem.AddChild(dataItem);
      }

      // Add root items
      if (missingRootItem.hasChildren)
        rootItem.AddChild(missingRootItem);
      if (prefabsRootItem.hasChildren)
        rootItem.AddChild(prefabsRootItem);
      if (scriptableObjectsRootItem.hasChildren)
        rootItem.AddChild(scriptableObjectsRootItem);
      if (scenesRootItem.hasChildren)
        rootItem.AddChild(scenesRootItem);
    }

    // Draw a cell that represents a data item in the tree view
    protected override void OnDataCellGUI(DataItem item, int columnIndex, Rect columnRect)
    {
      if (columnIndex == 0)
      {
        // Object column
        if (!isSearching)
          columnRect = columnRect.ContractLeft(GetContentIndent(item));

        EditorGUI.LabelField(columnRect, HighlightSearchString(item), label);
      }
      else if (columnIndex == 1)
      {
        // Component column
        EditorGUI.LabelField(columnRect, HighlightSearchString(new GUIContent(ObjectNames.NicifyVariableName(item.data.component.componentScript.name), item.data.component.componentIcon)), label);
      }
      else if (columnIndex == 2)
      {
        // Property column
        var serializedProperty = item.data.targetSerializedProperty;
        EditorGUI.LabelField(columnRect, HighlightSearchString(item.data.propertyDisplayName), serializedProperty != null && serializedProperty.prefabOverride ? boldLabel : label);
      }
      else if (columnIndex == 3)
      {
        // Localized string column
        var serializedProperty = item.data.targetSerializedProperty;
        if (serializedProperty != null)
        {
          var propertyRect = columnRect.ContractTop(EditorGUIUtility.standardVerticalSpacing * 0.5f).ContractBottom(EditorGUIUtility.standardVerticalSpacing * 0.5f);

          serializedProperty.serializedObject.Update();
          LocalizationEditorGUIExtensions.LocalizedStringSearchDropdown(propertyRect, GUIContent.none, serializedProperty);
          serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        else
        {
          EditorGUI.LabelField(columnRect, new GUIContent("Could not find target object", EditorIcons.errorMark), label);
        }
      }
      else if (columnIndex == 4)
      {
        // Non-Localized Value column
        var serializedProperty = item.data.targetSerializedProperty;
        if (serializedProperty != null && string.IsNullOrEmpty(serializedProperty.FindPropertyRelative("_path").stringValue))
        {
          var propertyRect = columnRect.ContractTop(EditorGUIUtility.standardVerticalSpacing * 0.5f).ContractBottom(EditorGUIUtility.standardVerticalSpacing * 0.5f);

          serializedProperty.serializedObject.Update();
          LocalizationEditorGUIExtensions.LocalizedStringValueField(propertyRect, GUIContent.none, serializedProperty);
          serializedProperty.serializedObject.ApplyModifiedProperties();
        }
      }
    }

    // Draw a cell that represents a group item in the tree view
    protected override void OnGroupCellGUI(GroupItem item, int columnIndex, Rect columnRect)
    {
      if (columnIndex == 0)
      {
        // Object column
        if (!isSearching)
        {
          columnRect = columnRect.ContractLeft(GetContentIndent(item));
          EditorGUI.LabelField(columnRect, item, boldLabel);
        }
      }
    }

    // Handler for when an item is double clicked
    protected override void OnDoubleClicked(DataItem item)
    {
      SetAsSelection(item);
    }

    // Return a context menu for a data item
    protected override GenericMenu GetDataItemContextMenu(DataItem item)
    {
      var menu = new GenericMenu();

      menu.AddItem(new GUIContent("Show Component"), false, () => SetAsSelection(item));
      menu.AddItem(new GUIContent("Show Asset"), false, () => item.data.asset.SetAsSelection());
      menu.AddItem(new GUIContent("Copy Path"), false, () => GUIUtility.systemCopyBuffer = item.data.asset.assetPath);

      if (item.data.asset.type.IsPrefab())
      {
        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Open Containing Prefab"), false, () => AssetDatabase.OpenAsset(item.data.asset.GetAsset<GameObject>()));
      }
      else if (item.data.asset.type.IsScene())
      {
        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Open Containing Scene"), false, () => EditorSceneManager.OpenScene(item.data.asset.assetPath, OpenSceneMode.Single));
        menu.AddItem(new GUIContent("Open Containing Scene Additive"), false, () => EditorSceneManager.OpenScene(item.data.asset.assetPath, OpenSceneMode.Additive));
      }

      if (item.data.component.componentScript != null)
      {
        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Edit Script"), false, () => AssetDatabase.OpenAsset(item.data.component.componentScript));
      }

      menu.AddSeparator("");

      if (item.data.propertyValue is LocalizedString localizedString && localizedString.isLocalized)
        menu.AddItem(new GUIContent("Find Definition"), false, () => LocaleExplorerWindow.ShowWindow(selected: localizedString.path));
      else
        menu.AddDisabledItem(new GUIContent("Find Definition"), false);

      menu.AddSeparator("");

      menu.AddItem(new GUIContent("Expand All"), false, () => ExpandAll());
      menu.AddItem(new GUIContent("Collapse All"), false, () => CollapseAll());

      return menu;
    }

    // Return if the data of an item matches the specified search query
    protected override bool Matches(PropertySearchResult data, string search)
    {
      return data.asset.assetName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
        || (data.component.componentPath?.Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false)
        || (data.component.componentScript?.name.Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false)
        || data.propertyDisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
        || data.propertyValue is LocalizedString localizedString && (localizedString.isLocalized
          ? localizedString.path.Contains(search, StringComparison.InvariantCultureIgnoreCase)
          : localizedString.value.Contains(search, StringComparison.InvariantCultureIgnoreCase));
    }

    // Set the data item as the selectiom in the editor
    private void SetAsSelection(DataItem item)
    {
      // Check the type of the asset
      if (item.data.asset.type.IsPrefab())
      {
        // Open the prefab
        AssetDatabase.OpenAsset(item.data.asset.GetAsset<GameObject>());
      }
      else if (item.data.asset.type.IsScene())
      {
        // Open the scene if not done already
        var scene = item.data.asset.GetScene();
        if (!scene.IsValid())
          EditorSceneManager.OpenScene(item.data.asset.assetPath, OpenSceneMode.Single);
      }

      // Select the component
      item.data.component.SetAsSelection();
    }
  }
}