using Audune.Utils.UnityEditor.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#if UNITY_6000_2_OR_NEWER
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
#endif

namespace Audune.Localization.Editor
{
  // Class that defines a tree view for the locale explorer window
  public class LocaleExplorerTreeView : ItemTreeView<string>
  {
    // Default keys
    private const string _pathKey = "path";
    private const string _valueKey = "value";

    private static readonly string[] _keys = new[] { _pathKey, _valueKey };

    // Used locales in the tree view
    private readonly IReadOnlyList<Locale> _locales;


    // Constructor
    public LocaleExplorerTreeView(TreeViewState treeViewState, IEnumerable<Locale> locales) : base(treeViewState)
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


    // Draw the key column GUI
    private void OnKeyColumnGUI(Rect rect, DataItem item)
    {
      // Draw a label for the path
      EditorGUI.LabelField(rect, HighlightSearchString(new GUIContent(isSearching ? item.data : item.displayName, item.icon), searchString, _keys, _pathKey), ItemTreeViewStyles.label);
    }

    // Draw the value column GUI
    private void OnValueColumnGUI(Rect rect, DataItem item, Locale locale)
    {
      // Check if the data has a value
      var hasValue = locale.strings.TryFind(item.data, out var value);

      // Draw a label for the value
      EditorGUI.LabelField(rect, hasValue ? HighlightSearchString(value.Replace("\n", " "), searchString, _keys, _valueKey) : "<color=#bf5130>Undefined</color>", ItemTreeViewStyles.label);
    }

    // Handler for when an item is double clicked
    protected override void OnDoubleClicked(DataItem item)
    {
      // Navigate to the references of the item
      NavigateToReferences(item.data);
    }

    // Fill a context menu for the specified data item
    protected override void FillDataItemContextMenu(DataItem item, GenericMenu menu)
    {
      // Items to navigate to the references of the item
      menu.AddItem(new GUIContent("Find References"), false, () => NavigateToReferences(item.data));

      // Fill the base menu items
      menu.AddSeparator("");
      base.FillDataItemContextMenu(item, menu);
    }


    // Navigate to the references of the specified localized string
    private void NavigateToReferences(string data)
    {
      // Open the localized string explorer window
      LocalizedStringExplorerWindow.Open<LocalizedStringExplorerWindow>(searchString: $"value: ={data}");
    }
  }
}