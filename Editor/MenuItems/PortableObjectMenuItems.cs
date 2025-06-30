using System.IO;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines menu items for Portable Object files
  public static class PortableObjectMenuItems
  {
    // Save the locale in the selected asset to a Portable Object file
    [MenuItem("Assets/Audune Localization/Save To Portable Object file (.po)")]
    public static void SaveSelectedToPortableObjectFile()
    {
      var locale = GetLocaleFromAsset(Selection.activeObject);
      if (locale == null)
        return;

      var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
      var path = Path.GetFullPath(Path.Combine($"{Application.dataPath}/..", $"{assetPath.Substring(0, assetPath.LastIndexOf("."))}.po.locale"))
        .Replace('/', Path.DirectorySeparatorChar);

      path = EditorUtility.SaveFilePanelInProject($"Save Portable Object file for {Selection.activeObject.name}", path, "locale", $"Enter a file name to save the Portable Object file for {Selection.activeObject.name} to");
      if (!string.IsNullOrEmpty(path))
      {
        var writer = new PortableObjectLocaleWriter();
        writer.Write(locale, path);
        AssetDatabase.Refresh();
      }
    }

    [MenuItem("Assets/Audune Localization/Save To Portable Object file (.po)", true)]
    public static bool ValidateSaveSelectedToPortableObjectFile()
    {
      return GetLocaleFromAsset(Selection.activeObject) != null;
    }

    // Save the locale in the selected asset to a template Portable Object file
    [MenuItem("Assets/Audune Localization/Save To Template Portable Object file (.po)")]
    public static void SaveSelectedToEmptyPortableObjectFile()
    {
      var locale = GetLocaleFromAsset(Selection.activeObject);
      if (locale == null)
        return;

      var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
      var path = Path.GetFullPath(Path.Combine($"{Application.dataPath}/..", $"{assetPath.Substring(0, assetPath.LastIndexOf("."))}.po.locale"))
        .Replace('/', Path.DirectorySeparatorChar);

      path = EditorUtility.SaveFilePanelInProject($"Save Portable Object file for {Selection.activeObject.name}", path, "locale", $"Enter a file name to save the Portable Object file for {Selection.activeObject.name} to");
      if (!string.IsNullOrEmpty(path))
      {
        var writer = new PortableObjectLocaleWriter(true);
        writer.Write(locale, path);
        AssetDatabase.Refresh();
      }
    }

    [MenuItem("Assets/Audune Localization/Save To Template Portable Object file (.po)", true)]
    public static bool ValidateSaveSelectedToEmptyPortableObjectFile()
    {
      return GetLocaleFromAsset(Selection.activeObject) != null;
    }


    // Get the locale from an asset
    private static Locale GetLocaleFromAsset(Object asset)
    {
      var assetPath = AssetDatabase.GetAssetPath(asset);
      return assetPath != "" ? AssetDatabase.LoadAssetAtPath<Locale>(assetPath) : null;
    }
  }
}