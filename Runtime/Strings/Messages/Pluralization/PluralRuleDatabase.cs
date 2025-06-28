using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a database of plural rules for a set of locales.
  /// </summary>
  public sealed class PluralRuleDatabase : IEnumerable<KeyValuePair<string, PluralRuleList>>
  {
    // Static instances of existing plural rule databases
    public static readonly PluralRuleDatabase plurals;
    public static readonly PluralRuleDatabase ordinalPlurals;


    // Static constructor
    static PluralRuleDatabase()
    {
      plurals = Parse(Resources.Load<TextAsset>("Pluralization/Plurals"));
      ordinalPlurals = Parse(Resources.Load<TextAsset>("Pluralization/OrdinalPlurals"));
    }


    // Internal state of the database
    private string _type;
    private Dictionary<string, PluralRuleList> _ruleLists = new Dictionary<string, PluralRuleList>();


    // Return the type of the pluralization rules in the database
    public string type => _type;

    // Return the codes for which a list of rules is defined 
    public IEnumerable<string> codes => _ruleLists.Keys;


    // Return if a list of rules for the specified code exists and store the rules
    public bool TryGetRules(string code, out PluralRuleList rules)
    {
      var result = _ruleLists.TryGetValue(code, out var rulesList);
      rules = result ? rulesList : null;
      return result;
    }

    // Return if a list of rules for the specified locale exists and store the rules
    public bool TryGetRules(Locale locale, out PluralRuleList rules)
    {
      return TryGetRules(locale.code, out rules) || TryGetRules(locale.culture.Name, out rules);
    }


    // Return a generic enumerator
    public IEnumerator<KeyValuePair<string, PluralRuleList>> GetEnumerator()
    {
      return _ruleLists.GetEnumerator();
    }

    // Return an enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }


    #region Parsing a rule database
    // Parse a plural rule database from a text reader in the CLDR XML format
    public static PluralRuleDatabase Parse(StringReader textReader)
    {
      var ruleDatabase = new PluralRuleDatabase();

      // Load the XML document
      var document = new XmlDocument();
      document.Load(textReader);

      // Get the <plurals> node
      var pluralsNode = document.SelectSingleNode("//supplementalData/plurals");
      ruleDatabase._type = pluralsNode.Attributes["type"].Value;

      // Iterate over the <pluralRules> nodes in the <plurals> node
      var pluralRulesNodes = pluralsNode.SelectNodes("pluralRules");
      foreach (XmlNode pluralRulesNode in pluralRulesNodes)
      {
        // Get the codes
        var codes = pluralRulesNode.Attributes["locales"]?.Value.Split(" ") ?? Enumerable.Empty<string>();

        // Iterate over the <pluralRule> nodes in the <pluralRules> node
        var list = new List<PluralRule>();
        foreach (XmlNode pluralRuleNode in pluralRulesNode)
        {
          // Get the keyword and rule
          if (!PluralKeywordExtensions.TryParseKeywordString(pluralRuleNode.Attributes["count"]?.Value ?? string.Empty, out var keyword))
            throw new FormatException("Expected one of \"zero\", \"one\", \"two\", \"few\", \"many\", \"other\"");
          var ruleString = pluralRuleNode.InnerText;

          var samplesIndex = ruleString.IndexOf('@');
          if (samplesIndex > -1)
            ruleString = ruleString[..(samplesIndex - 1)].TrimEnd();

          var rule = PluralRuleParser.Parse(keyword, ruleString);
          list.Add(rule);
        }

        // Create a new plural rule list and assign it to the database for each code
        var ruleList = new PluralRuleList(list);
        foreach (var code in codes)
          ruleDatabase._ruleLists[code] = ruleList;
      }

      return ruleDatabase;
    }

    // Parse a plural rule database from a text asset in the CLDR XML format
    public static PluralRuleDatabase Parse(TextAsset textAsset)
    {
      return Parse(new StringReader(textAsset.text));
    }
    #endregion
  }
}