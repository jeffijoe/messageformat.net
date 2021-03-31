namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class Operation
    {
        public Operation(OperandSymbol operandLeft, Relation relation, int[] operandRight)
        {
            OperandLeft = operandLeft;
            Relation = relation;
            OperandRight = operandRight;
        }

        public OperandSymbol OperandLeft { get; }

        public Relation Relation { get; }

        public int[] OperandRight { get; }
    }
}
