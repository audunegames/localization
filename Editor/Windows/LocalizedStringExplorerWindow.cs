using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Audune.Localization.Editor
{
  // Class that defines an editor window for exploring localized strings in the project
  [EditorWindowTitle(title = "Localized String Explorer")]
  public sealed class LocalizedStringExplorerWindow : SerializedPropertySearchWindow<LocalizedStringExplorerTreeView>
  {
    // Context menu item for opening the window
    [MenuItem("Window/Audune Localization/Localized String Explorer _%#&S", secondaryPriority = 1)]
    public static void Open()
    {
      Open<LocalizedStringExplorerWindow>();
    }


    // Return if the search field should be expanded
    public override bool expandSearchField => false;


    // Initialize the tree view
    protected override LocalizedStringExplorerTreeView Initialize(TreeViewState treeViewState)
    {
      return new LocalizedStringExplorerTreeView(treeViewState);
    }
  }
}