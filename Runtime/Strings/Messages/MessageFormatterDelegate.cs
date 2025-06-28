using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Delegate that defines a delegate that formats a message.
  /// </summary>
  public delegate string MessageFormatterDelegate(string message, IReadOnlyDictionary<string, object> arguments = null);
}