namespace Jeffjoe.MessageFormat.MetadataGenerator
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
