namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST
{
    public class OrCondition
    {
        public OrCondition(Operation[] andConditions)
        {
            AndConditions = andConditions;
        }

        public Operation[] AndConditions { get; }
    }
}
