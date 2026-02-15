using System.Collections.Generic;
using System.Diagnostics;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

/// <summary>
/// Corresponds to a pluralRule tag in CLDR XML.
/// </summary>
/// <example>
/// &lt;pluralRule count="one"&gt;i = 1 and v = 0 @integer 1&lt;/pluralRule&gt;
/// </example>
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
    /// The plural form this condition or rule defines, e.g., "one", "two", "few", "many", "other".
    /// </summary>
    public string Count { get; }

    /// <summary>
    /// The original text of this rule, e.g., "i = 1 and v = 0 @integer 1".
    /// </summary>
    public string RuleDescription { get; }

    /// <summary>
    /// Parsed representation of <see cref="RuleDescription"/>.
    /// </summary>
    public IReadOnlyList<OrCondition> OrConditions { get; }
}