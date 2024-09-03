using System;

namespace Audune.Localization
{
  // Exception that is thrown while formatting a message
  public sealed class MessageException : LocalizationException
  {
    // Constructor
    public MessageException(string message) : base(message) { }
    public MessageException(string message, Exception innerException) : base(message, innerException) { }
  }
}
