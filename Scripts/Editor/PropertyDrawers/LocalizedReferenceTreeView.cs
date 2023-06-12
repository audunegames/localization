using Audune.Utils.Unity;
using Audune.Utils.Unity.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for localized references
  // TODO: Make keys dependent on the type of the localized reference
  public class LocalizedReferenceTreeView : SearchTreeView<string>
  {
    // The locales used in the tree view
    private List<Locale> _locales;
    private Dictionary<string, List<string>> _values;


    // Constructor
    public LocalizedReferenceTreeView(LocalizedReferenceSearchWindow window, List<Locale> locales) : base(window, LocalesToKeys(locales), key => key.Replace('.', '/'))
    {
      _locales = locales;
      _values = LocalesToKeys(_locales).ToDictionary(key => key, key => _locales.Select(locale => locale.Strings.Find(key)).Where(value => value != null).ToList());

      multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(Enumerable
        .Repeat(new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Key"), width = 200, canSort = false, allowToggleVisibility = false }, 1)
        .Concat(LocalesToColumms(locales))
        .ToArray()));
    }


    // Create a tree view item for the specified item
    protected override TreeViewItem BuildItem(string item, int index, int depth = 0)
    {
      return new TreeViewItem { id = index, depth = depth, displayName = item.Split('.')[^1] };
    }

    // Return if an item matches the specified search query
    protected override bool Matches(string item, string search)
    {
      var key = items.Find(i => i == item);
      return key.Contains(search, StringComparison.InvariantCultureIgnoreCase) || (_values.TryGetValue(key, out var values) && values.Any(value => value.Contains(search, StringComparison.InvariantCultureIgnoreCase)));    
    }


    // Draw a cell that represents an item in the tree view
    protected override void OnItemCellGUI(TreeViewCell cell, string item)
    {
      if (cell.columnIndex == 0 && string.IsNullOrEmpty(searchString))
        EditorGUI.LabelField(cell.position.ContractLeft((cell.treeViewItem.depth + 1) * 14), cell.treeViewItem.displayName);
      else if (cell.columnIndex == 0)
        EditorGUI.LabelField(cell.position, item);
      else
        EditorGUI.LabelField(cell.position, _locales[cell.columnIndex - 1].Strings.TryFind(item, out var value) ? value.Replace("\n", " ") : "<Undefined>");
    }

    // Draw a cell that represents a group in the tree view
    protected override void OnGroupCellGUI(TreeViewCell cell)
    {
      if (cell.columnIndex == 0 && string.IsNullOrEmpty(searchString))
        EditorGUI.LabelField(cell.position.ContractLeft((cell.treeViewItem.depth + 1) * 14), cell.treeViewItem.displayName, EditorStyles.boldLabel);
      else if (cell.columnIndex == 0)
        EditorGUI.LabelField(cell.position, cell.treeViewItem.displayName, EditorStyles.boldLabel);
    }


    // Convert a list of locales to keys
    private static IEnumerable<string> LocalesToKeys(List<Locale> locales)
    {
      return locales.SelectMany(locale => locale.Strings.RecursiveEntries.Keys).Distinct();
    }

    // Convert a list of locales to tree view columns
    private static IEnumerable<MultiColumnHeaderState.Column> LocalesToColumms(List<Locale> locales)
    {
      return locales.Select(locale => new MultiColumnHeaderState.Column() { headerContent = new GUIContent(locale.Name), width = 150, canSort = false, allowToggleVisibility = false });
    }
  }
}