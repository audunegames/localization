using System;
using System.Linq;
using System.Text;

namespace Audune.Localization
{
  // Class that defines a scanner that iterates over a string
  internal sealed class Scanner
  {
    // The input of the scanner
    private readonly string _input;

    // The index of the character that is currently being scanned
    private int _index = 0;


    // Return the index of the scanner
    public int index => _index;

    // Return if the scanner reached the end of the input string
    public bool atEnd => _index >= _input.Length;

    // Return the character that was just scanned
    public char current => _index > 0 ? _input[_index - 1] : '\0';

    // Return the character that is about to be scanned
    public char next => !atEnd ? _input[_index] : '\0';


    // Constructor
    public Scanner(string input, int index = 0)
    {
      _input = input ?? string.Empty;
      _index = index;
    }


    #region Scanner functions
    // Advance to the next character and return the just scanned character
    public char Advance(int amount = 1)
    {
      if (!atEnd)
        _index += amount;
      return current;
    }

    // Check if the next character matches the predicate
    public bool Check(Predicate<char> predicate)
    {
      return !atEnd && predicate(next);
    }

    // Check if the next character is one of the specified characters
    public bool Check(params char[] chars)
    {
      return Check(c =>chars.Contains(c));
    }

    // Check if the next character sequence matches the specified string
    public bool Check(string str)
    {
      return !atEnd && _index + str.Length < _input.Length && str == _input[_index..(_index + str.Length)];
    }

    // Check if the next character matches the predicate and advance if so
    public bool Match(Predicate<char> predicate)
    {
      if (Check(predicate))
      {
        Advance();
        return true;
      }

      return false;
    }

    // Check if the next character is one of the specified characters and advance if so
    public bool Match(params char[] chars)
    {
      return Match(c => chars.Contains(c));
    }

    // Check if the next character sequence matches the specified string and advance if so
    public bool Match(string str)
    {
      if (Check(str))
      {
        Advance(str.Length);
        return true;
      }

      return false;
    }

    // Consume the next character if it matches the predicate or throw an error if the consuming was unsuccesful
    public char Consume(Predicate<char> predicate, string expected)
    {
      if (Match(predicate))
        return current;
      else
        throw new FormatException($"Expected {expected}, but found {(!atEnd ? $"'{_input[_index]}'" : "end of string")} at index {_index}");
    }

    // Consume the next character if it is one of the specified characters or throw an error if the consuming was unsuccesful
    public char Consume(params char[] chars)
    {
      return Consume(c => chars.Contains(c), chars.Length > 1 ? $"one of {string.Join(", ", chars.Select(c => $"'{c}'"))}" : $"'{chars[0]}'");
    }

    // Consume the next character sequence if it matches the specified string or throw an error if the consuming was unsuccesful
    public string Consume(string str)
    {
      var start = _index;
      if (Match(str))
        return _input[start..(_index - 1)];
      else
        throw new FormatException($"Expected \"{str}\"{(!atEnd ? ", but found end of string" : "")} at index {_index}");
    }

    // Consume the next character while it matches the predicate and return the consumed characters
    public string ReadWhile(Predicate<char> predicate)
    {
      var builder = new StringBuilder();
      while (Match(predicate))
        builder.Append(current);
      return builder.ToString();
    }

    // Consume the next character while it is one of the specified characters and return the consumed characters
    public string ReadWhile(params char[] chars)
    {
      return ReadWhile(c => chars.Contains(c));
    }

    // Consume the next character while it matches the predicate with a special case for the first character and return the consumed characters
    public string ReadWhile(Predicate<char> firstPredicate, string firstExpected, Predicate<char> restPredicate)
    {
      var str = new string(Consume(firstPredicate, firstExpected), 1);
      str += ReadWhile(restPredicate);
      return str;
    }

    // Consume the next character while it is one of the specified characters with a special case for the first character and return the consumed characters
    public string ReadWhile(char[] firstChars, char[] restChars)
    {
      Consume(firstChars);
      return ReadWhile(restChars);
    }

    // Consume the next character while it matches the predicate without saving the consumed characters
    public void SkipWhile(Predicate<char> predicate)
    {
      while (true)
      {
        if (!Match(predicate))
          break;
      }
    }

    // Consume the next character while it is one of the specified characters without saving the consumed characters
    public void SkipWhile(params char[] chars)
    {
      SkipWhile(c => chars.Contains(c));
    }

    // Asser tthat the scanner has reached the end of the input string
    public void AssertAtEnd()
    {
      if (!atEnd)
        throw new FormatException($"Expected end of string, but found '{next}' at index {_index}");
    }
    #endregion

    #region Scanner predicates
    // Return if a character is whitespace
    public static bool IsWhitespace(char c) => c == ' ';

    // Return if a character is a lowercase letter
    public static bool IsLowercaseLetter(char c) => c >= 'a' && c <= 'z';

    // Return if a character is a uppercase letter
    public static bool IsUppercaseLetter(char c) => c >= 'A' && c <= 'Z';

    // Return if a character is a letter
    public static bool IsLetter(char c) => IsLowercaseLetter(c) || IsUppercaseLetter(c);

    // Return if a character is a non-zero digit
    public static bool IsNonZeroDigit(char c) => c >= '1' && c <= '9';

    // Return if a character is a digit
    public static bool IsDigit(char c) => IsNonZeroDigit(c) || c == '0';

    // Return if a character is a letter or digit
    public static bool IsLetterOrDigit(char c) => IsLetter(c) || IsDigit(c);

    // Return if a character is a letter or an underscore
    public static bool IsLetterOrUnderscore(char c) => IsLetter(c) || c == '_';

    // Return if a character is a letter, a digit, or an underscore
    public static bool IsLetterOrDigitOrUnderscore(char c) => IsLetter(c) || IsDigit(c) || c == '_';
    #endregion
  }
}