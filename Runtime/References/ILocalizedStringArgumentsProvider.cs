using System.Collections.Generic;

namespace Audune.Localization
{
  // Interface that provides arguments for a localized string
  public interface ILocalizedStringArgumentsProvider
  {
    // Return the arguments to apply to a localized string
    public IReadOnlyDictionary<string, object> arguments { get; }
  }
}