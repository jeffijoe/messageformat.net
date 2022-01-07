namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST
{
    public class RangeOperand : IRightOperand
    {
        public RangeOperand(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; }
        public int End { get; }

        public override bool Equals(object obj)
        {
            if (obj is RangeOperand n)
                return n.Start == Start && n.End == End;

            return this == obj;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() + End.GetHashCode();
        }
    }
}
