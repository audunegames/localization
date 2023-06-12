using System;
using System.IO;
using System.Linq;
using Tommy;
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

        // Load the locale settings
        if (!node.TryGetNode("code", out var codeNode) || codeNode is not TomlString codeStringNode)
          throw new LocaleParserException("Could not find a \"code\" string node");
        locale._code = codeStringNode.AsString;

        /*if (!node.TryGetNode("steamCode", out var steamCodeNode) || steamCodeNode is not TomlString steamCodeStringNode)
          throw new LocaleParserException("Could not find a \"SteamCode\" string node");
        locale._steamCode = steamCodeStringNode.AsString;*/

        if (!node.TryGetNode("name", out var nameNode) || nameNode is not TomlString nameStringNode)
          throw new LocaleParserException("Could not find a \"name\" string node");
        locale._name = nameStringNode.AsString;

        // Load the localized string table
        if (!node.TryGetNode("strings", out var stringsNode) || stringsNode is not TomlTable stringsTableNode)
          throw new LocaleParserException("Could not find a \"strings\" table node");
        locale._strings = new LocalizedTable<string>();
        FillLocalizedTable(locale._strings, stringsTableNode, value => value);

        // Return the locale
        return locale;
      }
      catch (TomlParseException ex)
      {
        var errorStrings = string.Join("\n", ex.SyntaxErrors.Select(error => $"  at line {error.Line}, col {error.Column}: {error.Message}"));
        throw new LocaleParserException($"Could not parse TOML\n{errorStrings}");
      }
    }


    // Fill a localized table with the leaf nodes of a node
    private static void FillLocalizedTable<TNode, TValue>(LocalizedTable<TValue> table, TomlNode node, Func<TNode, TValue> valueSelector) where TNode : TomlNode
    {
      foreach (var key in node.Keys)
      {
        var childNode = node[key];
        if (childNode is TomlTable childTableNode)
          FillLocalizedTable(table.GetTableOrCreate(key), childTableNode, valueSelector);
        else if (childNode is TNode childValueNode)
          table.Add(key, valueSelector(childValueNode));
      }
    }

    // Fill a localized table with the leaf nodes of a string node
    private static void FillLocalizedTable<TValue>(LocalizedTable<TValue> table, TomlNode node, Func<string, TValue> valueSelector)
    {
      FillLocalizedTable<TomlString, TValue>(table, node, node => valueSelector(node.Value));
    }
  }
}