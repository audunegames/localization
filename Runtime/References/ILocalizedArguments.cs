using System.Collections.Generic;

namespace Audune.Localization
{
  // Interface that returns arguments for a localized string
  public interface ILocalizedArguments
  {
    // Return the arguments to apply to a localized string
    public IReadOnlyDictionary<string, object> localizedArguments { get; }
  }
}