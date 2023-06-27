using Audune.Localization.Plurals;
using Audune.Localization.Strings;
using Audune.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a locale
  [CreateAssetMenu(menuName = "Audune Localization/Locale")]
  public sealed class Locale : ScriptableObject, ILocalizedTable<string>
  {
    // Locale settings
    [SerializeField, Tooltip("The ISO 639 code of the locale")]
    internal string _code;
    [SerializeField, Tooltip("The alternative codes of the locale, in the format"), SerializableDictionaryDrawerOptions(ReorderableListDrawOptions.DrawFoldout | ReorderableListDrawOptions.DrawInfoField)]
    internal SerializableDictionary<string, string> _altCodes;
    [SerializeField, Tooltip("The name of the locale")]
    internal string _name;
    [SerializeField, Tooltip("The pluralization rules of the locale"), SerializableDictionaryDrawerOptions(ReorderableListDrawOptions.DrawFoldout | ReorderableListDrawOptions.DrawInfoField)]
    internal SerializableDictionary<PluralKeyword, string> _pluralRules;
    [SerializeField, Tooltip("The format of the locale")]
    internal LocaleFormat _format;

    // Locale tables
    [SerializeField, Tooltip("The strings table of the locale")]
    internal LocalizedStringTable _strings = new LocalizedStringTable();


    // Return the code of the locale
    public string Code => _code;

    // Return the alternative codes of the locale
    public IReadOnlyDictionary<string, string> AltCodes => _altCodes;

    // Return the name of the locale
    public string Name => _name;

    // Return the pluralization rules of the locale
    public PluralRules PluralRules => new PluralRules(_pluralRules);

    // Return the format of the locale
    public LocaleFormat LocaleFormat => _format;


    // Return the culture of the locale
    public CultureInfo Culture {
      get {
        try
        {
          return CultureInfo.GetCultureInfo(_code);
        }
        catch (Exception ex) when (ex is CultureNotFoundException || ex is ArgumentException)
        {
          return CultureInfo.InvariantCulture;
        }
      }
    }


    // Return the strings of the locale
    public LocalizedStringTable Strings => _strings;


    // Return if an entry in the strings table with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out string value)
    {
      return _strings.TryFind(path, out value);
    }

    // Return the value of the entry in the strings table with the specified path, or a default value if one cannot be found
    public string Find(string path, string defaultValue = default)
    {
      return _strings.Find(path, defaultValue);
    }


    // Return the string representation of the locale
    public override string ToString()
    {
      return $"{_name} ({Code})";
    }
  }
}
