using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a localized string reference
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
    // Return the arguments of the localized string
    public IReadOnlyDictionary<string, object> arguments => _arguments;

    // Return if the localized string is not empty
    public bool isPresent => !string.IsNullOrEmpty(_path) || !string.IsNullOrEmpty(_value);

    // Return if the localized string is localized
    public bool isLocalized => !string.IsNullOrEmpty(_path);


    // Resolve the localized string
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

    // Return a new localized string with the specified argument
    public ILocalizedString WithArgument(string key, object value)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      newString._arguments[key] = value;
      return newString;
    }

    // Return a new localized string with the specified arguments
    public ILocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      foreach (var e in arguments)
        newString._arguments[e.Key] = e.Value;
      return newString;
    }

    // Return a new localized string with the arguments from the specified provider
    public ILocalizedString WithArguments(ILocalizedStringArgumentsProvider arguments)
    {
      return WithArguments(arguments.arguments);
    }

    // Return a new localized string without the specified argument
    public ILocalizedString WithoutArgument(string key)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      newString._arguments.Remove(key);
      return newString;
    }

    // Return a new localized string without the specified arguments
    public ILocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      var newString = new LocalizedString(_path, _value, _arguments);
      foreach (var key in keys)
        newString._arguments.Remove(key);
      return newString;
    }

    // Return a new localized string without the arguments from the specified provider
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
