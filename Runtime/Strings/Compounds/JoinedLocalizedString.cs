using System.Collections.Generic;
using System.Linq;
using Audune.Utils.Dictionary;

namespace Audune.Localization
{
  // Class that defines a joined localized string
  internal class JoinedLocalizedString : ILocalizedString
  {
    // Internal state of the joined localized string
    private readonly List<ILocalizedString> _references;
    private readonly Dictionary<string, object> _arguments;


    // Constructor
    public JoinedLocalizedString(IEnumerable<ILocalizedString> references = null, IEnumerable<KeyValuePair<string, object>> arguments = null)
    {
      _references = references != null ? new List<ILocalizedString>(references) : new List<ILocalizedString>();
      _arguments = arguments != null ? new Dictionary<string, object>(arguments) : new Dictionary<string, object>();
    }


    // Return the string representation of the localized string
    public override string ToString()
    {
      return string.Join("", _references);
    }


    #region Localized string implementation
    // Return the arguments of the localized string
    IReadOnlyDictionary<string, object> ILocalizedString.arguments => _arguments;

    // Return if the localized string is not empty
    bool ILocalizedString.isPresent => _references.Count > 0;
    
    // Return if the localized string is localized
    bool ILocalizedString.isLocalized => _references.Any(s => s.isLocalized);


    // Resolve the localized string
    LocalizedStringResolver ILocalizedString.Resolve(IMessageFormatProvider formatProvider, IReadOnlyDictionary<string, object> extraArguments)
    {
      var actualArguments = extraArguments != null ? _arguments.Merge(extraArguments, g => g.First()) : _arguments;
      var resolvers = _references.Select(reference => reference.Resolve(formatProvider, actualArguments));
      return formatter => resolvers.Select(resolver => resolver(formatter)).Concatenate();
    }
    
    /// <summary>
    /// Return a new localized string with the specified argument.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add for the specified key.</param>
    /// <returns>A localized string with the added argument.</returns>
    ILocalizedString ILocalizedString.WithArgument(string key, object value)
    {
      var newString = new JoinedLocalizedString(_references, _arguments);
      newString._arguments[key] = value;
      return newString;
    }

    /// <summary>
    /// Return a new localized string without the specified argument
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A localized string with the removed argument.</returns>
    ILocalizedString ILocalizedString.WithoutArgument(string key)
    {
      var newString = new JoinedLocalizedString(_references, _arguments);
      newString._arguments.Remove(key);
      return newString;
    }
    
    /// <summary>
    /// Return a new localized string with the arguments from the specified enumerable.
    /// </summary>
    /// <param name="arguments">The enumerable of arguments to add.</param>
    /// <returns>A localized string with the added arguments.</returns>
    ILocalizedString ILocalizedString.WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      var newString = new JoinedLocalizedString(_references, _arguments);
      foreach (var e in arguments)
        newString._arguments[e.Key] = e.Value;
      return newString;
    }

    /// <summary>
    /// Return a new localized string without the arguments from the specified enumerable.
    /// </summary>
    /// <param name="keys">The enumerable of keys to remove.</param>
    /// <returns>A localized string with the added arguments.</returns>
    ILocalizedString ILocalizedString.WithoutArguments(IEnumerable<string> keys)
    {
      var newString = new JoinedLocalizedString(_references, _arguments);
      foreach (var key in keys)
        newString._arguments.Remove(key);
      return newString;
    }
    #endregion
  }
}