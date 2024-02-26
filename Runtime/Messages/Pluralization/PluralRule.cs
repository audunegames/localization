using System;

namespace Audune.Localization
{
  // Class that defines a predicate in a plural rule
  public sealed class PluralRule
  {
    // Enum that defines the opcode of a predicate
    internal enum Opcode : byte
    {
      Nop = 0x00,
      False = 0x01,
      True = 0x02,
      Const = 0x03,
      ValueAsNumber = 0x08,
      ValueAsInt = 0x09,
      ValueAsFracDigitsCount = 0x0A,
      ValueAsSignificantFracDigitsCount = 0x0B,
      ValueAsFracDigits = 0x0C,
      ValueAsSignificantFracDigits = 0x0D,
      ValueAsExp = 0x0E,
      Modulo = 0x11,
      Equals = 0x18,
      NotEquals = 0x19,
      InRange = 0x1A,
      NotInRange = 0x1B,
      And = 0x21,
      Or = 0x22,
    }


    // Internal state of the plural rule
    private readonly PluralKeyword _keyword;
    public readonly byte[] _code;
    private readonly PluralRuleEvaluator _evaluator;


    // Return the keyword of the plural rule
    public PluralKeyword keyword => _keyword;


    // Constructor
    public PluralRule(PluralKeyword keyword, byte[] code)
    {
      _keyword = keyword;
      _code = code;
      _evaluator = new PluralRuleEvaluator(this);
    }


    // Return if the rule matches the specified number
    public bool Matches(NumberContext number)
    {
      return _evaluator.Matches(number);
    }


    public override string ToString()
    {
      return $"{keyword}: {BitConverter.ToString(_code).Replace("-", " ")}";
    }


    // Parse a plural rule from the specified string
    public static PluralRule Parse(PluralKeyword keyword, string input)
    {
      return PluralRuleParser.Parse(keyword, input);
    }
  }
}