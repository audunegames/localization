using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a localized string reference.
  /// </summary>
  [Serializable]
  public class LocalizedString : ILocalizedString, IEquatable<LocalizedString>
  {
    // Localized string properties
    [SerializeField, Tooltip("The path of the string, in case the string is localized")]
    private string _path;
    [SerializeField, Tooltip("The value of the string, in case the string is non-localized")]
    private string _value;

    // Internal state of the localized string
    private Dictionary<string, object> _arguments;


    // Return the path of the localized string
    public string path => _path;

    // Return the value of the localized string
    public string value => _value;


    // Private constructor
    internal LocalizedString(string path, string value, IEnumerable<KeyValuePair<string, object>> arguments = null)
    {
      _path = path;
      _value = value;
      _arguments = arguments != null ? new Dictionary<string, object>(arguments) : new Dictionary<string, object>();
    }


    // Return the string representation of the localized string
    public override string ToString()
    {
      return isLocalized ? $"{{={_path}}}" : _value;
    }


    #region Localized string implementation
    /// <summary>
    /// Return the arguments of the localized string.
    /// </summary>
    public IReadOnlyDictionary<string, object> arguments => _arguments;

    /// <summary>
    /// Return if the localized string is not empty.
    /// </summary>
    public bool isPresent => !string.IsNullOrEmpty(_path) || !string.IsNullOrEmpty(_value);

    /// <summary>
    /// Return if the localized string is localized.
    /// </summary>
    public bool isLocalized => !string.IsNullOrEmpty(_path);


    /// <summary>
    /// Resolve the localized string.
    /// </summary>
    /// <param name="formatProvider">The format provider to use.</param>
    /// <param name="extraArguments">The extra arguments to add.</param>
    /// <returns>The resolver to resolve the localized string with.</returns>
    LocalizedStringResolver ILocalizedString.Resolve(IMessageFormatProvider formatProvider, IReadOnlyDictionary<string, object> extraArguments)
    {
      var actualArguments = _arguments != null ? (extraArguments != null ? _arguments.Merge(extraArguments, g => g.First()) : _arguments) : new Dictionary<string, object>();
      
      if (!string.IsNullOrEmpty(_path))
      {
        if (formatProvider.localizedStringTable.TryFind(_path, out var message))
          return formatter => formatter(message, actualArguments);
        else
          throw new LocalizationException($"String {_path} could not be found");
      }
      else if (!string.IsNullOrEmpty(_value))
      {
        return formatter => formatter(_value, actualArguments);
      }
      else
      {
        return formatter => string.Empty;
      }
    }

    /// <summary>
    /// Return a new localized string with the specified argument.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add for the specified key.</param>
    /// <returns>A localized string with the added argument.</returns>
    public ILocalizedString WithArgument(string key, object value)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      newString._arguments[key] = value;
      return newString;
    }

    /// <summary>
    /// Return a new localized string without the specified argument
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A localized string with the removed argument.</returns>
    public ILocalizedString WithoutArgument(string key)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      newString._arguments.Remove(key);
      return newString;
    }

    /// <summary>
    /// Return a new localized string with the arguments from the specified enumerable.
    /// </summary>
    /// <param name="arguments">The enumerable of arguments to add.</param>
    /// <returns>A localized string with the added arguments.</returns>
    public ILocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      foreach (var e in arguments)
        newString._arguments[e.Key] = e.Value;
      return newString;
    }

    /// <summary>
    /// Return a new localized string without the arguments from the specified enumerable.
    /// </summary>
    /// <param name="keys">The enumerable of keys to remove.</param>
    /// <returns>A localized string with the added arguments.</returns>
    public ILocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      foreach (var key in keys)
        newString._arguments.Remove(key);
      return newString;
    }

    /// <summary>
    /// Return a new localized string with the arguments from the specified provider.
    /// </summary>
    /// <param name="arguments">The arguments provides whose arguments to add.</param>
    /// <returns>A localized string with the added arguments.</returns>
    public ILocalizedString WithArguments(ILocalizedStringArgumentsProvider arguments)
    {
      return WithArguments(arguments.arguments);
    }

    /// <summary>
    /// Return a new localized string without the arguments from the specified provider.
    /// </summary>
    /// <param name="arguments">The arguments provides whose arguments to remove.</param>
    /// <returns>A localized string with the added arguments.</returns>
    public ILocalizedString WithoutArguments(ILocalizedStringArgumentsProvider arguments)
    {
      return WithoutArguments(arguments.arguments.Keys);
    }
    #endregion

    #region Equatable implementation
    // Return if the localized string equals another object
    public override bool Equals(object obj)
    {
      return Equals(obj as LocalizedString);
    }

    // Return if the localized string equals another localized string
    public bool Equals(LocalizedString other)
    {
      return other is not null && _path == other._path && _value == other._value && EqualityComparer<Dictionary<string, object>>.Default.Equals(_arguments, other._arguments);
    }

    // Return the hash code of the localized string
    public override int GetHashCode()
    {
      return HashCode.Combine(_path, _value, _arguments);
    }

    
    // Return if the localized string equals another localized string with the == operator
    public static bool operator ==(LocalizedString left, LocalizedString right)
    {
      return EqualityComparer<LocalizedString>.Default.Equals(left, right);
    }

    // Return if the localized string does not equal another localized string with the != operator
    public static bool operator !=(LocalizedString left, LocalizedString right)
    {
      return !(left == right);
    }
    #endregion
  }
}
