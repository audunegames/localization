using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a localized string dictionary.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
  public interface ILocalizedStringDictionary<TKey>
  {
    /// <summary>
    /// Return the entries of the dictionary.
    /// </summary>
    public IReadOnlyDictionary<TKey, ILocalizedString> entries { get; }

    /// <summary>
    /// Return the keys of the dictionary.
    /// </summary>
    public IEnumerable<TKey> keys => entries.Keys;

    /// <summary>
    /// Return the number of items in the dictionary.
    /// </summary>
    public int count => entries.Count;

    /// <summary>
    /// Return the value of the entry in the dictionary for the specified key.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <returns>The value for the specified key, or null if the key does not exist.</param>
    public ILocalizedString this[TKey key] => GetValue(key, null);


    /// <summary>
    /// Return if a value exists in the dictionary for the specified key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <param name="value">The value in which the localized string will be stored when the key exists.</param>
    /// <returns>Whether the key exists in the dictionary.</returns>
    public bool TryGetValue(TKey key, out ILocalizedString value);

    
    /// <summary>
    /// Return the value of the entry in the dictionary for the specified key, or the default value if the key does not exist.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>The value for the specified key, or the default value if the key does not exist.</param>
    public ILocalizedString GetValue(TKey key, ILocalizedString defaultValue = null)
    {
      return TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Return if an entry with the specified key can be found in the dictionary.
    /// </summary>
    /// <param name="key">The key to find.</param>
    /// <returns>Whether the key exists in the dictionary.</returns>
    public bool Contains(TKey key)
    {
      return TryGetValue(key, out _);
    }
  }
}