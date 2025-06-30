using System.Globalization;
using System;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines extensions for the CultureInfo class.
  /// </summary>
  internal static class CultureInfoExtensions
  {
    /// <summary>
    /// Return the culture info with the specified name, or the invariant culture if the name does not exist.
    /// </summary>
    /// <param name="name">The name of the culture to find.</param>
    /// <returns>The culture info with the specified name, or the invariant culture if the name does not exist.</returns>
    public static CultureInfo GetCultureInfoOrInvariant(string name)
    {
      try
      {
        return CultureInfo.GetCultureInfo(name);
      }
      catch (Exception ex) when (ex is CultureNotFoundException || ex is ArgumentException)
      {
        return CultureInfo.InvariantCulture;
      }
    }
  }
}