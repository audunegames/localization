namespace Audune.Localization
{
  // Interface that defines a localized reference
  public interface ILocalizedReference<TValue>
  {
    // Return if the reference can be resolved and store the value of the resolved reference
    public bool TryResolve(ILocalizedTable<TValue> table, out TValue value);
  }
}