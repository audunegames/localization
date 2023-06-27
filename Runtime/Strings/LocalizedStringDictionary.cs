using Audune.Utils.Collections;
using System;
using UnityEngine;

namespace Audune.Localization.Strings
{
  // Class that defines a dictionary of keys mapped to localized string references
  [Serializable]
  public sealed class LocalizedStringDictionary<TKey>
  {
    // The list of dictionary entries
    [SerializeField, Tooltip("The entries in the table"), SerializableDictionaryDrawerOptions(ReorderableListDrawOptions.DrawFoldout | ReorderableListDrawOptions.DrawInfoField)]
    private SerializableDictionary<TKey, LocalizedString> _entries;


    // Return if a value exists in the dictionary for the specified key
    public bool TryGet(TKey key, out LocalizedString value)
    {
      return _entries.TryGetValue(key, out value);
    }

    // Return the value in the dictionary for the specified key
    public LocalizedString Get(TKey key, LocalizedString defaultReference = null)
    {
      return _entries.TryGetValue(key, out var value) ? value : defaultReference;
    }
  }
}
