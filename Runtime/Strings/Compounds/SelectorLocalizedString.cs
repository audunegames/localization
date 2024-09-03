using System;
using System.Collections.Generic;

namespace Audune.Localization
{
  // Class that defines a localized string with a selector
  internal class SelectorLocalizedString : ILocalizedString
  {
    // Internal state of the selector localized string
    private readonly ILocalizedString _reference;
    private readonly Func<string, string> _selector;


    // Constructor
    public SelectorLocalizedString(ILocalizedString reference, Func<string, string> selector)
    {
      _reference = reference;
      _selector = selector;
    }


    // Return the string representation of the localized string
    public override string ToString()
    {
      return _reference.ToString();
    }


    #region Localized string implementation
    // Return the arguments of the localized string
    IReadOnlyDictionary<string, object> ILocalizedString.arguments => _reference.arguments;

    // Return if the localized string is not empty
    bool ILocalizedString.isPresent => _reference.isPresent;
    
    // Return if the localized string is localized
    bool ILocalizedString.isLocalized => _reference.isLocalized;


    // Resolve the localized string
    LocalizedStringResolver ILocalizedString.Resolve(IMessageFormatProvider formatProvider, IReadOnlyDictionary<string, object> extraArguments)
    {
      var resolver = _reference.Resolve(formatProvider, extraArguments);
      return formatter => _selector(resolver(formatter));
    }

    // Return a new localized string with the specified argument
    ILocalizedString ILocalizedString.WithArgument(string key, object value)
    {
      return new SelectorLocalizedString(_reference.WithArgument(key, value), _selector);
    }

    // Return a new localized string without the specified argument
    ILocalizedString ILocalizedString.WithoutArgument(string key)
    {
      return new SelectorLocalizedString(_reference.WithoutArgument(key), _selector);
    }
    
    // Return a new localized string with the arguments from the specified enumerable
    ILocalizedString ILocalizedString.WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      return new SelectorLocalizedString(_reference.WithArguments(arguments), _selector);
    }

    // Return a new localized string without the arguments from the specified enumerable
    ILocalizedString ILocalizedString.WithoutArguments(IEnumerable<string> keys)
    {
      return new SelectorLocalizedString(_reference.WithoutArguments(keys), _selector);
    }
    #endregion
  }
}