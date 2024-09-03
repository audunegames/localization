using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Audune.Localization
{
  // Class that defines a joined localized string
  internal class JoinedLocalizedString : ILocalizedString
  {
    // Internal state of the localized string list
    private readonly List<ILocalizedString> _strings;
    private readonly Dictionary<string, object> _arguments;


    // Constructor
    public JoinedLocalizedString(IEnumerable<ILocalizedString> strings = null, IEnumerable<KeyValuePair<string, object>> arguments = null)
    {
      _strings = strings != null ? new List<ILocalizedString>(strings) : new List<ILocalizedString>();
      _arguments = arguments != null ? new Dictionary<string, object>(arguments) : new Dictionary<string, object>();
    }


    // Return the string representation of the localized string
    public override string ToString()
    {
      return string.Join("", _strings);
    }


    #region Localized string implementation
    // Return the arguments of the localized string
    public IReadOnlyDictionary<string, object> arguments => _arguments;

    // Return if the localized string is not empty
    public bool isPresent => _strings.Count > 0;
    
    // Return if the localized string is localized
    public bool isLocalized => _strings.Any(s => s.isLocalized);


    // Return if the localized string can be resolved and store the value
    public bool TryResolve(ILocalizedStringTable table, out string value)
    {
      value = null;

      var builder = new StringBuilder();
      foreach (var item in _strings.Where(s => s != null))
      {
        if (item.TryResolve(table, out var itemValue))
          builder.Append(itemValue);
        else
          return false;
      }

      value = builder.ToString();
      return true;
    }

    // Return a new localized string with the specified argument
    public ILocalizedString WithArgument(string key, object value)
    {
      var newString = new JoinedLocalizedString(_strings, _arguments);
      newString._arguments[key] = value;
      return newString;
    }

    // Return a new localized string without the specified argument
    public ILocalizedString WithoutArgument(string key)
    {
      var newString = new JoinedLocalizedString(_strings, _arguments);
      newString._arguments.Remove(key);
      return newString;
    }
    
    // Return a new localized string with the arguments from the specified enumerable
    public ILocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      var newString = new JoinedLocalizedString(_strings, _arguments);
      foreach (var e in arguments)
        newString._arguments[e.Key] = e.Value;
      return newString;
    }

    // Return a new localized string without the arguments from the specified enumerable
    public ILocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      var newString = new JoinedLocalizedString(_strings, _arguments);
      foreach (var key in keys)
        newString._arguments.Remove(key);
      return newString;
    }
    #endregion
  }
}