using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a formatter for a localized string.
  /// </summary>
  public interface ILocalizedStringFormatter
  {    
    /// <summary>
    /// Format a message with the specified arguments using the specified format provider.
    /// </summary>
    /// <param name="formatProvider">The format provider to format the message with.</param>
    /// <param name="message">The message to format using the format provider.</param>
    /// <param name="arguments">The arguments to format inside of the message.</param>
    /// <returns>The formatted message, using the specified format provider.</returns>
    public string Format(IMessageFormatProvider formatProvider, string message, IReadOnlyDictionary<string, object> arguments = null);


    /// <summary>
    /// Format a localized string using the specified format provider.
    /// </summary>
    /// <param name="formatProvider">The format provider to format the localized string with.</param>
    /// <param name="reference">The localized string to format using the format provider.</param>
    /// <returns>The formatted localized string, using the specified format provider.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="reference" /> is null.</exception>
    public string Format(IMessageFormatProvider formatProvider, ILocalizedString reference)
    {
      if (reference == null)
        throw new ArgumentNullException(nameof(reference));

      var resolver = reference.Resolve(formatProvider);
      return resolver((message, arguments) => Format(formatProvider, message, arguments));
    }

    /// <summary>
    /// Format a text asset with the specified arguments using the specified format provider.
    /// </summary>
    /// <param name="formatProvider">The format provider to format the text asset with.</param>
    /// <param name="path">The path of the text asset to format using the format provider.</param>
    /// <param name="arguments">The arguments to format inside of the text asset.</param>
    /// <returns>The formatted text asset, using the specified format provider.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path" /> is null.</exception>
    /// <exception cref="LocalizationException">Thrown when the specified text asset could not be found.</exception>
    public string FormatAsset(IMessageFormatProvider formatProvider, string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));
        
      var textAsset = Resources.Load<TextAsset>(path);
      if (textAsset == null)
        throw new LocalizationException("Could not find a text asset at \"{path}\"");

      return Format(formatProvider, textAsset.text, arguments);
    }

    /// <summary>
    /// Format a file with the specified arguments using the specified format provider.
    /// </summary>
    /// <param name="formatProvider">The format provider to format the file with.</param>
    /// <param name="path">The path of the file to format using the format provider.</param>
    /// <param name="encoding">The encoding of the file to format using the format provider.</param>
    /// <param name="arguments">The arguments to format inside of the file.</param>
    /// <returns>The formatted file, using the specified format provider.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path" /> is null.</exception>
    /// <exception cref="LocalizationException">Thrown when the specified file could not be read.</exception>
    public string FormatFile(IMessageFormatProvider formatProvider, string path, Encoding encoding, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));

      try
      {
        var text = File.ReadAllText(path, encoding);
        return Format(formatProvider, text, arguments);
      }
      catch (IOException ex)
      {
        throw new LocalizationException($"Could not read the file at \"{path}\": {ex.Message}", ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new LocalizationException($"Could not read the file at \"{path}\": {ex.Message}", ex);
      }
    }
  }
}