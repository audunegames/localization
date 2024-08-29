using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Interface that defines a formatter for a message
  public interface IMessageFormatter
  {
    // Format a message with the specified arguments
    public string Format(string message, IReadOnlyDictionary<string, object> arguments = null);

    // Format the contents of a text asset with the specified arguments
    public string FormatAsset(string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));

      var textAsset = Resources.Load<TextAsset>(path);
      if (textAsset == null)
        throw new LocalizationException("Could not find a text asset at \"{path}\"");

      return Format(textAsset.text, arguments);
    }
  }
}