using System;

namespace Audune.Localization
{
  /// <summary>
  /// Exception that is thrown while formatting a message.
  /// </summary>
  public sealed class MessageException : LocalizationException
  {
    /// <summary>
    /// Constructor for a message
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    public MessageException(string message) : base(message) { }

    /// <summary>
    /// Constructor for a message and inner exception/
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    /// <param name="innerException">The exception that caused this exception to be thrown.</param>
    public MessageException(string message, Exception innerException) : base(message, innerException) { }
  }
}
