using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.Tests.TestHelpers;

/// <summary>
///     Used for testing. Will just pass through the input pattern.
/// </summary>
internal class FakeMessageFormatter : IMessageFormatter
{
    public CustomValueFormatter? CustomValueFormatter { get; set; }

    public string FormatMessage(string pattern, IReadOnlyDictionary<string, object?> argsMap) => pattern;

    public string FormatMessage(string pattern, object args) => pattern;
}