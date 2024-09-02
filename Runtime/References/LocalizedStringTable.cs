using Audune.Utils.Dictionary;
using Audune.Utils.UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a table of localized string values
  [Serializable]
  public sealed class LocalizedStringTable : ILocalizedStringTable
  {
    // Dictionary of entries in the table
    [SerializeField, SerializableDictionaryOptions(keyHeader = "String Table Key", listOptions = ReorderableListOptions.None)]
    private SerializableDictionary<string, string> _entries;


    // Constructor
    public LocalizedStringTable(IDictionary<string, string> entries = null)
    {
      _entries = entries != null ? new SerializableDictionary<string, string>(entries) : new SerializableDictionary<string, string>();
    }


    #region Localized string table implementation
    // Return the entries in the table
    public IReadOnlyDictionary<string, string> entries => _entries;


    // Return if an entry in the table with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out string value)
    {
      value = default;
      if (path == null)
        return false;

      return _entries.TryGetValue(path, out value);
    }

    // Return if an entry in the table with the specified path can be found and store the value of the entry using the specified string comparison type
    public bool TryFind(string path, out string value, StringComparison comparisonType)
    {
      value = default;
      if (path == null)
        return false;

      value = _entries.Where(e => e.Key.Equals(path, comparisonType)).SelectValue().FirstOrDefault();
      return value != default;
    }
    #endregion
  }
}