using System;
using System.Linq;

namespace Audune.Localization
{
  /// <summary>
  /// Enum that defines the plural keywords.
  /// </summary>
  public enum PluralKeyword
  {
    Zero,
    One,
    Two,
    Few,
    Many,
    Other
  }


  /// <summary>
  /// Class that defines extension methods for a plural keyword.
  /// </summary>
  public static class PluralKeywordExtensions
  {
    /// <summary>
    /// Convert a plural keyword to a keyword string.
    /// </summary>
    /// <param name="keyword">The plural keyword to convert.</param>
    /// <returns>The keyword string corresponding to the plural keyword.</returns>
    public static string ToKeywordString(this PluralKeyword keyword)
    {
      return Enum.GetName(typeof(PluralKeyword), keyword).ToLower();
    }

    /// <summary>
    /// Return if a keyword string can be parsed to a plural keyword and store the keyword.
    /// </summary>
    /// <param name="keywordString">The keyword string to parse.</param>
    /// <param name="keyword">The parsed plural keyword corresponding to the specified keyword string.</param>
    /// <returns>If the specified keyword string could be parsed to a plural keyword.</returns>
    public static bool TryParseKeywordString(string keywordString, out PluralKeyword keyword)
    {
      try
      {
        keyword = ((PluralKeyword[])Enum.GetValues(typeof(PluralKeyword))).Where(k => keywordString == k.ToKeywordString()).First();
        return true;
      }
      catch (InvalidOperationException)
      {
        keyword = PluralKeyword.Other;
        return false;
      }
    }
  }
}