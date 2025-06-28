using System;
using System.Globalization;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a context for pluralization of a number.
  /// </summary>
  public sealed class NumberContext
  {
    // Number context properties

    /// <summary>
    /// The value of the number context.
    /// </summary>
    public readonly float value;

    /// <summary>
    /// The absolute value of the number context.
    /// </summary>
    public readonly float absoluteValue;

    /// <summary>
    /// The absolute value as an integer of the number context.
    /// </summary>
    public readonly int absoluteIntValue;

    /// <summary>
    /// The amount of faction digits of the number context.
    /// </summary>
    public readonly int fractionDigitsCount;

    /// <summary>
    /// The amount of significant fraction digits of the number context.
    /// </summary>
    public readonly int significantFractionDigitsCount;

    /// <summary>
    /// The fraction digits of the number context.
    /// </summary>
    public readonly int fractionDigits;

    /// <summary>
    /// The significant fraction digits of the number context.
    /// </summary>
    public readonly int significantFractionDigits;

    /// <summary>
    /// The exponent of the number context.
    /// </summary>
    public readonly int exponent;


    /// <summary>
    /// Private constructor.
    /// </summary>
    /// <param name="floatValue">The value of the number context.</param>
    /// <param name="formattedFloatValue">The formatted value of the number context.</param>
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


    /// <summary>
    /// Return a number context with the specified offset applied.
    /// </summary>
    /// <param name="offset">The offset to apply to the number context.</param>
    /// <returns>A new number context with the specified offset applied.</returns>
    internal NumberContext Offset(float offset)
    {
      return Of(value - offset);
    }

    /// <summary>
    /// Return the string representation of the number context.
    /// </summary>
    /// <returns>The string representation of the number context</returns>
    public override string ToString()
    {
      return value.ToString("R", CultureInfo.InvariantCulture);
    }


    /// <summary>
    /// Create a number context from a float value.
    /// </summary>
    /// <param name="floatValue">The float value to base the number context on.</param>
    /// <returns>A number context for the specified value.</returns>
    public static NumberContext Of(float floatValue)
    {
      return new NumberContext(floatValue, floatValue.ToString("R", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Create a number context from an integer value.
    /// </summary>
    /// <param name="intValue">The integer value to base the number context on.</param>
    /// <returns>A number context for the specified value.</returns>
    public static NumberContext Of(int intValue)
    {
      return new NumberContext(intValue, intValue.ToString("D", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Create a number context from a string value.
    /// </summary>
    /// <param name="stringValue">The string value to base the number context on.</param>
    /// <returns>A number context for the specified value.</returns>
    /// <exception cref="FormatException">Thrown when the specified string value could not be parsed as a float.</exception>
    public static NumberContext Of(string stringValue)
    {
      if (float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedStringValue))
        return new NumberContext(parsedStringValue, stringValue);
      else
        throw new FormatException($"Invalid number format in string \"{stringValue}\"");
    }
  }
};