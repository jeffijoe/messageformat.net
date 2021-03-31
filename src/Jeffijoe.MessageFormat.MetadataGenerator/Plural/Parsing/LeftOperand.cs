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
    }
}
