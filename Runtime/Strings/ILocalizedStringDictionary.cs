using System.Collections.Generic;

namespace Audune.Localization
{
  // Interface that defines a localized string dictionary
  public interface ILocalizedStringDictionary<TKey>
  {
    // Return the keys of the dictionary
    public IReadOnlyList<TKey> keys { get; }

    // Return the number of items in the dictionary
    public int count { get; }


    // Return if a value exists in the dictionary for the specified key
    public bool TryGetValue(TKey key, out ILocalizedString value);

    
    // Return the value for the specified key, or null if no such key exists
    public ILocalizedString GetValue(TKey key, ILocalizedString defaultValue = null)
    {
      return TryGetValue(key, out var value) ? value : defaultValue;
    }
  }
}