using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a message compatible with ICU MessageFormat.
  /// See https://unicode-org.github.io/icu/userguide/format_parse/messages/ for more information
  /// </summary>
  internal sealed class Message
  {
    // List of components of the message
    internal List<MessageComponent> _components;


    // Constructor from an enumerable of components
    public Message(IEnumerable<MessageComponent> components)
    {
      _components = new List<MessageComponent>(components);
    }

    // Constructor from a string
    public Message(string input)
    {
      _components = new List<MessageComponent>(MessageParser.Parse(input));
    }
  }
}