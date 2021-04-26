using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST
{
    public class Operation
    {
        public Operation(ILeftOperand operandLeft, Relation relation, IReadOnlyList<IRightOperand> operandRight)
        {
            OperandLeft = operandLeft;
            Relation = relation;
            OperandRight = operandRight;
        }

        public ILeftOperand OperandLeft { get; }

        public Relation Relation { get; }

        public IReadOnlyList<IRightOperand> OperandRight { get; }
    }
}
