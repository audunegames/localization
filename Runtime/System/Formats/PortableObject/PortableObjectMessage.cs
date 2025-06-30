namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a message in a Portable Object file.
  /// </summary>
  internal class PortableObjectMessage
  {
    /// <summary>
    /// The context of the message.
    /// </summary>
    public string context;

    /// <summary>
    /// The untranslated string of the message.
    /// </summary>
    public string untranslatedString;

    /// <summary>
    /// The translated string of the message.
    /// </summary>
    public string translatedString;


    /// <summary>
    /// Return the unique key of the message.
    /// </summary>
    public string uniqueKey => !string.IsNullOrEmpty(context) ? $"{context}.{untranslatedString}" : untranslatedString;
  }
}