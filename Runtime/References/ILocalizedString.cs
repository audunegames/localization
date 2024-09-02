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


    // Return if the localized string can be resolved and store the value
    public bool TryResolve(ILocalizedStringTable table, out string value);


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

    #region Managing formatting
    // Return a new localized string with the value formatted with the specified function
    public ILocalizedString Format(LocalizedStringFormatter formatter)
    {
      return new FormattedLocalizedString(this, formatter);
    }
    #endregion


    #region Creating localized strings
    // Create a localized string from a path
    public static ILocalizedString Path(string path)
    {
      return new LocalizedString(path, null);
    }

    // Create a localized string from a value
    public static ILocalizedString Value(string value)
    {
      return new LocalizedString(null, value);
    }
    #endregion

    #region Compositing localized strings
    // Return a new localized string that joins the specified localized strings separated by the specified separator
    public static ILocalizedString Join(ILocalizedString separator, IEnumerable<ILocalizedString> strings)
    {
      var actualStrings = strings.Interleave(separator);

      var actualArguments = new Dictionary<string, object>();
      foreach (var e in strings.SelectMany(s => s.arguments))
      {
        if (!actualArguments.ContainsKey(e.Key))
          actualArguments[e.Key] = e.Value;
      }

      return new JoinedLocalizedString(actualStrings, actualArguments);
    }

    // Return a new localized string that concatenates the specified localized strings
    public static ILocalizedString Concat(IEnumerable<ILocalizedString> strings)
    {
      return Join(null, strings);
    }

    // Return a new localized string that concatenates the specified localized strings
    public static ILocalizedString Concat(params ILocalizedString[] strings)
    {
      return Concat(strings);
    }


    // Join an enumerable of localized strings using the * operator
    public static ILocalizedString operator *(IEnumerable<ILocalizedString> strings, ILocalizedString separator)
    {
      return Join(separator, strings);
    }

    // Concatenate two localized strings using the + operator
    public static ILocalizedString operator +(ILocalizedString left, ILocalizedString right)
    {
      return Concat(left, right);
    }
    #endregion
  }
}