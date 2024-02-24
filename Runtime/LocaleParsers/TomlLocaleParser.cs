using Audune.Utils.Dictionary;
using Audune.Utils.UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tommy;
using Tommy.Extensions;
using UnityEngine;

namespace Audune.Localization
{
  // Class that parses a locale file in TOML format
  [TypeDisplayName("TOML")]
  public class TomlLocaleParser : LocaleParser
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

        // Parse the locale code
        if (node.TryGetNode("code", out var codeNode) && codeNode is TomlString codeStringNode)
          locale._code = codeStringNode.Value;
        else
          throw new LocaleParserException("Could not find a \"code\" string or table node");

        // Parse the English name of the locale
        if (node.TryGetNode("english_name", out var englishNameNode) && englishNameNode is TomlString englishNameStringNode)
          locale._englishName = englishNameStringNode.Value;
        else
          throw new LocaleParserException("Could not find a \"english_name\" string node");

        // Parse the native name of the locale
        if (node.TryGetNode("native_name", out var nativeNameNode) && nativeNameNode is TomlString nativeNameStringNode)
          locale._nativeName = nativeNameStringNode.Value;
        else
          throw new LocaleParserException("Could not find a \"native_name\" string node");

        // Parse the alternative codes of the locale
        if (node.TryGetNode("alt_codes", out var altCodesNode) && altCodesNode is TomlTable altCodesTableNode)
          locale._altCodes = new SerializableDictionary<string, string>(altCodesTableNode.RawTable.Where(e => e.Value.IsString).SelectOnValue(node => node.AsString.Value).ToDictionary());
        else
          locale._altCodes = new SerializableDictionary<string, string>();

        // Parse the locale formats
        locale._decimalNumberFormat = node.FindNode("number_format.decimal")?.AsString ?? Locale.defaultDecimalNumberFormat;
        locale._percentNumberFormat = node.FindNode("number_format.percent")?.AsString ?? Locale.defaultPercentNumberFormat;
        locale._currencyNumberFormat = node.FindNode("number_format.currency")?.AsString ?? Locale.defaultCurrencyNumberFormat;
        locale._shortDateFormat = node.FindNode("date_format.short")?.AsString ?? Locale.defaultShortDateFormat;
        locale._longDateFormat = node.FindNode("date_format.long")?.AsString ?? Locale.defaultLongDateFormat;
        locale._shortTimeFormat = node.FindNode("time_format.short")?.AsString ?? Locale.defaultShortTimeFormat;
        locale._longTimeFormat = node.FindNode("time_format.long")?.AsString ?? Locale.defaultLongTimeFormat;

        // Parse the plural rules of the locale
        if (node.TryGetNode("plural_rules", out var pluralRulesNode) && pluralRulesNode is TomlTable pluralRulesTableNode)
          locale._pluralRules = new SerializableDictionary<PluralKeyword, string>(pluralRulesTableNode.RawTable.Where(e => e.Value.IsString).SelectOnKey(key => PluralKeywordUtils.Parse(key)).SelectOnValue(node => node.AsString.Value).ToDictionary());
        else
          locale._pluralRules = new SerializableDictionary<PluralKeyword, string>();

        

        // Load the localized string table
        if (node.TryGetNode("strings", out var stringsNode) && stringsNode is TomlTable stringsTableNode)
          locale._strings = new LocalizedStringTable(RecurseLeafStringNodes(stringsTableNode, value => value).ToDictionary());
        else
          throw new LocaleParserException("Could not find a \"strings\" table node");
        

        // Return the locale
        return locale;
      }
      catch (TomlParseException ex)
      {
        var errorStrings = string.Join("\n", ex.SyntaxErrors.Select(error => $"  at line {error.Line}, col {error.Column}: {error.Message}"));
        throw new LocaleParserException($"Could not parse TOML\n{errorStrings}");
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