using Audune.Utils.UnityEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a search window for selecting a localized string reference
  public class LocalizedStringSearchWindow : SearchWindow<string>
  {
    // Reference to the localization system
    private LocalizationSystem _localizationSystem;


    // Refresh the search window
    public void Refresh()
    {
      _localizationSystem = FindObjectOfType<LocalizationSystem>();
      if (_localizationSystem != null)
        _localizationSystem.InitializeIfNoLocaleSelected();
    }


    // OnToolbarGUI is called when the toolbar is drawn
    protected override void OnToolbarGUI()
    {
      if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(80)))
        Refresh();

      base.OnToolbarGUI();
    }


    // Create the tree view
    public override SearchTreeView<string> CreateTreeView()
    {
      Refresh();
      return new LocalizedStringSearchTreeView(_localizationSystem.loadedLocales);
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