using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Audune.Localization
{
  // Interface that defines a formatter for a localized string
  public interface ILocalizedStringFormatter
  {    
    // Format a message with the specified arguments using the specified format provider
    public string Format(IMessageFormatProvider formatProvider, string message, IReadOnlyDictionary<string, object> arguments = null);


    // Format a localized string using the specified format provider
    public string Format(IMessageFormatProvider formatProvider, ILocalizedString reference)
    {
      if (reference == null)
        throw new ArgumentNullException(nameof(reference));

      var resolver = reference.Resolve(formatProvider);
      return resolver((message, arguments) => Format(formatProvider, message, arguments));
    }

    // Format a text asset with the specified arguments using the specified format provider
    public string FormatAsset(IMessageFormatProvider formatProvider, string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));
        
      var textAsset = Resources.Load<TextAsset>(path);
      if (textAsset == null)
        throw new LocalizationException("Could not find a text asset at \"{path}\"");

      return Format(formatProvider, textAsset.text, arguments);
    }

    // Format a file with the specified arguments using the specified format provider
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