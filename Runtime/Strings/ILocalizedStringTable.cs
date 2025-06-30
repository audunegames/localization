using System;
using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a table of localized strings.
  /// </summary>
  public interface ILocalizedStringTable
  {
    /// <summary>
    /// Return the entries in the table.
    /// </summary>
    public IReadOnlyDictionary<string, string> entries { get; }

    /// <summary>
    /// Return the keys in the table.
    /// </summary>
    public IEnumerable<string> keys => entries.Keys;

    /// <summary>
    /// Return the number of entries in the table.
    /// </summary>
    public int count => entries.Count;

    /// <summary>
    /// Return the value of the entry in the table for the specified path.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <returns>The value for the specified key, or null if the key does not exist.</param>
    public string this[string key] => Find(key, null);

    
    /// <summary>
    /// Return if an entry with the specified path can be found and store the value of the entry.
    /// </summary>
    /// <param name="path">The path to find.</param>
    /// <param name="value">The value in which the string will be stored when the path exists.</param>
    /// <returns>Whether the path exists in the table.</returns>
    public bool TryFind(string path, out string value);

    /// <summary>
    /// Return if an entry in the table with the specified path can be found and store the value of the entry using the specified string comparison type.
    /// </summary>
    /// <param name="path">The path to find.</param>
    /// <param name="value">The value in which the string will be stored when the path exists.</param>
    /// <param name="comparisonType">The comparison type to use when finding the path.</param>
    /// <returns>Whether the path exists in the table.</returns>
    public bool TryFind(string path, out string value, StringComparison comparisonType);


    /// <summary>
    /// Return the value of the entry in the table for the specified path, or the default value if one cannot be found.
    /// </summary>
    /// <param name="path">The path to get the value for.</param>
    /// <param name="defaultValue">The default value to return if the path does not exist.</param>
    /// <returns>The value for the specified path, or the default value if the path does not exist.</param>
    public string Find(string path, string defaultValue)
    {
      return TryFind(path, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Return the value of the entry in the table for the specified path, or the default value if one cannot be found using the specified string comparison type.
    /// </summary>
    /// <param name="path">The path to get the value for.</param>
    /// <param name="defaultValue">The default value to return if the path does not exist.</param>
    /// <param name="comparisonType">The comparison type to use when finding the path.</param>
    /// <returns>The value for the specified path, or the default value if the path does not exist.</param>
    public string Find(string path, string defaultValue, StringComparison comparisonType = StringComparison.Ordinal)
    {
      return TryFind(path, out var value, comparisonType) ? value : defaultValue;
    }

    /// <summary>
    /// Return if an entry with the specified path can be found.
    /// </summary>
    /// <param name="path">The path to find.</param>
    /// <returns>Whether the path exists in the table.</returns>
    public bool Contains(string path)
    {
      return TryFind(path, out _);
    }

    /// <summary>
    /// Return if an entry in the table with the specified path can be found using the specified string comparison type.
    /// </summary>
    /// <param name="path">The path to find.</param>
    /// <param name="comparisonType">The comparison type to use when finding the path.</param>
    /// <returns>Whether the path exists in the table.</returns>
    public bool Contains(string path, StringComparison comparisonType)
    {
      return TryFind(path, out _, comparisonType);
    }
  }
}