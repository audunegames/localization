using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a dictionary of keys mapped to localized string references
  [Serializable]
  public sealed class LocalizedStringDictionary<TKey>
  {
    // The list of dictionary entries
    [SerializeField, Tooltip("The entries in the table")]
    private SerializableDictionary<TKey, LocalizedString> _entries;


    // Return the keys of the dictionary
    public IReadOnlyList<TKey> keys => _entries.Keys.ToList();


    // Return the value for the specified key, or null if no such key exists
    public LocalizedString Get(TKey key, LocalizedString defaultValue = null)
    {
      return _entries.TryGetValue(key, out var value) ? value : defaultValue;
    }

    // Return if a value exists in the dictionary for the specified key
    public bool TryGet(TKey key, out LocalizedString value)
    {
      return _entries.TryGetValue(key, out value);
    }
  }
}
