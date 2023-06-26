using Audune.Localization.Strings;
using Audune.Utils.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tommy;
using Tommy.Extensions;
using UnityEngine;

namespace Audune.Localization.Parsers
{
  // Class that parses a locale file in TOML format
  public class TomlParser : LocaleParser
  {
    // Parse a locale from a reader
    public override Locale Parse(TextReader reader)
    {
      try
      {
        // Parse the reader into a TOML node
        var parser = new TOMLParser(reader);
        var node = parser.Parse();

        // Create the locale
        var locale = ScriptableObject.CreateInstance<Locale>();

        // Parse the locale name
        if (node.TryGetNode("name", out var nameNode) && nameNode is TomlString nameStringNode)
          locale._name = nameStringNode.Value;
        else
          throw new FormatException("Could not find a \"name\" string node");

        // Parse the locale code
        if (node.TryGetNode("code", out var codeNode) && codeNode is TomlString codeStringNode)
          locale._code = codeStringNode.Value;
        else
          throw new FormatException("Could not find a \"code\" string or table node");

        // Parse the locale alternative codes
        if (node.TryGetNode("alt_codes", out var altCodesNode) && altCodesNode is TomlTable altCodesTableNode)
          locale._altCodes = new SerializableDictionary<string, string>(altCodesTableNode.RawTable.Where(e => e.Value.IsString).SelectValue(node => node.AsString.Value).ToDictionary());
        else
          locale._altCodes = new SerializableDictionary<string, string>();

        // Load the localized string table
        if (node.TryGetNode("strings", out var stringsNode) && stringsNode is TomlTable stringsTableNode)
          locale._strings = new LocalizedStringTable(RecurseLeafStringNodes(stringsTableNode, value => value).ToDictionary());
        else
          throw new FormatException("Could not find a \"strings\" table node");
        

        // Return the locale
        return locale;
      }
      catch (TomlParseException ex)
      {
        var errorStrings = string.Join("\n", ex.SyntaxErrors.Select(error => $"  at line {error.Line}, col {error.Column}: {error.Message}"));
        throw new FormatException($"Could not parse TOML\n{errorStrings}");
      }
    }


    // Recurse over the leaf nodes of a node
    private IEnumerable<KeyValuePair<string, TValue>> RecurseLeafNodes<TNode, TValue>(TomlNode node, Func<TNode, TValue> valueSelector) where TNode : TomlNode
    {
      foreach (var key in node.Keys)
      {
        var childNode = node[key];
        if (childNode is TomlTable childTableNode)
        {
          foreach (var e in RecurseLeafNodes(childTableNode, valueSelector))
            yield return new KeyValuePair<string, TValue>($"{key}.{e.Key}", e.Value);
        }
        else if (childNode is TNode childValueNode)
        {
          yield return new KeyValuePair<string, TValue>(key, valueSelector(childValueNode));
        }
      }
    }

    // Recurse over the leaf string nodes of a node
    private IEnumerable<KeyValuePair<string, TValue>> RecurseLeafStringNodes<TValue>(TomlNode node, Func<string, TValue> valueSelector)
    {
      return RecurseLeafNodes<TomlString, TValue>(node, n => valueSelector(n.Value));
    }
  }
}