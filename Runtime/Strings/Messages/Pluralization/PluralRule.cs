namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a predicate in a plural rule.
  /// </summary>
  public sealed class PluralRule
  {
    // Enum that defines the opcode of a predicate
    internal enum Opcode : byte
    {
      Nop = 0x00,
      False = 0x01,
      True = 0x02,
      Const = 0x03,
      OperandN = 0x08,
      OperandI = 0x09,
      OperandV = 0x0A,
      OperandW = 0x0B,
      OperandF = 0x0C,
      OperandT = 0x0D,
      OperandC = 0x0E,
      Modulo = 0x14,
      Equals = 0x20,
      NotEquals = 0x21,
      InRange = 0x22,
      NotInRange = 0x23,
      And = 0x2E,
      Or = 0x2F,
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


    // Parse a plural rule from the specified string
    public static PluralRule Parse(PluralKeyword keyword, string input)
    {
      return PluralRuleParser.Parse(keyword, input);
    }
  }
}