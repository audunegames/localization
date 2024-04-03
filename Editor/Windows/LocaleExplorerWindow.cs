using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines an editor window for exploring locales in the project
  [EditorWindowTitle(title = "Locale Explorer")]
  public sealed class LocaleExplorerWindow : EditorWindow
  {
    // Show the window
    public static void ShowWindow(string searchString = null, string selected = null)
    {
      var window = GetWindow<LocaleExplorerWindow>();
      window.minSize = new Vector2(800, 600);
      window.Refresh(searchString, selected);
    }

    // Show the window without context
    [MenuItem("Window/Audune Localization/Locale Explorer _%#&L", secondaryPriority = 0)]
    public static void ShowWindow()
    {
      ShowWindow(null, null);
    }


    // Tree view for displaying the localized strings
    private LocaleExplorerTreeView _treeView;


    // Refresh the window
    private void Refresh(string searchString = null, string selected = null, bool forceRebuild = false)
    {
      Rebuild(forceRebuild);
      _treeView.Reload();

      if (!string.IsNullOrEmpty(searchString))
        _treeView.searchString = searchString;
      else if (selected != null)
        _treeView.SetSelectionData(selected);
    }

    // Rebuild the tree view
    private void Rebuild(bool forceRebuild = false)
    {
      if (forceRebuild || _treeView == null)
        _treeView = new LocaleExplorerTreeView(Locale.GetAllLocaleAssets());
    }


    // OnGUI is called when the editor is drawn
    private void OnGUI()
    {
      Rebuild();

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
      if (GUILayout.Button(new GUIContent("Rescan Project", EditorIcons.refresh, "Rescan the project for locales"), EditorStyles.toolbarButton, GUILayout.Width(111)))
        Refresh(searchString: _treeView.searchString, selected: _treeView.GetSelectionData(), forceRebuild: true);

      // Locales dropdown
      if (GUILayout.Button(new GUIContent("Locales", EditorIcons.folderOpened, "Edit one of the locales in the project"), EditorStyles.toolbarDropDown, GUILayout.Width(80)))
      {
        var menu = new GenericMenu();
        foreach (var locale in Locale.GetAllLocaleAssets())
        {
          menu.AddItem(new GUIContent($"{locale.englishName}/Show Asset"), false, () => {
            Selection.SetActiveObjectWithContext(locale, locale);
            EditorGUIUtility.PingObject(locale);
          });
          menu.AddItem(new GUIContent($"{locale.englishName}/Copy Path"), false, () => GUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(locale));
          menu.AddSeparator($"{locale.englishName}/");
          menu.AddItem(new GUIContent($"{locale.englishName}/Edit Source"), false, () => AssetDatabase.OpenAsset(locale));
        }

        var rect = GUILayoutUtility.GetLastRect();
        rect.x += 111;
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        menu.DropDown(rect);
      }

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