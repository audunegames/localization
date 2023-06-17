using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a table of localized strings
  [Serializable]
  public class LocalizedTable<TValue> : ISerializationCallbackReceiver
  {
    // Dictionary of entries in the table
    private readonly Dictionary<string, TValue> _entries = new();

    // Dictionary of child tables in the table
    private readonly Dictionary<string, LocalizedTable<TValue>> _tables = new();


    // Return the entries in the table
    public IReadOnlyDictionary<string, TValue> Entries => _entries;

    // Return the child tables in the table
    public IReadOnlyDictionary<string, LocalizedTable<TValue>> Tables => _tables;


    // Return the entries in the table recursively
    public IReadOnlyDictionary<string, TValue> RecursiveEntries => _entries
      .Concat(_tables.SelectMany(table => table.Value.RecursiveEntries.Select(e => new KeyValuePair<string, TValue>($"{table.Key}.{e.Key}", e.Value))))
      .OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase)
      .ToDictionary(e => e.Key, e => e.Value);

    // Return the table in the table recursively
    public IReadOnlyDictionary<string, LocalizedTable<TValue>> RecursiveTables => _tables
      .Concat(_tables.SelectMany(table => table.Value.RecursiveTables.Select(e => new KeyValuePair<string, LocalizedTable<TValue>>($"{table.Key}.{e.Key}", e.Value))))
      .OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase)
      .ToDictionary(e => e.Key, e => e.Value);


    // Find an entry in the table with the specified path
    public TValue Find(string path)
    {
      return TryFind(path, out var value) ? value : default;
    }

    // Return if an entry in the table with the specified path can be found and store the entry
    public bool TryFind(string path, out TValue value)
    {
      if (path == null)
      {
        value = default;
        return false;
      }
      return RecursiveEntries.TryGetValue(path, out value);
    }


    // Add an entry to the table
    internal void Add(string key, TValue value)
    {
      _entries.Add(key, value);
    }

    // Remove an entry from the table
    internal void Remove(string key)
    {
      _entries.Remove(key);
    }

    // Return a child table in the table or create and add it if it does not exist yet
    internal LocalizedTable<TValue> GetTableOrCreate(string key)
    {
      if (!_tables.TryGetValue(key, out var table))
        table = _tables[key] = new LocalizedTable<TValue>();
      return table;
    }


    #region Serialization callbacks
    // Lists for keys and values for serialization
    [SerializeField] private List<string> _keys = new List<string>();
    [SerializeField] private List<TValue> _values = new List<TValue>();


    // Callback received before Unity serializes the table
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      _keys.Clear();
      _values.Clear();

      foreach (var e in RecursiveEntries)
      {
        _keys.Add(e.Key);
        _values.Add(e.Value);
      }
    }

    // Callback after Unity deserializes the table
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      _entries.Clear();
      _tables.Clear();

      for (var i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
      {
        var keyComponents = _keys[i].Split('.');
        var keyTable = this;
        for (var k = 0; k < keyComponents.Length - 1; k++)
          keyTable = keyTable.GetTableOrCreate(keyComponents[k]);
        keyTable.Add(keyComponents[^1], _values[i]);
      }
    }
    #endregion
  }
}