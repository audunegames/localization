using System.Collections.Generic;

namespace Audune.Localization
{
  // Class that defines a formatted localized string
  internal class FormattedLocalizedString : ILocalizedString
  {
    // Internal state of the localized string list
    private readonly ILocalizedString _source;
    private readonly LocalizedStringFormatter _formatter;


    // Constructor
    public FormattedLocalizedString(ILocalizedString source, LocalizedStringFormatter formatter)
    {
      _source = source;
      _formatter = formatter;
    }


    #region Localized string implementation
    // Return the arguments of the localized string
    public IReadOnlyDictionary<string, object> arguments => _source.arguments;

    // Return if the localized string is not empty
    public bool isPresent => _source.isPresent;
    
    // Return if the localized string is localized
    public bool isLocalized => _source.isLocalized;


    // Return if the localized string can be resolved and store the value
    public bool TryResolve(ILocalizedStringTable table, out string value)
    {
      var success = _source.TryResolve(table, out value);
      value = success ? _formatter(value) : null;
      return success;
    }

    // Return a new localized string with the specified argument
    public ILocalizedString WithArgument(string key, object value)
    {
      return new FormattedLocalizedString(_source.WithArgument(key, value), _formatter);
    }

    // Return a new localized string without the specified argument
    public ILocalizedString WithoutArgument(string key)
    {
      return new FormattedLocalizedString(_source.WithoutArgument(key), _formatter);
    }
    
    // Return a new localized string with the arguments from the specified enumerable
    public ILocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      return new FormattedLocalizedString(_source.WithArguments(arguments), _formatter);
    }

    // Return a new localized string without the arguments from the specified enumerable
    public ILocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      return new FormattedLocalizedString(_source.WithoutArguments(keys), _formatter);
    }
    #endregion
  }
}