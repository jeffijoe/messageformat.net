namespace Jeffjoe.MessageFormat.MetadataGenerator
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
