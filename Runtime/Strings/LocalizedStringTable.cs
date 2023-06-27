using Audune.Utils.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization.Strings
{
  // Class that defines a table of localized string values
  [Serializable]
  public sealed class LocalizedStringTable : ILocalizedTable<string>
  {
    // Dictionary of entries in the table
    [SerializeField, SerializableDictionaryDrawerOptions(ReorderableListDrawOptions.DrawFoldout | ReorderableListDrawOptions.DrawInfoField)]
    private SerializableDictionary<string, string> _entries;


    // Return the entries in the table
    public IReadOnlyDictionary<string, string> Entries => _entries;

    // Return the keys in the table
    public IEnumerable<string> Keys => _entries.Keys;


    // Constructor
    public LocalizedStringTable(IDictionary<string, string> entries = null)
    {
      _entries = entries != null ? new SerializableDictionary<string, string>(entries) : new SerializableDictionary<string, string>();
    }


    #region Localized table implementation
    // Return if an entry in the table with the specified path can be found
    public bool TryFind(string path, out string value)
    {
      value = default;
      if (path == null)
        return false;

      return _entries.TryGetValue(path, out value);
    }

    // Return the value of the entry in the table with the specified path, or a default value if one cannot be found
    public string Find(string path, string defaultValue = default)
    {
      return _entries.TryGetValue(path, out var value) ? value : defaultValue;
    }
    #endregion
  }
}