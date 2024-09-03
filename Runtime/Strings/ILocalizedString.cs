using System;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  // Interface that defines a localized string
  public interface ILocalizedString
  {
    // Return the arguments of the localized string
    public IReadOnlyDictionary<string, object> arguments { get; }

    // Return if the localized string is not empty
    public bool isPresent { get; }

    // Return if the localized string is empty
    public bool isEmpty => !isPresent;

    // Return if the localized string is localized
    public bool isLocalized { get;}


    // Resolve the localized string
    public LocalizedStringResolver Resolve(IMessageFormatProvider formatProvider, IReadOnlyDictionary<string, object> extraArguments = null);


    #region Managing arguments
    // Return a new localized string with the specified argument
    public ILocalizedString WithArgument(string key, object value);

    // Return a new localized string without the specified argument
    public ILocalizedString WithoutArgument(string key);
    

    // Return a new localized string with the arguments from the specified enumerable
    public ILocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      return arguments.Aggregate(this, (localizedString, argument) => localizedString.WithArgument(argument.Key, argument.Value));
    }

    // Return a new localized string without the arguments from the specified enumerable
    public ILocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      return arguments.Aggregate(this, (localizedString, argument) => localizedString.WithArgument(argument.Key, argument.Value));
    }

    // Return a new localized string with the arguments from the specified provider
    public ILocalizedString WithArguments(ILocalizedStringArgumentsProvider arguments)
    {
      return WithArguments(arguments.arguments);
    }

    // Return a new localized string without the arguments from the specified provider
    public ILocalizedString WithoutArguments(ILocalizedStringArgumentsProvider arguments)
    {
      return WithoutArguments(arguments.arguments.Keys);
    }
    #endregion

    #region Managing mapping values
    // Return a new localized string with the value mapped with the specified function
    public ILocalizedString Select(Func<string, string> selector)
    {
      return new SelectorLocalizedString(this, selector);
    }
    #endregion

    #region Creating localized strings
    // Create an empty localized string
    public static ILocalizedString Empty()
    {
      return new LocalizedString(null, null);
    }

    // Create a localized string from a path
    public static ILocalizedString Path(string path)
    {
      if (path == null)
        return null;

      return new LocalizedString(path, null);
    }

    // Create a localized string from a value
    public static ILocalizedString Value(string value)
    {
      if (value == null)
        return null;

      return new LocalizedString(null, value);
    }

    // Create a localized string from a function message
    public static ILocalizedString Function(string name, string argument = null)
    {
      var message = $"{{${name}{(!string.IsNullOrEmpty(argument) ? $": {argument}" : "")}}}";
      return Value(message);
    }

    // Create a localized string from an asset function message
    public static ILocalizedString Asset(string path)
    {
      return Function("asset", path);
    }
    #endregion

    #region Compositing localized strings
    // Return a new localized string that joins the specified localized strings separated by the specified separator
    public static ILocalizedString Join(ILocalizedString separator, IEnumerable<ILocalizedString> strings)
    {
      return new JoinedLocalizedString(strings.Interleave(separator));
    }

    // Return a new localized string that joins the specified localized strings separated by the specified separator string
    public static ILocalizedString Join(string separator, IEnumerable<ILocalizedString> strings)
    {
      return Join(Value(separator), strings);
    }

    // Return a new localized string that concatenates the specified localized strings
    public static ILocalizedString Concat(IEnumerable<ILocalizedString> strings)
    {
      return Join((ILocalizedString)null, strings);
    }

    // Return a new localized string that concatenates the specified localized strings
    public static ILocalizedString Concat(params ILocalizedString[] strings)
    {
      return Concat((IEnumerable<ILocalizedString>)strings);
    }


    // Concatenate two localized strings using the + operator
    public static ILocalizedString operator +(ILocalizedString left, ILocalizedString right)
    {
      return Concat(left, right);
    }

    // Concatenate a localized string and a strings using the + operator
    public static ILocalizedString operator +(string left, ILocalizedString right)
    {
      return Concat(Value(left), right);
    }

    // Concatenate a string and a localized strings using the + operator
    public static ILocalizedString operator +(ILocalizedString left, string right)
    {
      return Concat(left, Value(right));
    }
    #endregion
  }
}