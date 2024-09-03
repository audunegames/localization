using System;
using System.Linq;

namespace Audune.Localization
{
  // Enum that defines the plural keywords
  public enum PluralKeyword
  {
    Zero,
    One,
    Two,
    Few,
    Many,
    Other
  }


  // Class that defines extension methods for a plural keyword
  public static class PluralKeywordExtensions
  {
    // Convert a plural keyword to a keyword string
    public static string ToKeywordString(this PluralKeyword keyword)
    {
      return Enum.GetName(typeof(PluralKeyword), keyword).ToLower();
    }

    // Return if a keyword string can be parsed to a plural keyword and store the keyword
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