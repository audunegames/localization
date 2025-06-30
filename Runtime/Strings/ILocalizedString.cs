using System;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a localized string.
  /// </summary>
  public interface ILocalizedString
  {
    /// <summary>
    /// Return the arguments of the localized string.
    /// </summary>
    public IReadOnlyDictionary<string, object> arguments { get; }

    /// <summary>
    /// Return if the localized string is not empty.
    /// </summary>
    public bool isPresent { get; }

    /// <summary>
    /// Return if the localized string is empty.
    /// </summary>
    public bool isEmpty => !isPresent;

    /// <summary>
    /// Return if the localized string is localized.
    /// </summary>
    public bool isLocalized { get; }


    /// <summary>
    /// Resolve the localized string.
    /// </summary>
    /// <param name="formatProvider">The format provider to use.</param>
    /// <param name="extraArguments">The extra arguments to add.</param>
    /// <returns>The resolver to resolve the localized string with.</returns>
    public LocalizedStringResolver Resolve(IMessageFormatProvider formatProvider, IReadOnlyDictionary<string, object> extraArguments = null);


    #region Managing arguments
    /// <summary>
    /// Return a new localized string with the specified argument.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add for the specified key.</param>
    /// <returns>A localized string with the added argument.</returns>
    public ILocalizedString WithArgument(string key, object value);

    /// <summary>
    /// Return a new localized string without the specified argument
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A localized string with the removed argument.</returns>
    public ILocalizedString WithoutArgument(string key);
    

    /// <summary>
    /// Return a new localized string with the arguments from the specified enumerable.
    /// </summary>
    /// <param name="arguments">The enumerable of arguments to add.</param>
    /// <returns>A localized string with the added arguments.</returns>
    public ILocalizedString WithArguments(IEnumerable<KeyValuePair<string, object>> arguments)
    {
      return arguments.Aggregate(this, (localizedString, argument) => localizedString.WithArgument(argument.Key, argument.Value));
    }

    /// <summary>
    /// Return a new localized string without the arguments from the specified enumerable.
    /// </summary>
    /// <param name="keys">The enumerable of keys to remove.</param>
    /// <returns>A localized string with the added arguments.</returns>
    public ILocalizedString WithoutArguments(IEnumerable<string> keys)
    {
      return arguments.Aggregate(this, (localizedString, argument) => localizedString.WithArgument(argument.Key, argument.Value));
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

    #region Managing mapping values
    /// <summary>
    /// Return a new localized string with the value mapped with the specified function.
    /// </summary>
    /// <param name="selector">The selector the map the value of the localized string with.</param>
    /// <returns>The mapped localized string.</returns>
    public ILocalizedString Select(Func<string, string> selector)
    {
      return new SelectorLocalizedString(this, selector);
    }
    #endregion

    #region Creating localized strings
    /// <summary>
    /// Create an empty localized string.
    /// </summary>
    /// <returns>An empty localized string.</returns>
    public static ILocalizedString Empty()
    {
      return new LocalizedString(null, null);
    }

    /// <summary>
    /// Create a localized string from a path.
    /// </summary>
    /// <param name="path">The path to initialize the localized string with.</param>
    /// <returns>A localized string with the specified path.</returns>
    public static ILocalizedString Path(string path)
    {
      if (path == null)
        return null;

      return new LocalizedString(path, null);
    }

    /// <summary>
    /// Create a localized string from a value
    /// </summary>
    /// <param name="value">The value to initialize the localized string with.</param>
    /// <returns>A localized string with the specified value.</returns>
    public static ILocalizedString Value(object value)
    {
      if (value == null)
        return null;

      return new LocalizedString(null, value.ToString());
    }

    /// <summary>
    /// Create a localized string from a function message.
    /// </summary>
    /// <param name="name">The name of the function to initialize the localized string with.</param>
    /// <param name="argument">The argument of the function to initialize the localized string with.</param>
    /// <returns>A localized string with the specified function name and argument.</returns>
    public static ILocalizedString Function(string name, string argument = null)
    {
      var message = $"{{${name}{(!string.IsNullOrEmpty(argument) ? $": {argument}" : "")}}}";
      return Value(message);
    }

    /// <summary>
    /// Create a localized string from an asset function message.
    /// </summary>
    /// <param name="path">The asset path to initialize the localized string with.</param>
    /// <returns>A localized string with the specified asset path.</returns>
    public static ILocalizedString Asset(string path)
    {
      return Function("asset", path);
    }
    #endregion

    #region Compositing localized strings
    /// <summary>
    /// Return a new localized string that joins the specified localized strings separated by the specified separator.
    /// </summary>
    /// <param name="separator">The separator to insert between the localized strings.</param>
    /// <param name="strings">The localized strings to join.</param>
    /// <returns>The joined localized string.</returns>
    public static ILocalizedString Join(ILocalizedString separator, IEnumerable<ILocalizedString> strings)
    {
      return new JoinedLocalizedString(strings.Interleave(separator));
    }

    /// <summary>
    /// Return a new localized string that joins the specified localized strings separated by the specified separator string.
    /// </summary>
    /// <param name="separator">The separator to insert between the localized strings.</param>
    /// <param name="strings">The localized strings to join.</param>
    /// <returns>The joined localized string.</returns>
    public static ILocalizedString Join(string separator, IEnumerable<ILocalizedString> strings)
    {
      return Join(Value(separator), strings);
    }

    /// <summary>
    /// Return a new localized string that concatenates the specified localized strings.
    /// </summary>
    /// <param name="strings">The localized strings to concatenate.</param>
    /// <returns>The concatenated localized string.</returns>
    public static ILocalizedString Concat(IEnumerable<ILocalizedString> strings)
    {
      return Join((ILocalizedString)null, strings);
    }

    /// <summary>
    /// Return a new localized string that concatenates the specified localized strings.
    /// </summary>
    /// <param name="strings">The localized strings to concatenate.</param>
    /// <returns>The concatenated localized string.</returns>
    public static ILocalizedString Concat(params ILocalizedString[] strings)
    {
      return Concat((IEnumerable<ILocalizedString>)strings);
    }


    /// <summary>
    /// Concatenate the specified localized strings.
    /// </summary>
    /// <param name="left">The left localized string to concatenate.</param>
    /// <param name="right">The right localized string to concatenate.</param>
    /// <returns>The concatenated localized string.</returns>
    public static ILocalizedString operator +(ILocalizedString left, ILocalizedString right)
    {
      return Concat(left, right);
    }

    /// <summary>
    /// Concatenate the specified string with the specified localized string.
    /// </summary>
    /// <param name="left">The left string to concatenate.</param>
    /// <param name="right">The right localized string to concatenate.</param>
    /// <returns>The concatenated localized string.</returns>
    public static ILocalizedString operator +(string left, ILocalizedString right)
    {
      return Concat(Value(left), right);
    }

    /// <summary>
    /// Concatenate the specified localized string with the specified string.
    /// </summary>
    /// <param name="left">The left localized string to concatenate.</param>
    /// <param name="right">The right string to concatenate.</param>
    /// <returns>The concatenated localized string.</returns>
    public static ILocalizedString operator +(ILocalizedString left, string right)
    {
      return Concat(left, Value(right));
    }
    #endregion
  }
}