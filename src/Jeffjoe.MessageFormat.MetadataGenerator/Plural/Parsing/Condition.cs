using System.Diagnostics;

namespace Jeffjoe.MessageFormat.MetadataGenerator
{
    [DebuggerDisplay("{{RuleDescription}}")]
    public class Condition
    {
        public Condition(string count, string ruleDescription, OrCondition[] orConditions)
        {
            Count = count;
            RuleDescription = ruleDescription;
            OrConditions = orConditions;
        }

        public string Count { get; }

        public string RuleDescription { get; }

        public OrCondition[] OrConditions { get; }
    }
}
