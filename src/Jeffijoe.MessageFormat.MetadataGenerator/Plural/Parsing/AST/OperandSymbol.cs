namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

public enum OperandSymbol
{
    /// <summary>
    /// n - absolute value of the source number.
    /// </summary>
    AbsoluteValue,

    /// <summary>
    /// i - integer digits of n.
    /// </summary>
    IntegerDigits,

    /// <summary>
    /// v - number of visible fraction digits in n, with trailing zeros.
    /// </summary>
    VisibleFractionDigitNumber,

    /// <summary>
    /// w - number of visible fraction digits in n, without trailing zeros.
    /// </summary>
    VisibleFractionDigitNumberWithoutTrailingZeroes,

    /// <summary>
    /// f - number of visible fraction digits in n, with trailing zeros.
    /// </summary>
    VisibleFractionDigits,

    /// <summary>
    /// t - visible fraction digits in n, without trailing zeros.
    /// </summary>
    VisibleFractionDigitsWithoutTrailingZeroes,

    /// <summary>
    /// c - compact decimal exponent value: exponent of the power of 10 used in compact decimal formatting.
    /// </summary>
    ExponentC,

    /// <summary>
    /// e - currently, synonym for ‘c’. however, may be redefined in the future.
    /// </summary>
    ExponentE,
}