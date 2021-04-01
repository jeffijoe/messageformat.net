namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public interface ILeftOperand
    {
    }

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

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Operand.GetHashCode();
        }
    }

    public class ModuloOperand : ILeftOperand
    {
        public ModuloOperand(OperandSymbol operandSymbol, int modValue)
        {
            Operand = operandSymbol;
            ModValue = modValue;
        }

        public OperandSymbol Operand { get; }
        public int ModValue { get; }

        public override bool Equals(object obj)
        {
            if (obj is ModuloOperand op)
                return op.Operand == Operand && op.ModValue == ModValue;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Operand.GetHashCode() + ModValue.GetHashCode();
        }
    }
}
