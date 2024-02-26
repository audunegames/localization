using System.Collections.Generic;

namespace Audune.Localization
{
  // Class that defines an environment for a message formatter
  internal sealed class MessageEnvironment
  {
    // Internal state of the environment
    private Dictionary<string, object> _arguments;
    private NumberContext _number;


    // Constructor
    public MessageEnvironment()
    {
      _arguments = new Dictionary<string, object>();
      _number = null;
    }

    // Constructor that copies the values from another environment
    private MessageEnvironment(MessageEnvironment environment)
    {
      _arguments = environment._arguments;
      _number = environment._number;
    }


    // Return if the environment contains an argument with the specified key and store the value of the argument
    public bool TryGetArgument(string key, out object value) 
    { 
      return _arguments.TryGetValue(key, out value);
    }

    // Return if the environment contains a number and store the value of the number
    public bool TryGetNumber(out NumberContext number)
    {
      number = _number;
      return number != null;
    }


    // Return a new environment based on this one with the specified argument
    public MessageEnvironment WithArgument(string key, object value)
    {
      var newEnvironment = new MessageEnvironment(this);
      newEnvironment._arguments[key] = value;
      return newEnvironment;
    }

    // Return a new environment based on this one with the specified arguments
    public MessageEnvironment WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      var newEnvironment = new MessageEnvironment(this);
      foreach (var e in arguments)
        newEnvironment._arguments[e.Key] = e.Value;
      return newEnvironment;
    }

    // Return a new environment based on this one without the specified argument
    public MessageEnvironment WithoutArgument(string key)
    {
      var newEnvironment = new MessageEnvironment(this);
      newEnvironment._arguments.Remove(key);
      return newEnvironment;
    }

    // Return a new environment based on this one without the specified arguments
    public MessageEnvironment WithoutArgument(IEnumerable<string> keys)
    {
      var newEnvironment = new MessageEnvironment(this);
      foreach (var key in keys)
        newEnvironment._arguments.Remove(key);
      return newEnvironment;
    }

    // Return a new environment based on this one with the specified number
    public MessageEnvironment WithNumber(NumberContext number)
    {
      var newEnvironment = new MessageEnvironment(this);
      newEnvironment._number = number;
      return newEnvironment;
    }

    // Return a new environment based on this one without a number
    public MessageEnvironment WithoutNumber()
    {
      var newEnvironment = new MessageEnvironment(this);
      newEnvironment._number = null;
      return newEnvironment;
    }
  }
}