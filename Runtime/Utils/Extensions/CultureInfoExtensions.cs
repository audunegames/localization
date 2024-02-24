using System.Globalization;
using System;

namespace Audune.Localization
{
  // Class that defines extensions for the CultureInfo class
  public static class CultureInfoExtensions
  {
    // Return the culture info from the specified name, or the invariant culture if failed
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