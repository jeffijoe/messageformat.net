using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

public class PluralParser
{
    private readonly XmlDocument _rulesDocument;
    private readonly HashSet<string> _excludedLocales;

    public PluralParser(XmlDocument rulesDocument, string[] excludedLocales)
    {
        _rulesDocument = rulesDocument;
        _excludedLocales = new HashSet<string>(excludedLocales);
    }

    /// <summary>
    ///     Parses the represented XML document into a new <see cref="PluralRuleSet"/>, and returns it.
    /// </summary>
    /// <returns>A <see cref="PluralRuleSet"/> containing the parsed plural rules of a single type.</returns>
    public PluralRuleSet Parse()
    {
        var index = new PluralRuleSet();
        ParseInto(index);
        return index;
    }

    /// <summary>
    /// Parses the represented XML document and merges the rules into the given <see cref="PluralRuleSet"/>.
    /// </summary>
    /// <param name="ruleIndex"></param>
    /// <exception cref="ArgumentException">If the CLDR XML is missing expected attributes.</exception>
    public void ParseInto(PluralRuleSet ruleIndex)
    {
        var root = _rulesDocument.DocumentElement!;

        foreach(XmlNode dataElement in root.ChildNodes)
        {
            if (dataElement.Name != "plurals")
            {
                continue;
            }

            var typeAttr = dataElement.Attributes["type"];
            if (!typeAttr.Specified)
            {
                throw new ArgumentException("CLDR ruleset document is unexpectedly missing 'type' attribute on 'plurals' element.");
            }

            string pluralType = typeAttr.Value;

            foreach (XmlNode rule in dataElement.ChildNodes)
            {
                if(rule.Name == "pluralRules")
                {
                    var parsed = ParseSingleRule(rule);
                    if (parsed != null)
                    {
                        ruleIndex.Add(pluralType, parsed);
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