using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Jeffijoe.MessageFormat;

namespace Jeffijoe.MessageFormat.Benchmarks;

[MemoryDiagnoser]
public class MessageFormatterBenchmarks
{
    private MessageFormatter _formatter = null!;

    private readonly Dictionary<string, object?> _simpleArgs = new() { ["name"] = "World" };

    private readonly Dictionary<string, object?> _pluralSimpleArgs = new() { ["count"] = 5 };

    private readonly Dictionary<string, object?> _selectSimpleArgs = new() { ["gender"] = "male" };

    private readonly Dictionary<string, object?> _pluralOffsetArgs = new() { ["count"] = 3 };

    private readonly Dictionary<string, object?> _nested2Args = new() { ["gender"] = "female", ["count"] = 1 };

    private readonly Dictionary<string, object?> _nested3Args = new()
    {
        ["gender"] = "male",
        ["count"] = 2,
        ["total"] = 10
    };

    [GlobalSetup]
    public void Setup()
    {
        _formatter = new MessageFormatter();
    }

    [Benchmark]
    public string SimpleSubstitution()
    {
        return _formatter.FormatMessage("{name}", _simpleArgs);
    }

    [Benchmark]
    public string PluralSimple()
    {
        return _formatter.FormatMessage(
            "{count, plural, one {1 thing} other {# things}}",
            _pluralSimpleArgs);
    }

    [Benchmark]
    public string SelectSimple()
    {
        return _formatter.FormatMessage(
            "{gender, select, male {He} female {She} other {They}}",
            _selectSimpleArgs);
    }

    [Benchmark]
    public string PluralWithOffset()
    {
        return _formatter.FormatMessage(
            "{count, plural, offset:1 =0 {Nobody} one {You and one other} other {You and # others}}",
            _pluralOffsetArgs);
    }

    [Benchmark]
    public string Nested2Levels()
    {
        return _formatter.FormatMessage(
            "{gender, select, male {{count, plural, one {He has 1 item} other {He has # items}}} female {{count, plural, one {She has 1 item} other {She has # items}}} other {{count, plural, one {They have 1 item} other {They have # items}}}}",
            _nested2Args);
    }

    [Benchmark]
    public string Nested3Levels()
    {
        return _formatter.FormatMessage(
            "{gender, select, male {{count, plural, one {He has 1 of {total} items} other {He has # of {total} items}}} female {{count, plural, one {She has 1 of {total} items} other {She has # of {total} items}}} other {{count, plural, one {They have 1 of {total} items} other {They have # of {total} items}}}}",
            _nested3Args);
    }

    [Params(1, 2, 4, 8)]
    public int ThreadCount { get; set; }

    [Benchmark]
    public void MultiThreadFormatMessage()
    {
        var args = new Dictionary<string, object?> { ["count"] = 5 };
        var pattern = "{count, plural, one {1 thing} other {# things}}";

        var tasks = new Task[ThreadCount];
        for (var t = 0; t < ThreadCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    _formatter.FormatMessage(pattern, args);
                }
            });
        }

        Task.WaitAll(tasks);
    }
}
