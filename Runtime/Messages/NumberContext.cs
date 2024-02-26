using System;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a context for pluralization for a number
  public sealed class NumberContext
  {
    // Number context properties
    public readonly float value;
    public readonly float absoluteValue;
    public readonly int absoluteIntValue;
    public readonly int fractionDigitsCount;
    public readonly int significantFractionDigitsCount;
    public readonly int fractionDigits;
    public readonly int significantFractionDigits;
    public readonly int exponent;


    // Private constructor
    private NumberContext(float floatValue, string formattedFloatValue)
    {
      value = floatValue;
      absoluteValue = Mathf.Abs(floatValue);
      absoluteIntValue = (int)absoluteValue;

      var dotIndex = formattedFloatValue.IndexOf('.');
      if (dotIndex > -1)
      {
        var fraction = formattedFloatValue[(dotIndex + 1)..];
        var significantFraction = fraction.TrimEnd('0');

        fractionDigitsCount = fraction.Length;
        significantFractionDigitsCount = significantFraction.Length;
        fractionDigits = int.Parse(fraction, NumberStyles.Integer, CultureInfo.InvariantCulture);
        significantFractionDigits = !string.IsNullOrEmpty(significantFraction) ? int.Parse(significantFraction, NumberStyles.Integer, CultureInfo.InvariantCulture) : 0;
        exponent = 0;
      }
      else
      {
        fractionDigitsCount = 0;
        significantFractionDigitsCount = 0;
        fractionDigits = 0;
        significantFractionDigits = 0;
        exponent = 0;
      }
    }


    // Return a number context with the specified offset applied
    internal NumberContext Offset(float offset)
    {
      return Of(value - offset);
    }


    // Create a number context from a float value
    public static NumberContext Of(float floatValue)
    {
      return new NumberContext(floatValue, floatValue.ToString("R", CultureInfo.InvariantCulture));
    }

    // Create a number context from an integer value
    public static NumberContext Of(int intValue)
    {
      return new NumberContext(intValue, intValue.ToString("D", CultureInfo.InvariantCulture));
    }

    // Create a number context from a string value
    public static NumberContext Of(string stringValue)
    {
      if (float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedStringValue))
        return new NumberContext(parsedStringValue, stringValue);
      else
        throw new FormatException($"Invalid number format in string \"{stringValue}\"");
    }
  }
};