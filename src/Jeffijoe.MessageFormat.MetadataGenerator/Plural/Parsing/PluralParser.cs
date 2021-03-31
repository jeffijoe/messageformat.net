using System;
using System.Collections.Generic;
using System.Xml;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class PluralParser
    {
        private readonly XmlDocument _rulesDocument;

        public PluralParser(XmlDocument rulesDocument)
        {
            _rulesDocument = rulesDocument;
        }

        public IEnumerable<PluralRule> Parse()
        {
            var root = _rulesDocument.DocumentElement;

            foreach(XmlNode dataElement in root.ChildNodes)
            {
                if(dataElement.Name == "plurals")
                {
                    foreach (XmlNode rule in dataElement.ChildNodes)
                    {
                        if(rule.Name == "pluralRules")
                        {
                            yield return ParseSingleRule(rule);
                        }
                    }
                }
            }
        }

        private PluralRule ParseSingleRule(XmlNode rule)
        {
            var locales = rule.Attributes["locales"].Value.Split(' ');

            var conditions = new List<Condition>();
            foreach (XmlNode condition in rule.ChildNodes)
            {
                if(condition.Name == "pluralRule")
                {
                    var count = condition.Attributes["count"].Value;

                    // Ignore other, because other is basically everything else except for the conditions present
                    if (count == "other")
                        continue;

                    var ruleContent = condition.InnerText;

                    var ruleParser = new RuleParser(ruleContent);
                    var orConditions = ruleParser.ParseRuleContent();

                    conditions.Add(new Condition(count, ruleContent, orConditions));
                }
            }

            return new PluralRule(locales, conditions.ToArray());
        }


    }
}
