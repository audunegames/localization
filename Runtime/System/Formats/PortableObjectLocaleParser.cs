using System.Collections.Generic;
using System.IO;
using Audune.Utils.Dictionary;
using Audune.Utils.Types;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that parses a locale file in the Portable Object format.
  /// </summary>
  [TypeDisplayName("Portable Object (.po)")]
  public sealed class PortableObjectLocaleParser : LocaleParser
  {
    /// <summary>
    /// Parse a locale from a text reader.
    /// </summary>
    /// <param name="textReader">The text reader to parse the locale from.</param>
    /// <returns>The parsed locale.</returns>
    public override Locale Parse(TextReader textReader)
    {
      var reader = new PortableObjectReader(textReader);

      // Create the locale
      var locale = ScriptableObject.CreateInstance<Locale>();
      var strings = new Dictionary<string, string>();

      // Read messages from the reader and parse them
      try
      {
        while (reader.TryRead(out var message))
        {
          // Check if the message is a header message
          if (string.IsNullOrEmpty(message.untranslatedString))
          {
            // Decode the header
            var header = new PortableObjectHeader();
            header.DecodeMessage(message);
            DecodeHeader(header, locale);
          }
          else
          {
            // Add the message to the strings
            strings.Add(message.uniqueKey, message.translatedString);
          }
        }
      }
      catch (PortableObjectException ex)
      {
        throw new LocaleParserException(ex.Message, ex);
      }

      // Create the strings
      locale._strings = new LocalizedStringTable(strings);

      // Return the locale
      return locale;
    }


    /// <summary>
    /// Decode the header into the specified locale.
    /// </summary>
    private void DecodeHeader(PortableObjectHeader header, Locale locale)
    {
      // Decode the code and names
      locale._code = header["Language"];
      locale._englishName = header["X-English-Name"];
      locale._nativeName = header["X-Native-Name"];

      // Decode the alt codes
      locale._altCodes = new SerializableDictionary<string, string>();
      foreach (var e in header)
      {
        if (!e.Key.StartsWith("X-Alt-Code-"))
          continue;

        var key = e.Key[11..].ToLower();
        locale._altCodes[key] = e.Value;
      }

      // Decode the number formats
      locale._decimalNumberFormat = header.GetValue("X-Decimal-Number-Format", Locale.defaultDecimalNumberFormat);
      locale._percentNumberFormat = header.GetValue("X-Percent-Number-Format", Locale.defaultPercentNumberFormat);
      locale._currencyNumberFormat = header.GetValue("X-Currency-Number-Format", Locale.defaultCurrencyNumberFormat);
      locale._shortDateFormat = header.GetValue("X-Short-Date-Format:", Locale.defaultShortDateFormat);
      locale._longDateFormat = header.GetValue("X-Long-Date-Format", Locale.defaultLongDateFormat);
      locale._shortTimeFormat = header.GetValue("X-Short-Time-Format", Locale.defaultShortTimeFormat);
      locale._longTimeFormat = header.GetValue("X-Long-Time-Format", Locale.defaultLongTimeFormat);
    }
  }
}