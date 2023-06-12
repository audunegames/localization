using Audune.Utils.Unity.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a search window for localized references
  public class LocalizedReferenceSearchWindow : SearchWindow<string>
  {
    private static List<Locale> _loadedLocales = new List<Locale>();


    // Open the window
    public static void Open(SerializedProperty property, GUIContent title)
    {
      Open<LocalizedReferenceSearchWindow>(property, title);
    }


    // Create the tree view
    public override SearchTreeView<string> CreateTreeView()
    {
      var assets = AssetDatabase.FindAssets($"t:{typeof(LocalizationSettings).Name}");
      var settingsAsset = assets.Length > 0 ? AssetDatabase.GUIDToAssetPath(assets[0]) : null;
      var settings = settingsAsset != null ? AssetDatabase.LoadAssetAtPath<LocalizationSettings>(settingsAsset) : null;

      if (settings != null)
        _loadedLocales = settings.LoadLocales();

      return new LocalizedReferenceTreeView(this, _loadedLocales);
    }

    // Get the property value
    public override string GetPropertyValue()
    {
      return property.stringValue;
    }

    // Set the property value
    public override void SetPropertyValue(string value)
    {
      property.serializedObject.Update();
      property.stringValue = value;
      property.serializedObject.ApplyModifiedProperties();

      EditorUtility.SetDirty(property.serializedObject.targetObject);
    }
  }
}