namespace Audune.Localization
{
  // Interface that defines a pluralizer
  public interface IPluralizer
  {
    // Select a plural keyword based on the specified number
    public PluralKeyword Pluralize(NumberContext number);
  }
}