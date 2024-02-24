using System;

namespace Audune.Localization
{
  // Class that defines an exception thrown while interacting with the localization system
  public class LocalizationException : Exception
  {
    // Constructor
    public LocalizationException(string message) : base(message) { }
    public LocalizationException(string message, Exception innerException) : base(message, innerException) { }
  }
}
