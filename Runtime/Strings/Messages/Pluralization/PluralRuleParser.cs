﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a parser for plural rules.
  /// </summary>
  internal static class PluralRuleParser
  {
    // Enum that defines an operator for a relational condition
    public enum Operator
    {
      Equals,
      NotEquals,
    }


    // Parse a rules
    public static PluralRule Parse(PluralKeyword keyword, string input)
    {
      var scanner = new Scanner(input);

      if (keyword == PluralKeyword.Other)
      {
        if (!Try(scanner, ParseRule, out var code))
          code = new List<byte>() { (byte)PluralRule.Opcode.True };
        scanner.AssertAtEnd();
        return new PluralRule(keyword, code.ToArray());
      }
      else
      {
        var code = ParseRule(scanner);
        scanner.AssertAtEnd();
        return new PluralRule(keyword, code.ToArray());
      }
    }


    #region Rule parser functions
    // Parse a parser function optionally
    private static bool Try<TResult>(Scanner scanner, Func<Scanner, TResult> parser, out TResult result)
    {
      try
      {
        result = parser(scanner);
        return true;
      }
      catch (FormatException)
      {
        result = default;
        return false;
      }
    }

    // Parse a rule
    private static List<byte> ParseRule(Scanner scanner)
    {
      return ParseOrCondition(scanner);
    }

    // Parse an or condition
    private static List<byte> ParseOrCondition(Scanner scanner)
    {
      var code = ParseAndCondition(scanner);
      scanner.SkipWhile(Scanner.IsWhitespace);

      while (scanner.Match("or"))
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        code.AddRange(ParseAndCondition(scanner));
        code.Add((byte)PluralRule.Opcode.Or);
        scanner.SkipWhile(Scanner.IsWhitespace);
      }

      return code;
    }

    // Parse an and condition
    private static List<byte> ParseAndCondition(Scanner scanner)
    {
      var code = ParseRelation(scanner);
      scanner.SkipWhile(Scanner.IsWhitespace);

      while (scanner.Match("and"))
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        code.AddRange(ParseRelation(scanner));
        code.Add((byte)PluralRule.Opcode.And);
        scanner.SkipWhile(Scanner.IsWhitespace);
      }

      return code;
    }

    // Parse a relation
    private static List<byte> ParseRelation(Scanner scanner)
    {
      var expr = ParseExpression(scanner);
      scanner.SkipWhile(Scanner.IsWhitespace);

      var op = ParseOperator(scanner);
      scanner.SkipWhile(Scanner.IsWhitespace);

      var code = ParseList(scanner, expr, op);
      scanner.SkipWhile(Scanner.IsWhitespace);     

      return code;
    }

    // Parse an expression
    private static List<byte> ParseExpression(Scanner scanner)
    {
      var code = new List<byte> { ParseOperand(scanner) };
      scanner.SkipWhile(Scanner.IsWhitespace);

      if (scanner.Match('%'))
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        code.Add((byte)PluralRule.Opcode.Const);
        code.AddRange(ParseValue(scanner, out _));
        code.Add((byte)PluralRule.Opcode.Modulo);
        scanner.SkipWhile(Scanner.IsWhitespace);
      }

      return code;
    }

    // Parse an operand
    private static byte ParseOperand(Scanner scanner)
    {
      if (scanner.Match('n'))
        return (byte)PluralRule.Opcode.OperandN;
      else if (scanner.Match('i'))
        return (byte)PluralRule.Opcode.OperandI;
      else if (scanner.Match('v'))
        return (byte)PluralRule.Opcode.OperandV;
      else if (scanner.Match('w'))
        return (byte)PluralRule.Opcode.OperandW;
      else if (scanner.Match('f'))
        return (byte)PluralRule.Opcode.OperandF;
      else if (scanner.Match('t'))
        return (byte)PluralRule.Opcode.OperandT;
      else if (scanner.Match('c', 'e'))
        return (byte)PluralRule.Opcode.OperandC;
      else
        throw new FormatException($"Expected one of \"n\", \"i\", \"v\", \"w\", \"f\", \"t\", \"c\", \"e\" at index {scanner.index}");
    }

    // Parse an operator
    private static Operator ParseOperator(Scanner scanner)
    {
      if (scanner.Match("="))
        return Operator.Equals;
      if (scanner.Match("!="))
        return Operator.NotEquals;
      else
        throw new FormatException($"Expected one of \"=\", \"!=\" at index {scanner.index}");
    }

    // Parse a list
    private static List<byte> ParseList(Scanner scanner, List<byte> expr, Operator op)
    {
      var code = ParseRange(scanner, expr, op);
      scanner.SkipWhile(Scanner.IsWhitespace);

      while (scanner.Match(","))
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        code.AddRange(ParseRange(scanner, expr, op));
        code.Add((byte)PluralRule.Opcode.Or);
        scanner.SkipWhile(Scanner.IsWhitespace);
      }

      return code;
    }

    // Parse a range
    private static List<byte> ParseRange(Scanner scanner, List<byte> expr, Operator op)
    {
      var code = new List<byte>();
      code.AddRange(expr);

      code.Add((byte)PluralRule.Opcode.Const);
      code.AddRange(ParseValue(scanner, out var min));
      scanner.SkipWhile(Scanner.IsWhitespace);

      if (scanner.Match(".."))
      {
        scanner.SkipWhile(Scanner.IsWhitespace);

        code.Add((byte)PluralRule.Opcode.Const);
        code.AddRange(ParseValue(scanner, out var max));
        scanner.SkipWhile(Scanner.IsWhitespace);

        if (max <= min)
          throw new FormatException("The maximal value of a range must be higher than its minimal value");

        if (op == Operator.Equals)
          code.Add((byte)PluralRule.Opcode.InRange);
        else if (op == Operator.NotEquals)
          code.Add((byte)PluralRule.Opcode.NotInRange);
      }
      else
      {
        if (op == Operator.Equals)
          code.Add((byte)PluralRule.Opcode.Equals);
        else if (op == Operator.NotEquals)
          code.Add((byte)PluralRule.Opcode.NotEquals);
      }

      return code;
    }

    // Parse a value
    private static List<byte> ParseValue(Scanner scanner, out float floatValue)
    {
      var builder = new StringBuilder();

      if (scanner.Match('0'))
        builder.Append(scanner.current);
      else
        builder.Append(scanner.ReadWhile(Scanner.IsNonZeroDigit, "non-zero digit", Scanner.IsDigit));

      var value = builder.ToString();
      if (!float.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out floatValue))
        throw new FormatException($"Number \"{value}\" has an invalid number format");

      return new List<byte>(BitConverter.GetBytes(floatValue));
    }
    #endregion
  }
}
