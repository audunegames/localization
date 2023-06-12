using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a localized reference
  [Serializable]
  public class LocalizedReference<TValue> : IEquatable<LocalizedReference<TValue>>
  {
    // The path of the reference, in case the reference is a table reference
    [SerializeField]
    private string _path = null;

    // The value of the reference, in case the reference is a value reference
    [SerializeField]
    private TValue _value = default;


    // Return if the reference is a value reference
    public bool IsNonLocalized => string.IsNullOrEmpty(_path);


    // Constructor
    public LocalizedReference(string path)
    {
      _path = path;
    }
    public LocalizedReference(TValue value)
    {
      _value = value;
    }


    // Return if the reference can be resolved using the specified table and store the value
    public virtual bool TryResolve(LocalizedTable<TValue> table, out TValue value)
    {
      value = _value;

      if (!IsNonLocalized)
        return table.TryFind(_path, out value);
      else
        return true;
    }

    // Return the available paths of the reference using the specified table
    public IEnumerable<string> AvailablePaths(LocalizedTable<TValue> table)
    {
      return table.Entries.Keys;
    }

    // Return an applied localized reference based on the reference
    public AppliedLocalizedReference<TValue> Apply(Func<TValue, TValue> applier)
    {
      if (!IsNonLocalized)
        return new AppliedLocalizedReference<TValue>(_path, applier);
      else
        return new AppliedLocalizedReference<TValue>(_value, applier);
    }


    // Return the string representation of the reference
    public override string ToString()
    {
      if (!IsNonLocalized)
        return _path;
      else
        return $"<Non-Localized Value: {_value}>";
    }


    #region Equatable implementation
    // Return if the reference equals another object
    public override bool Equals(object obj)
    {
      return Equals(obj as LocalizedReference<TValue>);
    }

    // Return if the reference equals another reference
    public bool Equals(LocalizedReference<TValue> other)
    {
      return other is not null && _path == other._path && EqualityComparer<TValue>.Default.Equals(_value, other._value);
    }

    // Return the hash code of the reference
    public override int GetHashCode()
    {
      return HashCode.Combine(_path, _value);
    }
    #endregion

    #region Equality operators
    // Return if the reference equals another reference
    public static bool operator ==(LocalizedReference<TValue> left, LocalizedReference<TValue> right)
    {
      return EqualityComparer<LocalizedReference<TValue>>.Default.Equals(left, right);
    }

    // Return if the reference does not equal another reference
    public static bool operator !=(LocalizedReference<TValue> left, LocalizedReference<TValue> right)
    {
      return !(left == right);
    }
    #endregion

    #region Implicit operators
    // Convert a value to a value reference
    public static implicit operator LocalizedReference<TValue>(TValue value)
    {
      return new LocalizedReference<TValue>(value);
    }
    #endregion
  }
}