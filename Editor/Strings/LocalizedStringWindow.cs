using Audune.Localization.Settings;
using Audune.Utils.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Strings.Editor
{
  // Class that defines a search window for selecting a localized string reference
  public class LocalizedStringWindow : SearchWindow<string>
  {
    // The list of loaded locales for the
    private List<Locale> _loadedLocales;


    // Show the window as a dropdown at the specified button position
    public static void ShowAsDropDown(Rect buttonRect, SerializedProperty property)
    {
      ShowAsDropDown<LocalizedStringWindow>(buttonRect, property);
    }


    // Refresh
    public void Refresh()
    {
      var assets = AssetDatabase.FindAssets($"t:{typeof(LocalizationSettings).Name}");
      var settingsAsset = assets.Length > 0 ? AssetDatabase.GUIDToAssetPath(assets[0]) : null;
      var settings = settingsAsset != null ? AssetDatabase.LoadAssetAtPath<LocalizationSettings>(settingsAsset) : null;

      if (settings != null)
        _loadedLocales = settings.LoadLocales();
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
      return new LocalizedStringTreeView(_loadedLocales);
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