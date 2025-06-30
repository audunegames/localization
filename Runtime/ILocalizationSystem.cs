using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a localization system.
  /// </summary>
  public interface ILocalizationSystem : IMessageFunctionExecutor, ILocalizedStringFormatter
  {
    /// <summary>
    /// Return all registered locale loaders.
    /// </summary>
    public IEnumerable<LocaleLoader> loaders { get; }

    /// <summary>
    /// Return all enabled registered locale loaders.
    /// </summary>
    public IEnumerable<LocaleLoader> enabledLoaders { get; }

    /// <summary>
    /// Return all registered locale selectors.
    /// </summary>
    public IEnumerable<LocaleSelector> selectors { get; }

    /// <summary>
    /// Return all enabled registered locale selectors.
    /// </summary>
    public IEnumerable<LocaleSelector> enabledSelectors { get; }

    /// <summary>
    /// Return the loaded locales.
    /// </summary>
    public IReadOnlyList<ILocale> loadedLocales { get; }

    /// <summary>
    /// Return and set the selected locale.
    /// </summary>
    public ILocale selectedLocale { get; set; }

    /// <summary>
    /// Return and set the selected culture.
    /// </summary>
    public CultureInfo selectedCulture { get; set; }


    /// <summary>
    /// Event that is triggered when the selected locale is changed.
    /// </summary>
    public event Action<ILocale> onSelectedLocaleChanged;


    #region Loading and selecting locales
    /// <summary>
    /// Load the locales using the registered loaders.
    /// </summary>
    public void LoadLocales();
    
    /// <summary>
    /// Return if a locale can be selected using the registered selectors and store the selected locale.
    /// </summary>
    /// <returns>Whether a locale could be selected using the registered selectors.</returns>
    public bool TrySelectLocale();
    #endregion

    #region Formatting messages
    /// <summary>
    /// Format the specified message using the selected locale.
    /// </summary>
    /// <param name="message">The message to format using the selected locale.</param>
    /// <param name="arguments">The arguments to format inside of the message.</param>
    /// <returns>The formatted message, using the selected locale.</returns>
    /// <exception cref="LocalizationException">Thrown when no locale has been selected yet.</exception>
    public string Format(string message, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return Format(selectedLocale, message, arguments);
    }

    /// <summary>
    /// Format a localized string using the selected locale.
    /// </summary>
    /// <param name="reference">The localized string to format using the selected locale.</param>
    /// <returns>The formatted localized string, using the selected locale.</returns>
    /// <exception cref="LocalizationException">Thrown when no locale has been selected yet.</exception>
    public string Format(ILocalizedString reference)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return Format(selectedLocale, reference);
    }

    /// <summary>
    /// Format the contents of a text asset with the specified arguments using the selected locale.
    /// </summary>
    /// <param name="path">The path of the text asset to format using the selected locale.</param>
    /// <param name="arguments">The arguments to format inside of the text asset.</param>
    /// <returns>The formatted text asset, using the selected locale.</returns>
    /// <exception cref="LocalizationException">Thrown when no locale has been selected yet, or when the specified text asset could not be found.</exception>
    public string FormatAsset(string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return FormatAsset(selectedLocale, path, arguments);
    }

    /// <summary>
    /// Format the contents of a file with the specified arguments using the selected locale.
    /// </summary>
    /// <param name="path">The path of the file to format using the selected locale.</param>
    /// <param name="encoding">The encoding of the file to format using the selected locale.</param>
    /// <param name="arguments">The arguments to format inside of the file.</param>
    /// <returns>The formatted file, using the selected locale.</returns>
    /// <exception cref="LocalizationException">Thrown when no locale has been selected yet, or when the specified file could not be read.</exception>
    public string FormatFile(string path, Encoding encoding, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      return FormatFile(selectedLocale, path, encoding, arguments);
    }
    #endregion
  }
}