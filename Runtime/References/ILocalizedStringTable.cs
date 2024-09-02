using System;
using System.Collections.Generic;

namespace Audune.Localization
{
  // Interface that defines a table of localized strings
  public interface ILocalizedStringTable
  {
    // Return the entries in the table
    public IReadOnlyDictionary<string, string> entries { get; }

    // Return the keys in the table
    public IEnumerable<string> keys => entries.Keys;

    // Return the number of entries in the table
    public int count => entries.Count;

    
    // Return if an entry with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out string value);

    // Return if an entry in the table with the specified path can be found and store the value of the entry using the specified string comparison type
    public bool TryFind(string path, out string value, StringComparison comparisonType);


    // Return the value of the entry in the table with the specified path, or a default value if one cannot be found
    public string Find(string path, string defaultValue = default)
    {
      return TryFind(path, out var value) ? value : defaultValue;
    }

    // Return the value of the entry in the table with the specified path, or a default value if one cannot be found using the specified string comparison type
    public string Find(string path, string defaultValue = default, StringComparison comparisonType = StringComparison.Ordinal)
    {
      return TryFind(path, out var value, comparisonType) ? value : defaultValue;
    }

    // Return if an entry with the specified path can be found
    public bool Contains(string path)
    {
      return TryFind(path, out _);
    }

    // Return if an entry in the table with the specified path can be found using the specified string comparison type
    public bool Contains(string path, StringComparison comparisonType)
    {
      return TryFind(path, out _, comparisonType);
    }
  }
}