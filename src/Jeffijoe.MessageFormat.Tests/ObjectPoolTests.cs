using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests;

public class ObjectPoolTests
{
    [Fact]
    public void Allocate_WhenPoolEmpty_ReturnsNewObject()
    {
        var pool = new ObjectPool<StringBuilder>(() => new StringBuilder());
        var sb = pool.Allocate();
        Assert.NotNull(sb);
    }

    [Fact]
    public void Free_ThenAllocate_ReturnsSameInstance()
    {
        var pool = new ObjectPool<StringBuilder>(() => new StringBuilder());
        var sb = pool.Allocate();
        pool.Free(sb);
        var sb2 = pool.Allocate();
        Assert.Same(sb, sb2);
    }

    [Fact]
    public void Allocate_BeyondPoolSize_CreatesNewObjects()
    {
        var pool = new ObjectPool<StringBuilder>(() => new StringBuilder(), size: 2);
        var a = pool.Allocate();
        var b = pool.Allocate();
        var c = pool.Allocate();
        Assert.NotSame(a, b);
        Assert.NotSame(b, c);
        Assert.NotSame(a, c);
    }

    [Fact]
    public void Free_BeyondPoolSize_DoesNotThrow()
    {
        var pool = new ObjectPool<StringBuilder>(() => new StringBuilder(), size: 2);
        var a = pool.Allocate();
        var b = pool.Allocate();
        var c = pool.Allocate();
        pool.Free(a);
        pool.Free(b);
        pool.Free(c); // exceeds pool size, should silently discard
    }

    [Fact]
    public async Task ConcurrentAllocateAndFree_DoesNotThrow()
    {
        var pool = new ObjectPool<StringBuilder>(() => new StringBuilder());
        const int ThreadCount = 8;
        const int Iterations = 1000;

        var tasks = new Task[ThreadCount];
        for (var t = 0; t < ThreadCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (var i = 0; i < Iterations; i++)
                {
                    var sb = pool.Allocate();
                    sb.Append("test");
                    var output = sb.ToString();
                    // Assert we didn't get a dirty builder with data still left in it.
                    Assert.Equal("test", output);
                    sb.Clear();
                    pool.Free(sb);
                }
            });
        }

        await Task.WhenAll(tasks);
    }
}
