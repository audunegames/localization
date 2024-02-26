using System.Collections.Generic;

namespace Audune.Localization
{
  // Interface that defines a formatter for a message
  public interface IMessageFormatter
  {
    // Format a message with the specified arguments
    public string Format(string message, IReadOnlyDictionary<string, object> arguments);
  }
}