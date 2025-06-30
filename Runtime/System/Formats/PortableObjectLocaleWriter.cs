using System.IO;

namespace Audune.Localization
{
  /// <summary>
  /// Class that writes a locale to the Portable Object format.
  /// </summary>
  public sealed class PortableObjectLocaleWriter : LocaleWriter
  {
    /// <summary>
    /// Set if the writer should write empty string values.
    /// </summary>
    public readonly bool writeEmptyValues;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="writeEmptyValues">Whether the writer should write empty string values.</param>
    public PortableObjectLocaleWriter(bool writeEmptyValues = false)
    {
      this.writeEmptyValues = writeEmptyValues;
    }


    /// <summary>
    /// Write a locale to a text writer.
    /// </summary>
    /// <param name="textWriter">The text writer to write the locale to.</param>
    public override void Write(Locale locale, TextWriter textWriter)
    {
      var writer = new PortableObjectWriter(textWriter);

      // Write the header
      var header = EncodeHeader(locale);
      writer.Write(header);

      // Write the strings
      foreach (var e in locale.strings.entries)
        writer.Write(new PortableObjectMessage() { untranslatedString = e.Key, translatedString = writeEmptyValues ? null : e.Value });
    }


    /// <summary>
    /// Encode the header for the specified locale.
    /// </summary>
    private PortableObjectHeader EncodeHeader(Locale locale)
    {
      // Encode the code and names
      var header = new PortableObjectHeader() {
        { "Language", locale._code },
        { "X-English-Name", locale._englishName },
        { "X-Native-Name", locale._nativeName }
      };

      // Encode the alt codes
      foreach (var e in locale._altCodes)
        header.Add($"X-Alt-Code-{e.Key}", e.Value);

      // Encode the number formats
      if (locale._decimalNumberFormat != Locale.defaultDecimalNumberFormat)
        header.Add("X-Decimal-Number-Format", locale._decimalNumberFormat);
      if (locale._percentNumberFormat != Locale.defaultPercentNumberFormat)
        header.Add("X-Percent-Number-Format", locale._percentNumberFormat);
      if (locale._currencyNumberFormat != Locale.defaultCurrencyNumberFormat)
        header.Add("X-Currency-Number-Format", locale._currencyNumberFormat);
      if (locale._shortDateFormat != Locale.defaultShortDateFormat)
        header.Add("X-Short-Date-Format", locale._shortDateFormat);
      if (locale._longDateFormat != Locale.defaultLongDateFormat)
        header.Add("X-Long-Date-Format", locale._longDateFormat);
      if (locale._shortTimeFormat != Locale.defaultShortTimeFormat)
        header.Add("X-Short-Time-Format", locale._shortTimeFormat);
      if (locale._longTimeFormat != Locale.defaultLongTimeFormat)
        header.Add("X-Long-Time-Format", locale._longTimeFormat);

      header.contentType = "text/plain; charset=utf-8";
      header.contentTransferEncoding = "8bit";

      return header;
    }
  }
}