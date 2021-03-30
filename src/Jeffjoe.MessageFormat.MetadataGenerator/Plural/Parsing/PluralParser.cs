using System;
using System.Collections.Generic;
using System.Xml;

namespace Jeffjoe.MessageFormat.MetadataGenerator
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

                    var orConditions = ParseRuleContent(ruleContent);

                    conditions.Add(new Condition(count, ruleContent, orConditions));
                }
            }

            return new PluralRule(locales, conditions.ToArray());
        }

        private ReadOnlySpan<char> AdvanceWhitespace(ReadOnlySpan<char> characters)
        {
            while (!characters.IsEmpty && char.IsWhiteSpace(characters[0]))
            {
                characters = characters.Slice(1);
            }

            return characters;
        }

        private ReadOnlySpan<char> AdvanceAndSkipWhitespace(ReadOnlySpan<char> characters, int skipCharacters)
        {
            characters = characters.Slice(skipCharacters);
            characters = AdvanceWhitespace(characters);

            return characters;
        }

        private OrCondition[] ParseRuleContent(string ruleContent)
        {
            var conditions = new List<OrCondition>();

            var currentParsing = ruleContent.AsSpan();
            
            while(!currentParsing.IsEmpty)
            {
                currentParsing = AdvanceWhitespace(currentParsing);
                if (currentParsing.IsEmpty) continue;

                // Samples section
                if(currentParsing[0] == '@')
                {
                    break;
                }

                var operandSymbol = currentParsing[0] switch
                {
                    'v' => OperandSymbol.VisibleFractionDigitNumber,
                    'n' => OperandSymbol.AbsoluteValue,
                    var otherCharacter => throw new ArgumentException($"Invalid format, do not recognise character '{otherCharacter}'")
                };
                currentParsing = AdvanceAndSkipWhitespace(currentParsing, 1);
                if (currentParsing.IsEmpty) continue;

                var relation = currentParsing[0] switch
                {
                    '=' => Relation.Equals,
                    var otherCharacter => throw new ArgumentException($"Invalid format, do not recognise character '{otherCharacter}'")
                };

                currentParsing = AdvanceAndSkipWhitespace(currentParsing, 1);
                if (currentParsing.IsEmpty) continue;

                var (number, numberSize) = ParseNumber(currentParsing);
                currentParsing = AdvanceAndSkipWhitespace(currentParsing, numberSize);

                conditions.Add(new OrCondition(new[]
                {
                    new Operation(operandSymbol, relation, new[] { number })
                }));
            }

            return conditions.ToArray();
        }

        private static (int number, int numberSize) ParseNumber(ReadOnlySpan<char> currentParsing)
        {
            int numbersCount = 0;
            while (currentParsing.Length > numbersCount && char.IsNumber(currentParsing[numbersCount]))
            {
                numbersCount++;
            }

            var number = int.Parse(currentParsing.Slice(0, numbersCount));

            return (number, numbersCount);
        }
    }
}
