using Audune.Utils.UnityEditor.Editor;
using UnityEditor.IMGUI.Controls;

namespace Audune.Localization.Editor
{
  // Class that defines an editor window for selecting a localized string reference
  public class LocalizedStringSelectorWindow : ItemSelectorWindow<string, LocalizedStringSelectorTreeView>
  {
    // Create the tree view
    public override LocalizedStringSelectorTreeView CreateTreeView(TreeViewState treeViewState)
    {
      return new LocalizedStringSelectorTreeView(treeViewState, Locale.GetAllLocaleAssets("Assets"));
    }

    // Get the property value
    public override string GetPropertyValue()
    {
      return serializedProperty?.stringValue ?? null;
    }

    // Set the property value
    public override void SetPropertyValue(string value)
    {
      if (serializedProperty != null)
      {
        serializedProperty.serializedObject.Update();
        serializedProperty.stringValue = value;
        serializedProperty.serializedObject.ApplyModifiedProperties();
      }
    }
  }
}