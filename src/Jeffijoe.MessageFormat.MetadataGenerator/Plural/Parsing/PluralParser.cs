using System.Collections.Generic;
using System.Xml;
using System.Linq;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class PluralParser
    {
        private readonly XmlDocument _rulesDocument;
        private readonly HashSet<string> _excludedLocales;

        public PluralParser(XmlDocument rulesDocument, string[] excludedLocales)
        {
            _rulesDocument = rulesDocument;
            _excludedLocales = new HashSet<string>(excludedLocales);
        }

        public IEnumerable<PluralRule> Parse()
        {
            var root = _rulesDocument.DocumentElement!;
            
            foreach(XmlNode dataElement in root.ChildNodes)
            {
                if (dataElement.Name != "plurals")
                {
                    continue;
                }
                
                foreach (XmlNode rule in dataElement.ChildNodes)
                {
                    if(rule.Name == "pluralRules")
                    {
                        var parsed = ParseSingleRule(rule);
                        if (parsed != null)
                        {
                            yield return parsed;
                        }
                    }
                }
            }
        }

        private PluralRule? ParseSingleRule(XmlNode rule)
        {
            var locales = rule.Attributes!["locales"]!.Value.Split(' ');

            if (locales.All(l => _excludedLocales.Contains(l)))
            {
                return null;
            }

            var conditions = new List<Condition>();
            foreach (XmlNode condition in rule.ChildNodes)
            {
                if (condition.Name == "pluralRule")
                {
                    var count = condition.Attributes!["count"]!.Value;

                    // Ignore other, because other is basically everything else except for the conditions present
                    if (count == "other")
                        continue;

                    var ruleContent = condition.InnerText;

                    var ruleParser = new RuleParser(ruleContent);
                    var orConditions = ruleParser.ParseRuleContent();

                    conditions.Add(new Condition(count, ruleContent, orConditions));
                }
            }

            return new PluralRule(locales, conditions);
        }
    }
}
