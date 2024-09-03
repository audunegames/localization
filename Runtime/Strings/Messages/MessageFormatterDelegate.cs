using System.Collections.Generic;

namespace Audune.Localization
{
  // Delegate that defines a delegate that formats a message
  public delegate string MessageFormatterDelegate(string message, IReadOnlyDictionary<string, object> arguments = null);
}