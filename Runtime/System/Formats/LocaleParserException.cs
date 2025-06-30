using System;

namespace Audune.Localization
{
  // Class that defines an exception thrown while parsing a locale file
  public class LocaleParserException : LocalizationException
  {
    // Constructor
    public LocaleParserException(string message) : base(message) { }
    public LocaleParserException(string message, Exception innerException) : base(message, innerException) { }
  }
}
