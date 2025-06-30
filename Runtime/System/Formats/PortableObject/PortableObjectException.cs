using System;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines an exception thrown while interacting with Portable Object files.
  /// </summary>
  internal class PortableObjectException : LocalizationException
  {
    /// <summary>
    /// Constructor for a message
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    public PortableObjectException(string message) : base(message) { }

    /// <summary>
    /// Constructor for a message and inner exception/
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    /// <param name="innerException">The exception that caused this exception to be thrown.</param>
    public PortableObjectException(string message, Exception innerException) : base(message, innerException) { }
  }
}
