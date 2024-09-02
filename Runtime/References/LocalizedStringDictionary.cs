using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a dictionary of keys mapped to localized string references
  [Serializable]
  public sealed class LocalizedStringDictionary<TKey> : ILocalizedStringDictionary<TKey>
  {
    // The list of dictionary entries
    [SerializeField, Tooltip("The entries in the table")]
    private SerializableDictionary<TKey, LocalizedString> _entries;

  
    #region Localized string dictionary implementation
    // Return the keys of the dictionary
    public IReadOnlyList<TKey> keys => _entries.Keys.ToList();
    
    // Return the number of items in the dictionary
    public int count => _entries.Count;


    // Return if a value exists in the dictionary for the specified key
    public bool TryGetValue(TKey key, out ILocalizedString value)
    {
      var success = _entries.TryGetValue(key, out var localizedString);
      value = localizedString;
      return success;
    }
    #endregion
  }
}
