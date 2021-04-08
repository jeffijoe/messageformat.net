namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST
{
    public enum OperandSymbol
    {
        /// <summary>
        /// n - absolute value of the source number.
        /// </summary>
        AbsoluteValue,

        /// <summary>
        /// v - number of visible fraction digits in n, with trailing zeros.
        /// </summary>
        VisibleFractionDigitNumber,

        /// <summary>
        /// i - integer digits of n.
        /// </summary>
        IntegerDigits,
    }
}
