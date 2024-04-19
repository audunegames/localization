using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines an editor window for exploring locales in the project
  [EditorWindowTitle(title = "Locale Explorer")]
  public sealed class LocaleExplorerWindow : ItemWindow<string, LocaleExplorerTreeView>
  {
    // Context menu item for opening the window
    [MenuItem("Window/Audune Localization/Locale Explorer _%#&L", secondaryPriority = 0)]
    public static void Open()
    {
      Open<LocaleExplorerWindow>();
    }


    // Return if the search field should be expanded
    public override bool expandSearchField => false;


    // Initialize the tree view
    protected override LocaleExplorerTreeView Initialize(TreeViewState treeViewState)
    {
      return new LocaleExplorerTreeView(treeViewState, Locale.GetAllLocaleAssets()); ;
    }

    // OnButtonsGUI is called when the buttons on the toolbar are drawn
    protected override void OnToolbarButtonsGUI()
    {
      // Draw the base buttons
      base.OnToolbarButtonsGUI();

      // Locales dropdown
      if (GUILayout.Button(new GUIContent("Locales", EditorIcons.folder, "Edit one of the locales in the project"), EditorStyles.toolbarDropDown, GUILayout.Width(80)))
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
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        menu.DropDown(rect);
      }
    }
  }
}