using System.Collections.Generic;
using System.Diagnostics;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

/// <summary>
///     Represents the 'condition' part of the <see href="https://unicode.org/reports/tr35/tr35-numbers.html#plural-rules-syntax">LDML grammar</see>.
/// </summary>
/// <example>
///     Given the following 'pluralRule' tag:
///     &lt;pluralRule count="one"&gt;i = 1 and v = 0 @integer 1&lt;/pluralRule&gt;
///
///     A Condition instance would represent 'i = 1 and v = 0' as a single <see cref="OrCondition"/>.
/// </example>
/// <remarks>
///     The grammar defines a condition as a union of 'and_conditions', which we model as a
///     list of <see cref="OrCondition"/> that each internally tracks <see cref="OrCondition.AndConditions"/>.
/// </remarks>
[DebuggerDisplay("{{RuleDescription}}")]
public class Condition
{
    public Condition(string count, string ruleDescription, IReadOnlyList<OrCondition> orConditions)
    {
        Count = count;
        RuleDescription = ruleDescription;
        OrConditions = orConditions;
    }

    /// <summary>
    ///     The plural form this condition or rule defines, e.g., "one", "two", "few", "many", "other".
    /// </summary>
    public string Count { get; }

    /// <summary>
    ///     The original text of this rule, e.g., "i = 1 and v = 0 @integer 1".
    /// </summary>
    /// <remarks>
    ///     Note - this includes the sample text ('@integer 1') which gets stripped out
    ///     when parsing the rule's conditional logic.
    /// </remarks>
    public string RuleDescription { get; }

    /// <summary>
    ///     Parsed representation of <see cref="RuleDescription"/>.
    /// </summary>
    public IReadOnlyList<OrCondition> OrConditions { get; }
}