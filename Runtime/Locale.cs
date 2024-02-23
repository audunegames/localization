using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a locale
  [CreateAssetMenu(menuName = "Audune/Localization/Locale", fileName = "Locale")]
  public sealed class Locale : ScriptableObject, ILocalizedTable<string>
  {
    // Locale settings
    [SerializeField, Tooltip("The ISO 639 code of the locale")]
    internal string _code;
    [SerializeField, Tooltip("The alternative codes of the locale, in the format"), SerializableDictionaryOptions(keyHeader = "Code")]
    internal SerializableDictionary<string, string> _altCodes;
    [SerializeField, Tooltip("The English name of the locale")]
    internal string _englishName;
    [SerializeField, Tooltip("The native name of the locale")]
    internal string _nativeName;
    [SerializeField, Tooltip("The pluralization rules of the locale"), SerializableDictionaryOptions(keyHeader = "Keyword")]
    internal SerializableDictionary<PluralKeyword, string> _pluralRules;
    [SerializeField, Tooltip("The format of the locale")]
    internal LocaleFormat _format;

    // Locale tables
    [SerializeField, Tooltip("The strings table of the locale")]
    internal LocalizedStringTable _strings = new LocalizedStringTable();


    // Return the code of the locale
    public string code => _code;

    // Return the alternative codes of the locale
    public IReadOnlyDictionary<string, string> altCodes => _altCodes;

    // Return the English name of the locale
    public string englishName => _englishName;

    // Return the native name of the locale
    public string nativeName => _nativeName;

    // Return the pluralization rules of the locale
    public PluralRules pluralRules => new PluralRules(_pluralRules);

    // Return the format of the locale
    public LocaleFormat localeFormat => _format;


    // Return the culture of the locale
    public CultureInfo culture {
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
    public LocalizedStringTable strings => _strings;


    // Return if an entry in the strings table with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out string value)
    {
      return _strings.TryFind(path, out value);
    }


    // Return the string representation of the locale
    public override string ToString()
    {
      return _nativeName;
    }
  }
}
