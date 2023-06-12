using System;

namespace Audune.Localization
{
  // Class that defines an applied localized reference
  public class AppliedLocalizedReference<TValue> : LocalizedReference<TValue>
  {
    // Function
    public Func<TValue, TValue> applier;


    // Constructor
    public AppliedLocalizedReference(string path, Func<TValue, TValue> applier) : base(path)
    {
      this.applier = applier;
    }
    public AppliedLocalizedReference(TValue value, Func<TValue, TValue> applier) : base(value)
    {
      this.applier = applier;
    }


    // Return if the reference can be resolved using the specified table and store the value
    public override bool TryResolve(LocalizedTable<TValue> table, out TValue value)
    {
      var result = base.TryResolve(table, out value);
      value = result ? applier(value) : value;
      return result;
    }
  }
}