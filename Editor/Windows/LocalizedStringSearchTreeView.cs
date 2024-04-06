using Audune.Utils.UnityEditor.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for selecting a localized string reference
  public class LocalizedStringSearchTreeView : SearchTreeView<string>
  {
    // Default options for the tree view
    private static readonly Options _options = new Options {
      displayNameSelector = key => key.Split('.')[^1],
      iconSelector = key => EditorIcons.text,
      groupIconSelector = (path, expanded) => expanded ? EditorIcons.folderOpened : EditorIcons.folder,
      pathSelector = key => key.Split('.'),
      addDefaultItem = true,
      defaultItemData = null,
      defaultItemDisplayName = "<Non-Localized Value>",
    };


    // The locales used in the tree view
    private List<Locale> _locales;
    private Dictionary<string, List<string>> _values;


    // Constructor
    public LocalizedStringSearchTreeView(IEnumerable<Locale> locales) : base(LocalesToKeys(locales), _options)
    {
      _locales = new List<Locale>(locales ?? Enumerable.Empty<Locale>());
      _values = LocalesToKeys(_locales).ToDictionary(key => key, key => _locales.Select(locale => locale.strings.Find(key)).Where(value => value != null).ToList());

      multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(Enumerable
        .Repeat(new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Key"), width = 275, canSort = false, allowToggleVisibility = false }, 1)
        .Concat(LocalesToColumms(locales))
        .ToArray()));
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
        var hasValue = _locales[columnIndex - 1].strings.TryFind(item.data, out var value);
        EditorGUI.LabelField(columnRect, HighlightSearchString(hasValue ? value.Replace("\n", " ") : "<color=#bf5130>Undefined</color>"), label);
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
      return locales?.Select(locale => new MultiColumnHeaderState.Column() { headerContent = new GUIContent($"{locale.englishName} Value"), canSort = false, width = 150 }) ?? Enumerable.Empty<MultiColumnHeaderState.Column>();
    }
    #endregion
  }
}