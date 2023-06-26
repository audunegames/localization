namespace Audune.Localization
{
  // Interface that defines a localized reference
  public interface ILocalizedReference<TValue>
  {
    // Return if the reference can be resolved
    public bool TryResolve(ILocalizedTable<TValue> table, out TValue value);

    // Return the resolved value of the reference, or a default value if it cannot be resolved
    public TValue Resolve(ILocalizedTable<TValue> table, TValue defaultValue = default);
  }
}