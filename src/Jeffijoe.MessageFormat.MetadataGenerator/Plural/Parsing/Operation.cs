namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class Operation
    {
        public Operation(ILeftOperand operandLeft, Relation relation, IRightOperand[] operandRight)
        {
            OperandLeft = operandLeft;
            Relation = relation;
            OperandRight = operandRight;
        }

        public ILeftOperand OperandLeft { get; }

        public Relation Relation { get; }

        public IRightOperand[] OperandRight { get; }
    }
}
