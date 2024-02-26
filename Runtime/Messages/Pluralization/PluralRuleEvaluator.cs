using System;
using System.Collections.Generic;
using System.IO;

namespace Audune.Localization
{
  // Class that defines an evaluator for a plural rule
  internal class PluralRuleEvaluator
  {
    // Internal state of the evaluator
    private PluralRule _rule;
    private Stack<object> _stack = new Stack<object>();


    // Constructor
    public PluralRuleEvaluator(PluralRule rule)
    {
      _rule = rule;
    }


    // Evaluate if the plural rule matches for a number context value
    public bool Matches(NumberContext number)
    {
      // Clear the stack
      _stack.Clear();

      // Create a reader over the code of the rule
      using var stream = new MemoryStream(_rule._code);
      using var reader = new BinaryReader(stream);

      // Iterate over the reader while there are bytes available
      while (reader.BaseStream.Position < reader.BaseStream.Length)
      {
        // Read the opcode 
        var opcode = (PluralRule.Opcode)reader.ReadByte();

        // Check the opcode
        if (opcode == PluralRule.Opcode.Nop)
        {
          // Do nothing
        }
        else if (opcode == PluralRule.Opcode.False)
        {
          // Push a false boolean value to the stack
          PushBool(false);
        }
        else if (opcode == PluralRule.Opcode.True)
        {
          // Push a true boolean value to the stack
          PushBool(true);
        }
        else if (opcode == PluralRule.Opcode.Const)
        {
          // Push a constant float value to the stack
          PushFloat(reader.ReadSingle());
        }
        else if (opcode == PluralRule.Opcode.ValueAsNumber)
        {
          // Push the absolute value of the value to match against to the stack
          PushFloat(number.absoluteValue);
        }
        else if (opcode == PluralRule.Opcode.ValueAsInt)
        {
          // Push the absolute integer value of the value to match against to the stack
          PushFloat(number.absoluteIntValue);
        }
        else if (opcode == PluralRule.Opcode.ValueAsFracDigitsCount)
        {
          // Push the number of fraction digits *with* trailing zeroes of the value to match against to the stack
          PushFloat(number.fractionDigitsCount);
        }
        else if (opcode == PluralRule.Opcode.ValueAsSignificantFracDigitsCount)
        {
          // Push the number of fraction digits *without* trailing zeroes of the value to match against to the stack
          PushFloat(number.significantFractionDigitsCount);
        }
        else if (opcode == PluralRule.Opcode.ValueAsFracDigits)
        {
          // Push the fraction digits *with* trailing zeroes of the value to match against to the stack
          PushFloat(number.fractionDigits);
        }
        else if (opcode == PluralRule.Opcode.ValueAsSignificantFracDigits)
        {
          //  Push the fraction digits *without* trailing zeroes of the value to match against to the stack
          PushFloat(number.significantFractionDigits);
        }
        else if (opcode == PluralRule.Opcode.ValueAsExp)
        {
          // Push the exponent of the value to match against to the stack
          PushFloat(number.exponent);
        }
        else if (opcode == PluralRule.Opcode.Modulo)
        {
          // Push the modulo of the two topmost floats to the stack
          var left = PopFloat();
          var right = PopFloat();
          PushFloat(left % right);
        }
        else if (opcode == PluralRule.Opcode.Equals)
        {
          // Push if the two topmost floats are equal to each other to the stack
          var left = PopFloat();
          var right = PopFloat();
          PushBool(left == right);
        }
        else if (opcode == PluralRule.Opcode.NotEquals)
        {
          // Push if the two topmost floats are not equal to each other to the stack
          var left = PopFloat();
          var right = PopFloat();
          PushBool(left != right);
        }
        else if (opcode == PluralRule.Opcode.InRange)
        {
          // Push if the topmost float are is in the range of the next two floats to the stack
          var left = PopFloat();
          var min = PopFloat();
          var max = PopFloat();
          PushBool(left >= min && left <= max);
        }
        else if (opcode == PluralRule.Opcode.NotInRange)
        {
          // Push if the topmost float are is in the range of the next two floats to the stack
          var left = PopFloat();
          var min = PopFloat();
          var max = PopFloat();
          PushBool(!(left >= min && left <= max));
        }
        else if (opcode == PluralRule.Opcode.And)
        {
          // Push the logical and of the two topmost bool values on the stack
          var left = PopBool();
          var right = PopBool();
          PushBool(left && right);
        }
        else if (opcode == PluralRule.Opcode.Or)
        {
          // Push the logical or of the two topmost bool values on the stack
          var left = PopBool();
          var right = PopBool();
          PushBool(left || right);
        }
        else
        {
          // Invalid opcode
          throw new ArgumentException($"Invalid opcode {opcode}");
        }
      }

      // Return the topmost bool and check if the stack is empty afterwards
      var result = PopBool();
      if (_stack.Count > 0)
        throw new ArgumentException($"Invalid stack state after evaluating");
      return result;
    }


    #region Pushing values to and popping values from the stack
    // Push a float value to the stack
    public void PushFloat(float value)
    {
      _stack.Push(value);
    }

    // Push a boolean value to the stack
    public void PushBool(bool value)
    {
      _stack.Push(value);
    }

    // Pop a float value from the stack
    public float PopFloat()
    {
      if (_stack.Count == 0)
        throw new ArgumentException($"Expected a value of type {typeof(float)} on the stack, but the stack is empty");

      var value = _stack.Pop();
      if (value is float floatValue)
        return floatValue;
      else
        throw new ArgumentException($"Expected a value of type {typeof(float)} on the stack, but found a value of type {value.GetType()}");
    }

    // Pop a boolean value from the stack
    public bool PopBool()
    {
      if (_stack.Count == 0)
        throw new ArgumentException($"Expected a value of type {typeof(bool)} on the stack, but the stack is empty");

      var value = _stack.Pop();
      if (value is bool boolValue)
        return boolValue;
      else
        throw new ArgumentException($"Expected a value of type {typeof(bool)} on the stack, but found a value of type {value.GetType()}");
    }
    #endregion
  }
}