using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Audune.Localization
{
  // Interface that defines a localization system
  public interface ILocalizationSystem : IMessageFunctionExecutor, ILocalizedStringFormatter
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

    #region Formatting messages
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

    // Format the contents of a file with the specified arguments using the selected locale
    public string FormatFile(string path, Encoding encoding, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return FormatFile(selectedLocale, path, encoding, arguments);
    }
    #endregion
  }
}