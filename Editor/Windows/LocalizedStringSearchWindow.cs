using Audune.Utils.UnityEditor.Editor;

namespace Audune.Localization.Editor
{
  // Class that defines a search window for selecting a localized string reference
  public class LocalizedStringSearchWindow : SearchWindow<string>
  {
    // Create the tree view
    public override SearchTreeView<string> CreateTreeView()
    {
      return new LocalizedStringSearchTreeView(Locale.GetAllLocaleAssets("Assets"));
    }

    // Get the property value
    public override string GetPropertyValue()
    {
      return serializedProperty?.stringValue ?? default;
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