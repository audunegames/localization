namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a format that can be formatted to a message in a Portable Object file.
  /// </summary>
  internal interface IPortableObjectMessageFormat
  {
    /// <summary>
    /// Encode the format to a message.
    /// </summary>
    /// <returns>The encoded message.</returns>
    public PortableObjectMessage EncodeMessage();

    /// <summary>
    /// Decode the specified message to the format.
    /// </summary>
    /// <param name="message">The message to decode.</param>
    public void DecodeMessage(PortableObjectMessage message);
  }
}