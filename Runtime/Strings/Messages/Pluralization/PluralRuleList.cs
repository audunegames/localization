using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  // Class that defines a list of plural rules
  public sealed class PluralRuleList : IEnumerable<PluralRule>, IPluralizer
  {
    // Internal state of the plural rule list
    private readonly List<PluralRule> _rules;


    // Constructor
    public PluralRuleList(IEnumerable<PluralRule> rules)
    {
      _rules = new List<PluralRule>(rules);
    }

    // Internal constructor from an existing list
    internal PluralRuleList(List<PluralRule> rules)
    {
      _rules = rules;
    }


    // Select a plural keyword based on the specified number
    public PluralKeyword Pluralize(NumberContext number)
    {
      return _rules.Where(r => r.Matches(number)).Select(r => r.keyword).First();
    }


    // Return a generic enumerator
    public IEnumerator<PluralRule> GetEnumerator()
    {
      return _rules.GetEnumerator();
    }

    // Return an enumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}