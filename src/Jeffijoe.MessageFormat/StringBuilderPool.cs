using System.Text;

namespace Jeffijoe.MessageFormat;

internal static class StringBuilderPool
{
    private const int MaxBuilderCapacity = 4096;

    private static readonly ObjectPool<StringBuilder> SbPool = new(static () => new StringBuilder());

    public static StringBuilder Get()
    {
        return SbPool.Allocate();
    }

    public static void Return(StringBuilder sb)
    {
        // If the builder grew too large, just let it go
        // rather than returning it so it can get garbage-collected.
        if (sb.Capacity > MaxBuilderCapacity)
        {
            return;
        }

        sb.Clear();
        SbPool.Free(sb);
    }
}
