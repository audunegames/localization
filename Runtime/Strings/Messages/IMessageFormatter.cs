using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a formatter for a message.
  /// </summary>
  public interface IMessageFormatter
  {
    /// <summary>
    /// Format a message with the specified arguments.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <param name="arguments">The arguments to format inside of the message.</param>
    /// <returns>The formatted message.</returns>
    public string Format(string message, IReadOnlyDictionary<string, object> arguments = null);
  }
}