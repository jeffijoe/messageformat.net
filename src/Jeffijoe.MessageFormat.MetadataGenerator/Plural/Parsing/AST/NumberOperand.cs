namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

public class NumberOperand : IRightOperand
{
    public NumberOperand(int number)
    {
        Number = number;
    }

    public int Number { get; }

    public override bool Equals(object? obj)
    {
        if (obj is NumberOperand n)
            return n.Number == Number;

        return this == obj;
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }
}