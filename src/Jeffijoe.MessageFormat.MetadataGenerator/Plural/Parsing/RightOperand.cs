namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public interface IRightOperand
    {
    }

    public class NumberOperand : IRightOperand
    {
        public NumberOperand(int number)
        {
            Number = number;
        }

        public int Number { get; }

        public override bool Equals(object obj)
        {
            if (obj is NumberOperand n)
                return n.Number == Number;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }

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

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() + End.GetHashCode();
        }
    }
}
