using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  // Class that defines a list of localized references
  public class LocalizedReferenceList<TValue> : ILocalizedReference<TValue>, IReadOnlyList<ILocalizedReference<TValue>>
  {
    // Items of the sequence
    private readonly List<ILocalizedReference<TValue>> _items;

    // Aggregator of the sequence
    private readonly Func<TValue, TValue, TValue> _aggregator;


    // Return the item at the specified index
    public ILocalizedReference<TValue> this[int index] => _items[index];

    // Return the count of the items
    public int Count => _items.Count;


    // Constructor
    internal LocalizedReferenceList(IEnumerable<ILocalizedReference<TValue>> items, Func<TValue, TValue, TValue> aggregator)
    {
      _items = new List<ILocalizedReference<TValue>>(items);
      _aggregator = aggregator;
    }


    // Return if the reference can be resolved and store the value of the resolved reference
    public bool TryResolve(ILocalizedTable<TValue> table, out TValue value)
    {
      var values = _items.Select(reference => reference.TryResolve(table, out var value) ? (result: true, value) : (result: false, default));
      var result = values.All(t => t.result);
      value = result ? values.Select(t => t.value).Aggregate(_aggregator) : default;
      return result;
    }


    // Return a generic enumerator
    public IEnumerator<ILocalizedReference<TValue>> GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    // Return an enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}