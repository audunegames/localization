using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a dictionary of keys mapped to localized references
  [Serializable]
  public sealed class LocalizedReferenceDictionary<TValue, TKey>
  {
    // Class that defines an entry in the table
    [Serializable]
    public class Entry
    {
      public TKey key;
      public LocalizedReference<TValue> reference;

      public Entry(TKey key, LocalizedReference<TValue> reference)
      {
        this.key = key;
        this.reference = reference;
      }
    }


    // The list of dictionary entries
    [SerializeField, Tooltip("The list of entries in the table")]
    private List<Entry> _entries;


    // Return the value in the dictionary for the specified key
    public LocalizedReference<TValue> Get(TKey key, LocalizedReference<TValue> defaultReference = null)
    {
      var entry = _entries.Find(e => Equals(e.key, key));
      return entry != null ? entry.reference : defaultReference;
    }
  }
}
