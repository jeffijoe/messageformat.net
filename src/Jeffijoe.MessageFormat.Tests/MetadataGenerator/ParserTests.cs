using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

using System;
using System.Collections.Generic;
using System.Xml;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator
{
    public class ParserTests
    {
        [Fact]
        public void CanParseLocales()
        {
            var rules = ParseRules(@"
<supplementalData>
    <plurals type=""cardinal"">
        <pluralRules locales=""am as bn doi fa gu hi kn pcm zu"">
        </pluralRules>
    </plurals>
</supplementalData>
");

            var rule = Assert.Single(rules);
            var expected = new[]
            {
                "am", "as", "bn", "doi", "fa", "gu", "hi", "kn", "pcm", "zu"
            };
            var actual = rule.Locales;
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void OtherCountIsIgnored()
        {
            var rules = ParseRules(@"
<supplementalData>
    <plurals type=""cardinal"">
        <pluralRules locales=""am"">
            <pluralRule count=""other""> @integer 2~17, 100, 1000, 10000, 100000, 1000000, … @decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</pluralRule>
        </pluralRules>
    </plurals>
</supplementalData>
");
            var rule = Assert.Single(rules);
            Assert.Empty(rule.Conditions);
        }

        [Fact]
        public void CanParseSingleCount_RuleDescription_WithoutRelations()
        {
            var rules = ParseRules(GenerateXmlWithRuleContent("@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));

            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var expected = "@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …";
            Assert.Equal(expected, condition.RuleDescription);
        }

        [Fact]
        public void CanParseSingleCount_VisibleDigitsNumber()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent(@"v = 0 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var expected = new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) });

            AssertOperationEqual(expected, actual);
        }

        [Fact]
        public void CanParseSingleCount_IntegerDigits()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent(@"i = 0 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var expected = new Operation(new VariableOperand(OperandSymbol.IntegerDigits), Relation.Equals, new[] { new NumberOperand(0) });

            AssertOperationEqual(expected, actual);
        }

        [Fact]
        public void CanParseSingleCount_AbsoluteNumber()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent("n = 1 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var expected = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] {new NumberOperand(1) });

            AssertOperationEqual(expected, actual);
        }

        [Theory]
        [InlineData("n = 2 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …", Relation.Equals)]
        [InlineData("n != 2 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …", Relation.NotEquals)]
        public void CanParseVariousRelations(string ruleText, Relation expectedRelation)
        {
            var rules = ParseRules(GenerateXmlWithRuleContent(ruleText));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var expected = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), expectedRelation, new[] { new NumberOperand(2) });

            AssertOperationEqual(expected, actual);
        }

        [Fact]
        public void CanParseOrRules()
        {
            var rules = ParseRules(GenerateXmlWithRuleContent("n = 2 or n = 1 or n = 0 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            
            Assert.Equal(3, condition.OrConditions.Length);

            var actualFirst = Assert.Single(condition.OrConditions[0].AndConditions);
            var expectedFirst = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(2) });
            AssertOperationEqual(expectedFirst, actualFirst);

            var actualSecond = Assert.Single(condition.OrConditions[1].AndConditions);
            var expectedSecond = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(1) });
            AssertOperationEqual(expectedSecond, actualSecond);

            var actualThird = Assert.Single(condition.OrConditions[2].AndConditions);
            var expectedThird = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(0) });
            AssertOperationEqual(expectedThird, actualThird);
        }

        [Fact]
        public void CanParseAndRules()
        {
            var rules = ParseRules(GenerateXmlWithRuleContent("n = 2 and n = 1 and n = 0 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);

            var orCondition = Assert.Single(condition.OrConditions);
            Assert.Equal(3, orCondition.AndConditions.Length);

            var actualFirst = orCondition.AndConditions[0];
            var expectedFirst = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(2) });
            AssertOperationEqual(expectedFirst, actualFirst);

            var actualSecond = orCondition.AndConditions[1];
            var expectedSecond = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(1) });
            AssertOperationEqual(expectedSecond, actualSecond);

            var actualThird = orCondition.AndConditions[2];
            var expectedThird = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(0) });
            AssertOperationEqual(expectedThird, actualThird);
        }

        [Fact]
        public void CanParseModuloInLeftOperator()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent("n % 5 = 3 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var modulo = new ModuloOperand(OperandSymbol.AbsoluteValue, 5);
            var expected = new Operation(modulo, Relation.Equals, new[] { new NumberOperand(3) });

            AssertOperationEqual(expected, actual);
        }

        [Fact]
        public void CanParseRangeInRightOperator()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent("n = 3..5 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var range = new[] { new RangeOperand(3, 5) };
            var expected = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, range);

            AssertOperationEqual(expected, actual);
        }

        [Fact]
        public void CanParseCommaSeparatedInRightOperator()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent("n = 3,5,8, 10 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var range = new[] { new NumberOperand(3), new NumberOperand(5), new NumberOperand(8), new NumberOperand(10) };
            var expected = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, range);

            AssertOperationEqual(expected, actual);
        }

        [Fact]
        public void CanParseMixedCommaSeparatedAndRangeInRightOperator()
        {
            var rules = ParseRules(
                GenerateXmlWithRuleContent("n = 3,5..7,12,15 @integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …"));
            var rule = Assert.Single(rules);
            var condition = Assert.Single(rule.Conditions);
            var orCondition = Assert.Single(condition.OrConditions);
            var actual = Assert.Single(orCondition.AndConditions);
            var range = new IRightOperand[] { new NumberOperand(3), new RangeOperand(5, 7), new NumberOperand(12), new NumberOperand(15) };
            var expected = new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, range);

            AssertOperationEqual(expected, actual);
        }

        private static string GenerateXmlWithRuleContent(string ruleText)
        {
            return $@"
<supplementalData>
    <plurals type=""cardinal"">
        <pluralRules locales=""am"">
            <pluralRule count=""one"">{ruleText}</pluralRule>
        </pluralRules>
    </plurals>
</supplementalData>
";
        }

        private static void AssertOperationEqual(Operation expected, Operation actual)
        {
            Assert.Equal(expected.OperandLeft, actual.OperandLeft);
            Assert.Equal(expected.Relation, actual.Relation);
            Assert.Equal(expected.OperandRight, actual.OperandRight);
        }

        private static IEnumerable<PluralRule> ParseRules(string xmlText)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlText);

            var parser = new PluralParser(xml, Array.Empty<string>());

            return parser.Parse();
        }
    }
}
