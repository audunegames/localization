using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Audune.Localization
{
  // Class that defines a parser for a message
  internal static class MessageParser
  {
    // Enum that defines the type of a nested context in the parser
    public enum ContextType
    {
      Plural,
      Select,
    }


    // Parse an enumerable of components
    public static IEnumerable<MessageComponent> Parse(string input)
    {
      var scanner = new Scanner(input);

      var components = ParseComponents(scanner, new Stack<ContextType>()).ToList();
      scanner.AssertAtEnd();

      return components;
    }


    #region Component parser functions
    // Parse an enumerable of components
    private static IEnumerable<MessageComponent> ParseComponents(Scanner scanner, Stack<ContextType> contexts)
    {
      var lastIndex = scanner.index;
      while (!scanner.atEnd && (contexts.Count == 0 || !scanner.Check('}')))
      {
        yield return ParseComponent(scanner, contexts);
        if (lastIndex == scanner.index)
          throw new FormatException($"Unxpected '{scanner.next}' at index {scanner.index}");
        else
          lastIndex = scanner.index;
      }
    }

    // Parse a component
    private static MessageComponent ParseComponent(Scanner scanner, Stack<ContextType> contexts)
    {
      if (scanner.Match('{'))
      {
        var argumentComponent = ParseArgumentComponent(scanner, contexts);

        scanner.Consume('}');

        return argumentComponent;
      }

      return ParseTextComponent(scanner, contexts);
    }

    // Parse a text component
    private static MessageComponent ParseTextComponent(Scanner scanner, Stack<ContextType> contexts)
    {
      var builder = new StringBuilder();
      var quoted = false;

      while (!scanner.atEnd)
      {
        if (quoted)
        {
          if (scanner.Match('\''))
          {
            if (scanner.Match('\''))
              builder.Append('\'');
            else
              quoted = false;
          }
          else
          {
            builder.Append(scanner.Advance());
          }
        }
        else
        {
          if (scanner.Match('\''))
          {
            if (scanner.Match('\''))
              builder.Append('\'');
            else if (scanner.Check(c => c == '{' || c == '}' || (contexts.TryPeek(out var context) && context == ContextType.Plural && c == '#')))
              quoted = true;
            else
              builder.Append('\'');
          }
          else
          {
            if (scanner.Check(c => c == '{' || c == '}'))
              break;
            else
              builder.Append(scanner.Advance());
          }
        }
      }

      return new MessageComponent.Text(builder.ToString());
    }

    // Parse an argument component
    private static MessageComponent ParseArgumentComponent(Scanner scanner, Stack<ContextType> contexts)
    {
      var name = ParseName(scanner);
      scanner.SkipWhile(Scanner.IsWhitespace);

      if (!scanner.Match(','))
        return new MessageComponent.Format(name);
      scanner.SkipWhile(Scanner.IsWhitespace);

      var typeKeyword = ParseKeyword(scanner);
      scanner.SkipWhile(Scanner.IsWhitespace);

      if (typeKeyword == "number")
        return new MessageComponent.NumberFormat(name, ParseNumberFormatStyle(scanner));
      else if (typeKeyword == "date")
        return new MessageComponent.DateFormat(name, DateFormatType.Date, ParseDateFormatStyle(scanner));
      else if (typeKeyword == "time")
        return new MessageComponent.DateFormat(name, DateFormatType.Date, ParseDateFormatStyle(scanner));
      else if (typeKeyword == "plural")
        return ParsePluralFormatComponent(scanner, contexts, name);
      else if (typeKeyword == "select")
        return ParseSelectFormatComponent(scanner, contexts, name);
      else
        throw new FormatException($"Argument type \"{typeKeyword}\" is unsupported");
    }

    // Parse a plural format component
    private static MessageComponent ParsePluralFormatComponent(Scanner scanner, Stack<ContextType> contexts, string name)
    {
      var offset = 0.0f;

      scanner.Consume(',');
      scanner.SkipWhile(Scanner.IsWhitespace);

      if (scanner.Match("offset:"))
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        offset = ParseNumber(scanner);
      }

      var pluralBranches = new SortedDictionary<PluralSelector, Message>();
      do
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        var selector = ParsePluralSelector(scanner);
        scanner.SkipWhile(Scanner.IsWhitespace);

        scanner.Consume('{');

        contexts.Push(ContextType.Plural);
        var message = new Message(ParseComponents(scanner, contexts));
        contexts.Pop();

        scanner.Consume('}');

        pluralBranches.Add(selector, message);
      } while (!scanner.Check('}') && !scanner.atEnd);

      if (!pluralBranches.SelectKey().OfType<PluralSelector.Keyword>().Any(s => s.keyword == PluralKeyword.Other))
        throw new FormatException("Plural component is missing a default \"other\" argument");

      return new MessageComponent.PluralFormat(name, pluralBranches, offset);
    }

    // Parse a select component
    private static MessageComponent ParseSelectFormatComponent(Scanner scanner, Stack<ContextType> contexts, string name)
    {
      scanner.Consume(',');
      scanner.SkipWhile(Scanner.IsWhitespace);

      var selectBranches = new Dictionary<string, Message>();
      do
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        var keyword = ParseKeyword(scanner);
        scanner.SkipWhile(Scanner.IsWhitespace);

        scanner.Consume('{');

        contexts.Push(ContextType.Select);
        var message = new Message(ParseComponents(scanner, contexts));
        contexts.Pop();

        scanner.Consume('}');

        selectBranches.Add(keyword, message);
      } while (!scanner.Check('}') && !scanner.atEnd);

      if (!selectBranches.Any(e => e.Key == "other"))
        throw new FormatException("Select component is missing a default \"other\" argument");

      return new MessageComponent.SelectFormat(name, selectBranches);
    }
    #endregion

    #region Primitive parser functions
    // Parse a number
    private static float ParseNumber(Scanner scanner)
    {
      var builder = new StringBuilder();

      if (scanner.Match('+'))
        builder.Append(CultureInfo.InvariantCulture.NumberFormat.PositiveSign);
      else if (scanner.Match('-'))
        builder.Append(CultureInfo.InvariantCulture.NumberFormat.NegativeSign);

      if (scanner.Match('0'))
        return 0.0f;
      else
        builder.Append(scanner.ReadWhile(Scanner.IsNonZeroDigit, "non-zero digit", Scanner.IsDigit));

      var value = builder.ToString();
      if (!float.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var floatValue))
        throw new FormatException($"Number \"{value}\" has an invalid number format");

      return floatValue;
    }

    // Parse a name string
    private static string ParseName(Scanner scanner)
    {
      return scanner.ReadWhile(Scanner.IsLetterOrUnderscore, "letter or underscore", Scanner.IsLetterOrDigitOrUnderscore);
    }

    // Parse a keyword string
    private static string ParseKeyword(Scanner scanner)
    {
      return scanner.ReadWhile(Scanner.IsLowercaseLetter);
    }

    // Parse a number format style
    private static NumberFormatStyle ParseNumberFormatStyle(Scanner scanner)
    {
      if (!scanner.Match(','))
        return NumberFormatStyle.Decimal;
      scanner.SkipWhile(Scanner.IsWhitespace);

      var styleKeyword = ParseKeyword(scanner);
      if (styleKeyword == "decimal")
        return NumberFormatStyle.Decimal;
      else if (styleKeyword == "percent")
        return NumberFormatStyle.Percent;
      else if (styleKeyword == "currency")
        return NumberFormatStyle.Currency;
      else
        throw new FormatException($"Number format style \"{styleKeyword}\" is unsupported");
    }

    // Parse a date format style
    private static DateFormatStyle ParseDateFormatStyle(Scanner scanner)
    {
      if (!scanner.Match(','))
        return DateFormatStyle.Short;
      scanner.SkipWhile(Scanner.IsWhitespace);

      var styleKeyword = ParseKeyword(scanner);
      if (styleKeyword == "short")
        return DateFormatStyle.Short;
      else if (styleKeyword == "long")
        return DateFormatStyle.Long;
      else
        throw new FormatException($"Date format style \"{styleKeyword}\" is unsupported");
    }

    // Parse a plural selector
    private static PluralSelector ParsePluralSelector(Scanner scanner)
    {
      if (scanner.Match('='))
        return new PluralSelector.Explicit(ParseNumber(scanner));
      else
        return new PluralSelector.Keyword(ParsePluralKeyword(scanner));
    }

    // Parse a plural keyword
    private static PluralKeyword ParsePluralKeyword(Scanner scanner)
    {
      if (scanner.Match("zero"))
        return PluralKeyword.Zero;
      else if (scanner.Match("one"))
        return PluralKeyword.One;
      else if (scanner.Match("two"))
        return PluralKeyword.Two;
      else if (scanner.Match("few"))
        return PluralKeyword.Few;
      else if (scanner.Match("many"))
        return PluralKeyword.Many;
      else if (scanner.Match("other"))
        return PluralKeyword.Other;
      else
        throw new FormatException("Expected one of \"zero\", \"one\", \"two\", \"few\", \"many\", \"other\"");
    }
    #endregion
  }
}
