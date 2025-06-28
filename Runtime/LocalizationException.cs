using System;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines an exception thrown while interacting with the localization system.
  /// </summary>
  public class LocalizationException : Exception
  {
    /// <summary>
    /// Constructor for a message
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    public LocalizationException(string message) : base(message) { }

    /// <summary>
    /// Constructor for a message and inner exception/
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    /// <param name="innerException">The exception that caused this exception to be thrown.</param>
    public LocalizationException(string message, Exception innerException) : base(message, innerException) { }
  }
}
