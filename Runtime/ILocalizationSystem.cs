using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  // Interface that defines a localization system
  public interface ILocalizationSystem : IMessageFunctionExecutor
  {
    // Return all registered locale loaders
    public IEnumerable<LocaleLoader> loaders { get; }

    // Return all enabled registered locale loaders
    public IEnumerable<LocaleLoader> enabledLoaders { get; }

    // Return all registered locale selectors
    public IEnumerable<LocaleSelector> selectors { get; }

    // Return all enabled registered locale selectors
    public IEnumerable<LocaleSelector> enabledSelectors { get; }

    // Return the loaded locales
    public IReadOnlyList<Locale> loadedLocales { get; }

    // Return and set the selected locale
    public Locale selectedLocale { get; set; }

    // Return and set the selected culture
    public CultureInfo selectedCulture { get; set; }


    // Event that is triggered when the locale of the system has changed
    public event Action<Locale> onLocaleChanged;


    #region Loading and selecting locales
    // Load the locales using the registered loaders
    public void LoadLocales();
    
    // Return if a locale can be selected using the registered selectors and store the selected locale
    public bool TrySelectLocale();
    #endregion
    
    #region Managing message functions
    // Register a function with the specified name
    public void RegisterFunction(string name, Func<string, string> func);

    // Unregister a function with the specified name
    public void UnregisterFunction(string name);
    #endregion

    #region Formatting messages
    // Format a message with the specified arguments using the specified locale
    public string Format(Locale locale, string message, IReadOnlyDictionary<string, object> arguments = null);

    // Format a localized string using the specified locale
    public string Format(Locale locale, ILocalizedString reference)
    {
      if (reference == null)
        throw new ArgumentNullException(nameof(reference));

      if (!reference.TryResolve(locale.strings, out var message))
        throw new LocalizationException($"String \"{reference}\" could not be found in locale {locale}");

      return Format(message, reference.arguments);
    }

    // Format the contents of a text asset with the specified arguments using the specified locale
    public string FormatAsset(Locale locale, string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));
        
      var textAsset = Resources.Load<TextAsset>(path);
      if (textAsset == null)
        throw new LocalizationException("Could not find a text asset at \"{path}\"");

      return Format(locale, textAsset.text, arguments);
    }


    // Format the specified message using the selected locale
    public string Format(string message, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return Format(selectedLocale, message, arguments);
    }

    // Format a localized string using the selected locale
    public string Format(ILocalizedString reference)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return Format(selectedLocale, reference);
    }

    // Format the contents of a text asset with the specified arguments using the selected locale
    public string FormatAsset(string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return FormatAsset(selectedLocale, path, arguments);
    }
    #endregion
  }
}