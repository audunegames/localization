using Audune.Utils.UnityEditor.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for selecting a localized string reference
  public class LocalizedStringSelectorTreeView : ItemSelectorTreeView<string>
  {
    // Default keys
    private const string _pathKey = "path";
    private const string _valueKey = "value";

    private static readonly string[] _keys = new[] { _pathKey, _valueKey };


    // Used locales in the tree view
    private readonly IReadOnlyList<Locale> _locales;


    // Return if a default item should be added
    protected override bool _addDefaultItem => true;


    // Constructor
    public LocalizedStringSelectorTreeView(TreeViewState treeViewState, IEnumerable<Locale> locales) : base(treeViewState)
    {
      // Set the used locales
      _locales = locales.ToList();

      // Set the tree view items
      items = _locales.GetStrings().ToList();

      // Set the tree view columns
      columns = Enumerable.Repeat(new Column(new GUIContent("Path"), OnKeyColumnGUI, width: 275), 1)
        .Concat(_locales.Select(locale => new Column(new GUIContent($"{locale.englishName} Value"), (rect, item) => OnValueColumnGUI(rect, item, locale), width: 150, isHideable: true)))
        .ToList();

      // Set the tree view item matcher
      matcher = ItemMatcher.Keys(
        (_pathKey, ItemMatcher.String()),
        (_valueKey, ItemMatcher.String<string>(data => _locales.GetValues(data).Select(e => e.Value))));
    }


    // Return the path for a data item
    protected override string[] SelectDataPath(string data)
    {
      return data.Split('.');
    }

    // Return the display string for a data item
    protected override string SelectDataDisplayName(string data)
    {
      return data.Split('.')[^1];
    }

    // Return the icon for a data item
    protected override Texture2D SelectDataIcon(string data)
    {
      // Check if the data has missing values
      var hasMissingValues = !string.IsNullOrEmpty(data) && _locales.ContainsMissingString(data);
      return hasMissingValues ? EditorIcons.errorMark : EditorIcons.text;
    }

    // Return the data for the default item
    protected override string SelectDefaultData()
    {
      return null;
    }

    // Return the display name for the default item
    protected override string SelectDefaultDisplayName()
    {
      return "<Non-Localized Value>";
    }


    // Draw the key column GUI
    private void OnKeyColumnGUI(Rect rect, DataItem item)
    {
      // Draw a label for the path
      EditorGUI.LabelField(rect, HighlightSearchString(new GUIContent(isSearching ? item.data : item.displayName, item.icon), searchString, _keys, _pathKey), ItemTreeViewStyles.label);
    }

    // Draw the value column GUI
    private void OnValueColumnGUI(Rect rect, DataItem item, Locale locale)
    {
      // Check if the item has any data
      if (item.data == null)
        return;

      // Check if the data has a value
      var hasValue = locale.strings.TryFind(item.data, out var value);

      // Draw a label for the value
      EditorGUI.LabelField(rect, hasValue ? HighlightSearchString(value.Replace("\n", " "), searchString, _keys, _valueKey) : "<color=#bf5130>Undefined</color>", ItemTreeViewStyles.label);
    }
  }
}