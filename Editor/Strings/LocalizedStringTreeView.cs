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
  public class LocalizedStringTreeView : SearchTreeView<string>
  {
    // Default options for the localized reference search tree view
    public static readonly Options LocalizedReferenceOptions = new Options {
      pathSelector = key => key.Replace('.', '/'),
      displayNameSelector = key => key.Split('.')[^1],
      addDefaultItem = true,
      defaultItemData = null,
      defaultItemDisplayName = "<Non-Localized Value>",
    };


    // The locales used in the tree view
    private List<Locale> _locales;
    private Dictionary<string, List<string>> _values;


    // Constructor
    public LocalizedStringTreeView(List<Locale> locales) : base(LocalesToKeys(locales), LocalizedReferenceOptions)
    {
      _locales = locales;
      _values = LocalesToKeys(_locales).ToDictionary(key => key, key => _locales.Select(locale => locale.strings.Find(key)).Where(value => value != null).ToList());

      multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(Enumerable
        .Repeat(new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Key"), width = 300, allowToggleVisibility = false }, 1)
        .Concat(LocalesToColumms(locales))
        .ToArray()));
      showAlternatingRowBackgrounds = true;
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


    // Draw a cell that represents a data item in the tree view
    protected override void OnDataCellGUI(DataItem item, int columnIndex, Rect columnRect)
    {
      if (columnIndex == 0)
      {
        var label = string.IsNullOrEmpty(searchString) ? item.displayName : item.data;
        if (string.IsNullOrEmpty(searchString))
          DefaultGUI.Label(columnRect.ContractLeft(GetContentIndent(item)), label, IsSelected(item.id), false);
        else
          DefaultGUI.Label(columnRect, HighlightSearchString(label), IsSelected(item.id), false);
      }
      else if (!string.IsNullOrEmpty(item.data))
      {
        var label = _locales[columnIndex - 1].strings.TryFind(item.data, out var value) ? value.Replace("\n", " ") : "<Undefined>";
        if (string.IsNullOrEmpty(searchString))
          DefaultGUI.Label(columnRect, label, IsSelected(item.id), false);
        else
          DefaultGUI.Label(columnRect, HighlightSearchString(label), IsSelected(item.id), false);
      }        
    }

    // Draw a cell that represents a group item in the tree view
    protected override void OnGroupCellGUI(GroupItem item, int columnIndex, Rect columnRect)
    {
      if (columnIndex == 0)
      {
        if (string.IsNullOrEmpty(searchString))
          DefaultGUI.FoldoutLabel(columnRect.ContractLeft(GetContentIndent(item)), item.displayName, IsSelected(item.id), false);
      }
    }


    // Convert a list of locales to keys
    private static IEnumerable<string> LocalesToKeys(List<Locale> locales)
    {
      return locales?.SelectMany(locale => locale.strings.Keys).Distinct() ?? Enumerable.Empty<string>();
    }

    // Convert a list of locales to tree view columns
    private static IEnumerable<MultiColumnHeaderState.Column> LocalesToColumms(List<Locale> locales)
    {
      return locales?.Select(locale => new MultiColumnHeaderState.Column() { headerContent = new GUIContent(locale.nativeName), width = 150 }) ?? Enumerable.Empty<MultiColumnHeaderState.Column>();
    }
  }
}