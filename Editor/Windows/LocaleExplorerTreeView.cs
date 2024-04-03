using Audune.Utils.UnityEditor.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for the locale explorer window
  public class LocaleExplorerTreeView : ItemsTreeView<string>
  {
    // Default strings
    private const string _missingDisplayName = "Strings with missing values";
    private const string _stringsDisplayName = "Strings";


    // Default options for the tree view
    private static readonly Options _options = new Options {
      displayNameSelector = key => key.Split('.')[^1],
      iconSelector = key => EditorIcons.text,
      groupIconSelector = (path, expanded) => {
        if (path.Length > 1)
          return expanded ? EditorIcons.folderOpened : EditorIcons.folder;
        else if (path[0] == _missingDisplayName)
          return EditorIcons.errorMark;
        else if (path[0] == _stringsDisplayName)
          return EditorIcons.font;
        else
          return null;
      },
    };


    // The locales used in the tree view
    private List<Locale> _locales;
    private Dictionary<string, List<string>> _values;


    // Constructor
    public LocaleExplorerTreeView(IEnumerable<Locale> locales) : base(LocalesToKeys(locales), _options)
    {
      _locales = new List<Locale>(locales ?? Enumerable.Empty<Locale>());
      _values = LocalesToKeys(_locales).ToDictionary(key => key, key => _locales.Select(locale => locale.strings.Find(key)).Where(value => value != null).ToList());

      multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(Enumerable
        .Repeat(new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Key"), width = 275, canSort = false, allowToggleVisibility = false }, 1)
        .Concat(LocalesToColumms(locales))
        .ToArray()));
    }


    // Build the items of the tree view
    protected override void Build(ref TreeViewItem rootItem, ref int id)
    {
      // Create root items for different types of items
      var missingRootItem = CreateGroupItem(id++, new[] { _missingDisplayName });
      var stringsRootItem = CreateGroupItem(id++, new[] { _stringsDisplayName });

      // Iterate over the items and create data items for them
      var stringsPathItems = new Dictionary<string, GroupItem>();
      foreach (var item in items)
      {
        // Create the data item
        var dataItem = CreateDataItem(id++, item);
        var hasMissingValues = _locales.ContainsMissingString(item);
        if (hasMissingValues)
          dataItem.icon = EditorIcons.errorMark;

        // Create a separate data item if the localized string of the item contains a missing value
        if (hasMissingValues)
          missingRootItem.AddChild(CreateDataItem(id++, item, displayName: item, icon: EditorIcons.errorMark));

        // Create the path items for the string item
        var path = item.Split('.');
        var pathItem = stringsRootItem;
        for (int i = 0; i < path.Length - 1; i++)
        {
          var joinedPath = path[..(i + 1)];
          var joinedPathString = string.Join("/", path[..(i + 1)]);
          if (!stringsPathItems.TryGetValue(joinedPathString, out var newPathItem))
          {
            newPathItem = CreateGroupItem(id++, new[] { _stringsDisplayName }.Concat(joinedPath).ToArray(), path[i]);
            pathItem.AddChild(newPathItem);
            stringsPathItems.Add(joinedPathString, newPathItem);
          }
          pathItem = newPathItem;
        }

        // Add the data item to the correct path item
        pathItem.AddChild(dataItem);
      }

      // Add root items
      if (missingRootItem.hasChildren)
        rootItem.AddChild(missingRootItem);
      rootItem.AddChild(stringsRootItem);
    }

    // Draw a cell that represents a data item in the tree view
    protected override void OnDataCellGUI(DataItem item, int columnIndex, Rect columnRect)
    {
      if (columnIndex == 0)
      {
        // Key column
        if (!isSearching)
          columnRect = columnRect.ContractLeft(GetContentIndent(item));

        var hasMissingValues = !string.IsNullOrEmpty(item.data) && _locales.ContainsMissingString(item.data);
        EditorGUI.LabelField(columnRect, HighlightSearchString(new GUIContent(isSearching ? item.data : item.displayName, hasMissingValues ? EditorIcons.errorMark : item.icon)), label);
      }
      else if (!string.IsNullOrEmpty(item.data))
      {
        // Locale column
        EditorGUI.LabelField(columnRect, HighlightSearchString(_locales[columnIndex - 1].strings.TryFind(item.data, out var value) ? value.Replace("\n", " ") : "<color=red><Undefined></color>"), label);
      }
    }

    // Draw a cell that represents a group item in the tree view
    protected override void OnGroupCellGUI(GroupItem item, int columnIndex, Rect columnRect)
    {
      if (columnIndex == 0)
      {
        // Key column
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
      LocalizedStringExplorerWindow.ShowWindow(searchString: item.data);
    }

    // Return a context menu for a data item
    protected override GenericMenu GetDataItemContextMenu(DataItem item)
    {
      var menu = new GenericMenu();

      menu.AddItem(new GUIContent("Find References"), false, () => LocalizedStringExplorerWindow.ShowWindow(searchString: item.data));

      menu.AddSeparator("");

      menu.AddItem(new GUIContent("Expand All"), false, () => ExpandAll());
      menu.AddItem(new GUIContent("Collapse All"), false, () => CollapseAll());

      return menu;
    }

    // Return if an item matches the specified search query
    protected override bool Matches(string data, string search)
    {
      if (string.IsNullOrEmpty(data))
        return false;
      if (string.IsNullOrEmpty(search))
        return true;

      return data.Contains(search, StringComparison.InvariantCultureIgnoreCase) 
        || (_values.TryGetValue(data, out var values) && values.Any(value => value.Contains(search, StringComparison.InvariantCultureIgnoreCase)));    
    }


    #region Convert locales to keys and columns
    // Convert a list of locales to keys
    private static IEnumerable<string> LocalesToKeys(IEnumerable<Locale> locales)
    {
      return locales?.SelectMany(locale => locale.strings.Keys).Distinct() ?? Enumerable.Empty<string>();
    }

    // Convert a list of locales to tree view columns
    private static IEnumerable<MultiColumnHeaderState.Column> LocalesToColumms(IEnumerable<Locale> locales)
    {
      return locales?.Select(locale => new MultiColumnHeaderState.Column() { headerContent = new GUIContent($"{locale.englishName} Value"), width = 150, canSort = false }) ?? Enumerable.Empty<MultiColumnHeaderState.Column>();
    }
    #endregion
  }
}