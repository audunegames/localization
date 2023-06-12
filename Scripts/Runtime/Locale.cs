using System;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a set of localized strings
  [Serializable]
  public sealed class Locale : ScriptableObject
  {
    // Locale settings
    [SerializeField, Tooltip("The ISO 639-1 code of the locale")]
    internal string _code;
    [SerializeField, Tooltip("The display name of the locale")]
    internal string _name;

    // Locale tables
    [SerializeField, Tooltip("The strings table of the locale")]
    internal LocalizedTable<string> _strings = new LocalizedTable<string>();


    // Return the code of the locale
    public string Code => _code;

    // Return the name of the locale
    public string Name => _name;

    // Return the strings of the locale
    public LocalizedTable<string> Strings => _strings;


    // Return the culture of the locale
    public CultureInfo GetCultureInfo()
    {
      try {
        return CultureInfo.GetCultureInfo(_code);
      } catch (CultureNotFoundException) {
        return CultureInfo.InvariantCulture;
      }
    }


    // Return the string representation of the locale
    public override string ToString()
    {
      return $"{_name} ({_code})";
    }
  }
}
