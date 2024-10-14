using System.Text;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat.Tests.TestHelpers;

/// <summary>
///     Tracks the amount of times Parse is called.
/// </summary>
internal class TrackingPatternParser : IPatternParser
{
    /// <summary>
    ///     The real parser.
    /// </summary>
    private readonly PatternParser parser;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackingPatternParser"/> class.
    /// </summary>
    public TrackingPatternParser()
    {
        parser = new PatternParser();
    }

    /// <summary>
    ///     The amount of times Parse was called.
    /// </summary>
    public int ParseCount { get; private set; }

    /// <inheritdoc />
    public IFormatterRequestCollection Parse(StringBuilder source)
    {
        ParseCount++;
        return parser.Parse(source);
    }
}