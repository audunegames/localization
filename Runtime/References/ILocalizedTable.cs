namespace Audune.Localization
{
  // Interface that defines a table of localized values
  public interface ILocalizedTable<TValue>
  {
    // Return if an entry with the specified path can be found and store the value of the entry
    public bool TryFind(string path, out TValue value);
  }
}