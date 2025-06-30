using System;

namespace Audune.Localization
{
  /// <summary>
  /// Enum that defines the checks to execute to select a locale in a system locale selector.
  /// </summary>
  [Flags]
  public enum SystemLocaleCheckType
  {
    None = 0,
    CultureInfo = 1 << 0,
    UnitySystemLanguage = 1 << 1,
  }
}