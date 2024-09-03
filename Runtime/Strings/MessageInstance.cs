using System.Collections.Generic;

namespace Audune.Localization
{
  // Class that defines an instance of a message with arguments
  public sealed class MessageInstance
  {
    // Reference to an empty message instance
    public static readonly MessageInstance empty = new MessageInstance(string.Empty, new Dictionary<string, object>());


    // The message of the instance
    public readonly string message;

    // The arguments of the instance
    public readonly IDictionary<string, object> arguments;


    // Constructor
    public MessageInstance(string message, IDictionary<string, object> arguments)
    {
      this.message = message;
      this.arguments = arguments;
    }
  }
}