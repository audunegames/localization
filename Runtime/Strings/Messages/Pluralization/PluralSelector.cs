using System;
using System.Collections.Generic;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a selector for a plural format message component.
  /// </summary>
  internal abstract class PluralSelector : IComparable<PluralSelector>
  {
    // Return if the selector matches the specified number or keyword
    public abstract bool Matches(NumberContext number, PluralKeyword keyword);

    // Compare the selector to another selector
    public abstract int CompareTo(PluralSelector other);


    #region Explicit selector
    // Class that defines a plural selector that matches an explicit value
    public class Explicit : PluralSelector, IComparable<Explicit>
    {
      // The explicit value of the selector
      public readonly float value;


      // Constructor
      public Explicit(float value)
      {
        this.value = value;
      }


      // Return if the selector matches the specified number or keyword
      public override bool Matches(NumberContext number, PluralKeyword keyword)
      {
        return value == number.value;
      }

      // Compare the selector to another selector
      public override int CompareTo(PluralSelector other)
      {
        return other is Explicit otherExplicit ? CompareTo(otherExplicit) : -1;
      }

      // Compare the selector to another explicit selector
      public int CompareTo(Explicit other)
      {
        return Comparer<float>.Default.Compare(value, other.value);
      }
    }
    #endregion

    #region Keyword selector
    // Class that defines a plural selector that matches a keyword
    public class Keyword : PluralSelector, IComparable<Keyword>
    {
      // The keyword of the selector
      public readonly PluralKeyword keyword;


      // Constructor
      public Keyword(PluralKeyword keyword)
      {
        this.keyword = keyword;
      }


      // Return if the selector matches the specified number or keyword
      public override bool Matches(NumberContext number, PluralKeyword keyword)
      {
        return this.keyword == keyword;
      }

      // Compare the selector to another selector
      public override int CompareTo(PluralSelector other)
      {
        return other is Keyword otherKeyword ? CompareTo(otherKeyword) : 1;
      }

      // Compare the selector to another keyword selector
      public int CompareTo(Keyword other)
      {
        return Comparer<PluralKeyword>.Default.Compare(keyword, other.keyword);
      }
    }
    #endregion
  }
}