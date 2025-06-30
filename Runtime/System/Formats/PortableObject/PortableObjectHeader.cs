using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  // Class that defines the header for a Portable Object file
  internal sealed class PortableObjectHeader : IPortableObjectMessageFormat, IReadOnlyDictionary<string, string>
  {
    // Portable object header variables
    private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

    /// <summary>
    /// Return the keys of the headers.
    /// </summary>
    public IEnumerable<string> Keys => _headers.Keys;

    /// <summary>
    /// Return the values of the headers.
    /// </summary>
    public IEnumerable<string> Values => _headers.Values;

    /// <summary>
    /// Return the amount of entries of the headers.
    /// </summary>
    public int Count => _headers.Count;


    /// <summary>
    /// Return the value of the header for the specified key.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <returns>The value for the specified key, or null if the key does not exist.</param>
    public string this[string key] => GetValue(key, null);

    /// <summary>
    /// Return and set the Language header.
    /// </summary>
    public string language {
      get => GetValue("Language", null);
      set => Add("Language", value);
    }

    /// <summary>
    /// Return and set the Content-Type header.
    /// </summary>
    public string contentType {
      get => GetValue("Content-Type", null);
      set => Add("Content-Type", value);
    }

    /// <summary>
    /// Return and set the Content-Transfer-Encoding header.
    /// </summary>
    public string contentTransferEncoding {
      get => GetValue("Content-Transfer-Encoding", null);
      set => Add("Content-Transfer-Encoding", value);
    }


    /// <summary>
    /// Return if an header with the specified key can be found.
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <param name="value">The value of the header that will be stored.</param>
    /// <returns>Whether the key exists in the headers.</returns>
    public bool TryGetValue(string key, out string value)
    {
      return _headers.TryGetValue(key, out value);
    }

    /// <summary>
    /// Return the value of the header for the specified key, or the default value if the key does not exist.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>The value for the specified key, or the default value if the key does not exist.</param>
    public string GetValue(string key, string defaultValue = null)
    {
      return TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Return if a header with the specified key can be found.
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <returns>Whether the key exists in the headers.</returns>
    public bool ContainsKey(string key)
    {
      return TryGetValue(key, out _);
    }

    /// <summary>
    /// Add a header with the specified key and value to the headers.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    public void Add(string key, string value)
    {
      _headers[key] = value;
    }

    /// <summary>
    /// Remove the header with the specified key from the headers.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>Whether the header was succesfully removed.</returns>
    public bool Remove(string key)
    {
      return _headers.Remove(key);
    }

    /// <summary>
    /// Return a generic enumerator.
    /// </summary>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return _headers.GetEnumerator();
    }

    /// <summary>
    /// Return an enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }


    /// <summary>
    /// Encode the format to a message.
    /// </summary>
    /// <returns>The encoded message.</returns>
    public PortableObjectMessage EncodeMessage()
    {
      return new PortableObjectMessage {
        untranslatedString = string.Empty,
        translatedString = string.Join("", _headers.Select(e => $"{e.Key}: {e.Value}\n")),
      };
    }

    /// <summary>
    /// Decode the specified message to the format.
    /// </summary>
    /// <param name="message">The message to decode.</param>
    public void DecodeMessage(PortableObjectMessage message)
    {
      if (!string.IsNullOrEmpty(message.untranslatedString))
        throw new PortableObjectException("Invalid PO header identifier");

      _headers.Clear();
      foreach (var line in message.translatedString.Split("\n"))
      {
        var actualLine = line.Trim();
        if (string.IsNullOrEmpty(actualLine))
          continue;

        var index = -1;
        if ((index = actualLine.IndexOf(":")) < 0)
          throw new PortableObjectException($"Invalid PO header format in line \"line\"");

        var key = actualLine[..index].Trim();
        var value = actualLine[(index + 1)..].Trim();
        _headers[key] = value;
      }
    }
  }
}