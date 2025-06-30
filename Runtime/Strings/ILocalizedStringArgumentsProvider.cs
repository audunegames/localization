using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Interface that provides arguments for a localized string.
  /// </summary>
  public interface ILocalizedStringArgumentsProvider
  {
    /// <summary>
    /// Return the arguments to apply to a localized string.
    /// </summary>
    public IReadOnlyDictionary<string, object> arguments { get; }
  }
}