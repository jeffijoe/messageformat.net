using System.Text;
using BenchmarkDotNet.Attributes;
using Jeffijoe.MessageFormat;

namespace Jeffijoe.MessageFormat.Benchmarks;

[MemoryDiagnoser]
public class PoolBenchmarks
{
    private const int OperationsPerThread = 1000;

    [Benchmark]
    public void SingleThreadGetReturn()
    {
        for (var i = 0; i < OperationsPerThread; i++)
        {
            var sb = StringBuilderPool.Get();
            sb.Append("test");
            StringBuilderPool.Return(sb);
        }
    }

    [Params(1, 2, 4, 8)]
    public int ThreadCount { get; set; }

    [Benchmark]
    public void MultiThreadGetReturn()
    {
        var tasks = new Task[ThreadCount];
        for (var t = 0; t < ThreadCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (var i = 0; i < OperationsPerThread; i++)
                {
                    var sb = StringBuilderPool.Get();
                    sb.Append("test");
                    StringBuilderPool.Return(sb);
                }
            });
        }

        Task.WaitAll(tasks);
    }
}
