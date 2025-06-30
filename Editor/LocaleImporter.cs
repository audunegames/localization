using Audune.Utils.Types;
using Audune.Utils.Types.Editor;
using System;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a scripted importer for localized strings
  [ScriptedImporter(202203002, new[] { "locale" }, new[] { "toml", "po" })]
  public class LocaleImporter : ScriptedImporter
  {
    // Locale importer settings
    [SerializeField, Tooltip("The format of the locale file"), SerializableTypeOptions(typeof(LocaleParser), TypeDisplayOptions.DontShowNamespace)]
    private SerializableType _localeFileFormat = typeof(LocaleParser).GetChildTypes().FirstOrDefault();


    // Import the asset
    public override void OnImportAsset(AssetImportContext context)
    {
      // Create the parser
      if (_localeFileFormat.type == null)
        throw new ArgumentException("Cannot parse locale without a specified format");

      var parser = Activator.CreateInstance(_localeFileFormat) as LocaleParser;

      // Parse the locale
      var locale = parser.Parse(context.assetPath);
      if (locale == null)
        throw new NotImplementedException($"Cannot parse locale as format {_localeFileFormat.type.ToDisplayString()}");

      // Add the locale to the asset
      context.AddObjectToAsset("locale", locale);
      context.SetMainObject(locale);
    }
  }
}