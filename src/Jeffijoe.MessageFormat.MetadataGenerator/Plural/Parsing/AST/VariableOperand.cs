namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST
{
    public class VariableOperand : ILeftOperand
    {
        public VariableOperand(OperandSymbol operand)
        {
            Operand = operand;
        }

        public OperandSymbol Operand { get; }

        public override bool Equals(object obj)
        {
            if (obj is VariableOperand op)
                return op.Operand == Operand;

            return this == obj;
        }

        public override int GetHashCode()
        {
            return Operand.GetHashCode();
        }
    }
}