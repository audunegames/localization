using Audune.Utils.Dictionary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a component of a message
  internal abstract class MessageComponent
  {
    // Interface that defines a visitor for message components
    public interface IVisitor<TResult>
    {
      TResult VisitTextComponent(Text component);
      TResult VisitFormatComponent(Format component);
      TResult VisitNumberFormatComponent(NumberFormat component);
      TResult VisitDateFormatComponent(DateFormat component);
      TResult VisitPluralFormatComponent(PluralFormat component);
      TResult VisitSelectFormatComponent(SelectFormat component);
    }

    // Interface that defines a visitor for message components with a context
    public interface IVisitor<TResult, TContext>
    {
      TResult VisitTextComponent(Text component, TContext context);
      TResult VisitFormatComponent(Format component, TContext context);
      TResult VisitNumberFormatComponent(NumberFormat component, TContext context);
      TResult VisitDateFormatComponent(DateFormat component, TContext context);
      TResult VisitPluralFormatComponent(PluralFormat component, TContext context);
      TResult VisitSelectFormatComponent(SelectFormat component, TContext context);
    }


    // Accept a visitor on the component
    public abstract TResult Accept<TResult>(IVisitor<TResult> visitor);

    // Accept a visitor on the component with a context
    public abstract TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context);


    #region Text component
    // Class that defines a message component containing text
    public class Text : MessageComponent
    {
      // The text of the component
      public readonly string text;


      // Constructor
      public Text(string text)
      {
        this.text = text;
      }


      // Accept a visitor on the component
      public override TResult Accept<TResult>(IVisitor<TResult> visitor)
      {
        return visitor.VisitTextComponent(this);
      }

      // Accept a visitor on the component with a context
      public override TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.VisitTextComponent(this, context);
      }
    }
    #endregion

    #region Format component
    // Class that defines a message component containing a format argument
    public class Format : MessageComponent
    {
      // The name of the argument
      public readonly string name;


      // Constructor
      public Format(string name)
      {
        this.name = name;
      }


      // Accept a visitor on the component
      public override TResult Accept<TResult>(IVisitor<TResult> visitor)
      {
        return visitor.VisitFormatComponent(this);
      }

      // Accept a visitor on the component with a context
      public override TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.VisitFormatComponent(this, context);
      }
    }
    #endregion

    #region Number format component
    // Class that defines a message component containing a number format argument
    public class NumberFormat : Format
    {
      // The style of the argument
      public readonly NumberFormatStyle style;


      // Constructor
      public NumberFormat(string name, NumberFormatStyle style = NumberFormatStyle.Decimal) : base(name)
      {
        this.style = style;
      }


      // Accept a visitor on the component
      public override TResult Accept<TResult>(IVisitor<TResult> visitor)
      {
        return visitor.VisitNumberFormatComponent(this);
      }

      // Accept a visitor on the component with a context
      public override TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.VisitNumberFormatComponent(this, context);
      }
    }
    #endregion

    #region Date format component
    // Class that defines a message component containing a date format argument
    public class DateFormat : Format
    {
      // The type of the component
      public readonly DateFormatType type;

      // The style of the component
      public readonly DateFormatStyle style;


      // Constructor
      public DateFormat(string name, DateFormatType type, DateFormatStyle style = DateFormatStyle.Short) : base(name)
      {
        this.type = type;
        this.style = style;
      }


      // Accept a visitor on the component
      public override TResult Accept<TResult>(IVisitor<TResult> visitor)
      {
        return visitor.VisitDateFormatComponent(this);
      }

      // Accept a visitor on the component with a context
      public override TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.VisitDateFormatComponent(this, context);
      }
    }
    #endregion

    #region Plural format component
    // Class that defines a message component containing a plural format argument
    public class PluralFormat : Format
    {
      // Enum that defines the type of a plural format argument
      public enum Type
      {
        Plural,
        Ordinal,
      }


      // The type of a plural format argument
      public readonly Type type;

      // The branches of the component
      public readonly SortedDictionary<PluralSelector, Message> branches;

      // The offset of the component
      public readonly float offset;


      // Constructor
      public PluralFormat(string name, Type type, SortedDictionary<PluralSelector, Message> branches, float offset = 0.0f) : base(name)
      {
        this.type = type;
        this.branches = branches;
        this.offset = offset;
      }


      // Return a branch for the specified plural selector based on the specified number or keyword
      public bool TryGetBranch(NumberContext number, PluralKeyword keyword, out Message message)
      {
        message = branches.Where(e => e.Key.Matches(number, keyword)).SelectValue().FirstOrDefault();
        return message != null;
      }


      // Accept a visitor on the component
      public override TResult Accept<TResult>(IVisitor<TResult> visitor)
      {
        return visitor.VisitPluralFormatComponent(this);
      }

      // Accept a visitor on the component with a context
      public override TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.VisitPluralFormatComponent(this, context);
      }
    }
    #endregion

    #region Select format component
    // Class that defines a message component containing a select format argument
    public class SelectFormat : Format
    {
      // The branches of the component
      private readonly IReadOnlyDictionary<string, Message> branches;


      // Constructor
      public SelectFormat(string name, Dictionary<string, Message> branches) : base(name)
      {
        this.branches = branches;
      }


      // Return a branch for the specified select format based on the keyword
      public bool TryGetBranch(string keyword, out Message message)
      {
        return branches.TryGetValue(keyword, out message);
      }


      // Accept a visitor on the component
      public override TResult Accept<TResult>(IVisitor<TResult> visitor)
      {
        return visitor.VisitSelectFormatComponent(this);
      }

      // Accept a visitor on the component with a context
      public override TResult Accept<TResult, TContext>(IVisitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.VisitSelectFormatComponent(this, context);
      }
    }
    #endregion
  }
}