using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat.Tests.TestHelpers;

/// <summary>
///     Fake literal parser.
/// </summary>
internal class FakeLiteralParser : ILiteralParser
{
    /// <summary>
    ///     The literal to return.
    /// </summary>
    private readonly Literal literal;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FakeLiteralParser"/> class.
    /// </summary>
    /// <param name="literal"></param>
    public FakeLiteralParser(Literal literal)
    {
        this.literal = literal;
    }

    /// <inheritdoc />
    public IEnumerable<Literal> ParseLiterals(StringBuilder sb)
    {
        yield return literal;
    }

    /// <summary>
    ///     Creates a fake literal parser that returns a single literal with
    ///     the specified inner text.
    /// </summary>
    /// <param name="innerText"></param>
    /// <returns></returns>
    public static ILiteralParser Of(string innerText) =>
        new FakeLiteralParser(new Literal(0, innerText.Length, 1, 1, innerText));
}