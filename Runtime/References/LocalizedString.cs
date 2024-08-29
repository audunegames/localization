using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a localized string reference
  [Serializable]
  public class LocalizedString : ILocalizedReference<string>, IEquatable<LocalizedString>
  {
    // Localized string properties
    [SerializeField, Tooltip("The path of the reference, in case the reference is a localized reference")]
    private string _path;
    [SerializeField, Tooltip("The value of the reference, in case the reference is a non-localized reference")]
    private string _value;

    // Internal state of the localized string
    private SerializableDictionary<string, object> _arguments;
    private Func<string, string> _formatter;


    // Return the path of the localized string
    public string path => _path;

    // Return the value of the localized string
    public string value => _value;

    // Return the arguments of the localized string
    public IReadOnlyDictionary<string, object> arguments => _arguments;

    // Return if the string is empty
    public bool isEmpty => string.IsNullOrEmpty(_path) && string.IsNullOrEmpty(_value);

    // Return if the string is localized
    public bool isLocalized => !string.IsNullOrEmpty(_path);


    // Private constructor
    private LocalizedString()
    {
      _path = null;
      _value = null;
      _arguments = new SerializableDictionary<string, object>();
      _formatter = s => s;
    }

    // Private constructor that copies the values from another localized string
    private LocalizedString(LocalizedString localizedString)
    {
      _path = localizedString._path;
      _value = localizedString._value;
      _arguments = new SerializableDictionary<string, object>(localizedString._arguments);
      _formatter = localizedString._formatter;
    }


    // Return if the reference can be resolved
    public bool TryResolve(ILocalizedTable<string> table, out string value)
    {
      if (!string.IsNullOrEmpty(_path))
      {
        var success = table.TryFind(_path, out value);
        value = success ? (_formatter?.Invoke(value) ?? value) : value;
        return success;
      }

      value = _formatter?.Invoke(_value) ?? _value;
      return true;
    }


    // Return the string representation of the localized string
    public override string ToString()
    {
      var builder = new StringBuilder();
      if (isLocalized)
        builder.Append(_path);
      else
        builder.Append($"<Non-Localized Value: \"{_value}\">");
      if (_arguments.Count > 0)
        builder.Append($" with arguments {{{string.Join(", ", _arguments.Select(e => $"{e.Key} = {e.Value}"))}}}");
      return builder.ToString();
    }

    // Return the message representation of the localized string
    public string ToMessageString()
    {
      return isLocalized ? $"{{={_path}}}" : _value;
    }


    #region Field management
    // Return a new localized string with the specified path
    public LocalizedString WithPath(string path)
    {
      var newReference = new LocalizedString(this);
      newReference._path = path;
      newReference._value = null;
      return newReference;
    }

    // Return a new localized string with the specified value
    public LocalizedString WithValue(string value)
    {
      var newReference = new LocalizedString(this);
      newReference._path = null;
      newReference._value = value;
      return newReference;
    }

    // Return a new localized string with the specified argument
    public LocalizedString WithArgument(string key, object value)
    {
      var newReference = new LocalizedString(this);
      newReference._arguments[key] = value;
      return newReference;
    }

    // Return a new localized string with the specified arguments
    public LocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      var newReference = new LocalizedString(this);
      foreach (var e in arguments)
        newReference._arguments[e.Key] = e.Value;
      return newReference;
    }

    // Return a new localized string with the specified arguments from the arguments interface
    public LocalizedString WithArguments(ILocalizedArguments arguments)
    {
      return WithArguments(arguments.localizedArguments);
    }

    // Return a new localized string without the specified argument
    public LocalizedString WithoutArgument(string key)
    {
      var newReference = new LocalizedString(this);
      newReference._arguments.Remove(key);
      return newReference;
    }

    // Return a new localized string without the specified arguments
    public LocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      var newReference = new LocalizedString(this);
      foreach (var key in keys)
        newReference._arguments.Remove(key);
      return newReference;
    }

    // Return a new localized string without the specified arguments from the arguments interface
    public LocalizedString WithoutArguments(ILocalizedArguments arguments)
    {
      return WithoutArguments(arguments.localizedArguments.Keys);
    }

    // Return a new localized string with the specified formatter
    public LocalizedString WithFormatter(Func<string, string> formatter)
    {
      var newReference = new LocalizedString(this);
      newReference._formatter = formatter;
      return newReference;
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
      return other is not null && _path == other._path && _value == other._value && EqualityComparer<SerializableDictionary<string, object>>.Default.Equals(_arguments, other._arguments);
    }

    // Return the hash code of the localized string
    public override int GetHashCode()
    {
      return HashCode.Combine(_path, _value, _arguments);
    }
    #endregion

    #region Creating localized strings
    // Create a localized string from a path
    public static LocalizedString FromPath(string path)
    {
      return new LocalizedString().WithPath(path);
    }

    // Create a localized string from a value
    public static LocalizedString FromValue(string value)
    {
      return new LocalizedString().WithValue(value);
    }
    #endregion

    #region Concatenation
    // Return the concatenation of two localized strings
    public static LocalizedString Concat(LocalizedString left, LocalizedString right)
    {
      return FromValue(left.ToMessageString() + right.ToMessageString());
    }

    // Return the join of multiple localized string
    public static LocalizedString Join(LocalizedString separator, IEnumerable<LocalizedString> strings)
    {
      return FromValue(string.Join(separator.ToMessageString(), strings.Select(s => s.ToMessageString())));
    }


    // Return the concatenation of two localized strings
    public static LocalizedString operator +(LocalizedString left, LocalizedString right)
    {
      return Concat(left, right);
    }
    #endregion

    #region Equality operators
    // Return if the localized string equals another localized string
    public static bool operator ==(LocalizedString left, LocalizedString right)
    {
      return EqualityComparer<LocalizedString>.Default.Equals(left, right);
    }

    // Return if the localized string does not equal another localized string
    public static bool operator !=(LocalizedString left, LocalizedString right)
    {
      return !(left == right);
    }
    #endregion

    #region Implicit operators
    // Convert a value to a non-localized string
    public static implicit operator LocalizedString(string value)
    {
      return new LocalizedString().WithValue(value);
    }
    #endregion
  }
}
