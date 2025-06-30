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

    /// <summary>
    /// Return a new localized string with the specified argument.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add for the specified key.</param>
    /// <returns>A localized string with the added argument.</returns>
    ILocalizedString ILocalizedString.WithArgument(string key, object value)
    {
      return new SelectorLocalizedString(_reference.WithArgument(key, value), _selector);
    }

    /// <summary>
    /// Return a new localized string without the specified argument
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A localized string with the removed argument.</returns>
    ILocalizedString ILocalizedString.WithoutArgument(string key)
    {
      return new SelectorLocalizedString(_reference.WithoutArgument(key), _selector);
    }
    
    /// <summary>
    /// Return a new localized string with the arguments from the specified enumerable.
    /// </summary>
    /// <param name="arguments">The enumerable of arguments to add.</param>
    /// <returns>A localized string with the added arguments.</returns>
    ILocalizedString ILocalizedString.WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      return new SelectorLocalizedString(_reference.WithArguments(arguments), _selector);
    }

    /// <summary>
    /// Return a new localized string without the arguments from the specified enumerable.
    /// </summary>
    /// <param name="keys">The enumerable of keys to remove.</param>
    /// <returns>A localized string with the added arguments.</returns>
    ILocalizedString ILocalizedString.WithoutArguments(IEnumerable<string> keys)
    {
      return new SelectorLocalizedString(_reference.WithoutArguments(keys), _selector);
    }
    #endregion
  }
}