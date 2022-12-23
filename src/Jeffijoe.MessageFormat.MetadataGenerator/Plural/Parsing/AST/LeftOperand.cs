namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST
{
    public class ModuloOperand : ILeftOperand
    {
        public ModuloOperand(OperandSymbol operandSymbol, int modValue)
        {
            Operand = operandSymbol;
            ModValue = modValue;
        }

        public OperandSymbol Operand { get; }
        public int ModValue { get; }

        public override bool Equals(object? obj)
        {
            if (obj is ModuloOperand op)
                return op.Operand == Operand && op.ModValue == ModValue;

            return this == obj;
        }

        public override int GetHashCode()
        {
            return Operand.GetHashCode() + ModValue.GetHashCode();
        }
    }
}
