namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a pluralizer.
  /// </summary>
  public interface IPluralizer
  {
    /// <summary>
    /// Select a plural keyword based on the specified number.
    /// </summary>
    /// <param name="number">The number to use to select a plural keyword.</param>
    /// <returns>The plural keyword that corresponds to the specified number.</returns>
    public PluralKeyword Pluralize(NumberContext number);
  }
}