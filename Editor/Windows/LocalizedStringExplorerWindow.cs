using Audune.Utils.UnityEditor.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines an editor window for exploring localized strings in the project
  [EditorWindowTitle(title = "Localized String Explorer")]
  public sealed class LocalizedStringExplorerWindow : EditorWindow
  {
    // Show the window
    public static void ShowWindow(string searchString = null)
    {
      var window = GetWindow<LocalizedStringExplorerWindow>();
      window.minSize = new Vector2(800, 600);
      window.Refresh(searchString);
    }

    // Show the window without context
    [MenuItem("Window/Audune Localization/Localized String Explorer _%#&S", secondaryPriority = 1)]
    public static void ShowWindow()
    {
      ShowWindow(null);
    }


    // Tree view for displaying the localized strings
    private LocalizedStringExplorerTreeView _treeView;


    // Refresh the window
    private void Refresh(string searchString = null, bool forceRebuild = false)
    {

      Rebuild(forceRebuild);
      _treeView.Reload();

      if (!string.IsNullOrEmpty(searchString))
        _treeView.searchString = searchString;
    }

    // Rebuild the tree view
    private void Rebuild(bool forceRebuild = false)
    {
      if (forceRebuild || _treeView == null)
      {
        var properties = PropertySearch.SearchInProject("Assets").OrderBy(r => r.asset.assetDirectoryName);
        _treeView = new LocalizedStringExplorerTreeView(properties, Locale.GetAllLocaleAssets());
      }
    }


    // OnGUI is called when the editor is drawn
    private void OnGUI()
    {
      if (_treeView == null)
        Refresh();

      // Draw the search box
      GUILayout.BeginHorizontal(EditorStyles.toolbar);
      OnToolbarGUI();
      GUILayout.EndHorizontal();

      // Draw the tree view
      OnTreeViewGUI();
    }


    // OnToolbarGUI is called when the toolbar is drawn
    private void OnToolbarGUI()
    {
      // Rescan project button
      if (GUILayout.Button(new GUIContent("Rescan Project", EditorIcons.refresh, "Rescan the project for assets that contain localized strings"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        Refresh(searchString: _treeView.searchString, forceRebuild: true);

      GUILayout.FlexibleSpace();

      // Search bar
      _treeView.searchString = EditorGUILayout.TextField(_treeView.searchString, EditorStyles.toolbarSearchField, GUILayout.MaxWidth(300));

      // Tree view buttons
      if (GUILayout.Button(new GUIContent("Expand All", "Expand all tree view items"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        _treeView.ExpandAll();
      if (GUILayout.Button(new GUIContent("Collapse All", "Collapse all tree view items"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        _treeView.CollapseAll();
    }

    // OnTreeViewGUI is called when the tree view is drawn
    private void OnTreeViewGUI()
    {
      var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
      _treeView.Reload();
      _treeView.OnGUI(rect);
    }
  }
}