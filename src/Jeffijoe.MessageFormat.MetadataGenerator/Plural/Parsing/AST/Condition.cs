using System.Collections.Generic;
using System.Diagnostics;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

[DebuggerDisplay("{{RuleDescription}}")]
public class Condition
{
    public Condition(string count, string ruleDescription, IReadOnlyList<OrCondition> orConditions)
    {
        Count = count;
        RuleDescription = ruleDescription;
        OrConditions = orConditions;
    }

    public string Count { get; }

    public string RuleDescription { get; }

    public IReadOnlyList<OrCondition> OrConditions { get; }
}