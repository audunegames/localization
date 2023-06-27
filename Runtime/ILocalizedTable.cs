using System.Collections.Generic;

namespace Audune.Localization
{
  // Interface that defines a table of localized values
  public interface ILocalizedTable<TValue>
  {
    // Return if an entry with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out TValue value);

    // Return the value of the entry with the specified path, or a default value if it cannot be found
    public TValue Find(string path, TValue defaultValue = default);


    // Return if an entry in the strings table with the specified reference can be found
    public sealed bool TryFind(ILocalizedReference<TValue> reference, out TValue value)
    {
      return reference.TryResolve(this, out value);
    }

    // Return the value of the entry in the strings table with the specified reference, or a default value if it cannot be found
    public sealed TValue Find(ILocalizedReference<TValue> reference, TValue defaultValue = default)
    {
      return reference.Resolve(this, defaultValue);
    }
  }
}